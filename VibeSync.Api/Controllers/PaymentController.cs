using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using VibeSync.Api.Controllers.Base;
using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Extensions;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Application.UseCases;
using VibeSync.Domain.Domains;
using VibeSync.Domain.Enums;

namespace VibeSync.Api.Controllers;

[Route("api/[controller]")]
public class PaymentController : BaseController
{
    private readonly IUserPlanRepository _userPlanRepository;
    private readonly CreateCheckoutSessionUseCase _createCheckoutSessionUseCase;
    private readonly string _webhookSecret;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(
        ILogger<PaymentController> logger,
        IUserPlanRepository userPlanRepository,
        CreateCheckoutSessionUseCase createCheckoutSessionUseCase,
        IConfiguration configuration)
        : base(logger)
    {
        _logger = logger;
        _userPlanRepository = userPlanRepository;
        _createCheckoutSessionUseCase = createCheckoutSessionUseCase;
        _webhookSecret = configuration["Stripe:WebhookSecret"]!;
        if (string.IsNullOrEmpty(_webhookSecret))
        {
            _logger.LogCritical("############################################################");
            _logger.LogCritical("ERRO CRÍTICO: Stripe Webhook Secret 'Stripe:WebhookSecret' NÃO ESTÁ CONFIGURADO!");
            _logger.LogCritical("############################################################");
            throw new InvalidOperationException("Stripe Webhook Secret 'Stripe:WebhookSecret' não está configurado.");
        }
    }

    [HttpPost("create-checkout-session")]
    [ProducesResponseType(typeof(CheckoutResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CheckoutRequest request)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized(new ErrorResponse("User ID não encontrado no token.", StatusCodes.Status401Unauthorized));

        return await Handle(() => _createCheckoutSessionUseCase.Execute(request with { UserId = userId.Value }));
    }

    [HttpPost("webhook")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        Event stripeEvent;

        try
        {
            var signatureHeader = Request.Headers["Stripe-Signature"];
            stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, _webhookSecret, throwOnApiVersionMismatch: false);
            stripeEvent = EventUtility.ParseEvent(json);

            _logger.LogInformation("Stripe Event Received: Id={EventId}, Type={EventType}", stripeEvent.Id, stripeEvent.Type);
        }
        catch (JsonException e)
        {
            _logger.LogError(e, "Webhook Stripe: Erro ao desserializar JSON.");
            return BadRequest(new ErrorResponse("Payload JSON inválido.", StatusCodes.Status400BadRequest));
        }
        catch (StripeException e)
        {
            _logger.LogError(e, "Webhook Stripe: Erro na validação da assinatura ou construção do evento: {ErrorMessage}", e.Message);
            return BadRequest(new ErrorResponse($"Erro na assinatura/payload do Webhook Stripe: {e.Message}", StatusCodes.Status400BadRequest));
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Webhook Stripe: Erro inesperado durante o processamento inicial.");
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse("Erro interno inesperado ao processar webhook.", StatusCodes.Status500InternalServerError));
        }

        try
        {
            Task processingTask = stripeEvent.Type switch
            {
                EventTypes.CheckoutSessionCompleted => HandleCheckoutSessionCompletedAsync(stripeEvent),
                EventTypes.InvoicePaid => HandleInvoicePaidAsync(stripeEvent),
                EventTypes.InvoicePaymentFailed => HandleInvoicePaymentFailedAsync(stripeEvent),
                EventTypes.CustomerSubscriptionUpdated => HandleSubscriptionUpdatedAsync(stripeEvent),
                EventTypes.CustomerSubscriptionDeleted => HandleSubscriptionDeletedAsync(stripeEvent),

                _ => HandleUnknownEvent(stripeEvent)
            };
            await processingTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Webhook Stripe: Erro interno ao processar o evento Type={EventType}, Id={EventId}.", stripeEvent.Type, stripeEvent.Id);
        }

        return Ok();
    }

    // HandleCheckoutSessionCompletedAsync: Cria o registro inicial UserPlan
    private async Task HandleCheckoutSessionCompletedAsync(Event stripeEvent)
    {
        if (stripeEvent.Data.Object is not Session session) { /* Log e return */ return; }
        if (string.IsNullOrEmpty(session.SubscriptionId)) { /* Log e return */ return; } // Essencial para assinatura

        var stripeSubscriptionId = session.SubscriptionId;
        var stripeCustomerId = session.CustomerId;

        if (!session.Metadata.TryGetValue("userId", out var userIdStr) || string.IsNullOrEmpty(userIdStr) ||
            !session.Metadata.TryGetValue("planId", out var planIdStr) || !Guid.TryParse(planIdStr, out var planId))
        {
            _logger.LogError("Metadados userId/planId ausentes/inválidos na Session {SessionId} (Evento: {EventId})", session.Id, stripeEvent.Id);
            return;
        }

        if (await _userPlanRepository.GetByStripeSubscriptionIdAsync(stripeSubscriptionId) != null)
        {
            _logger.LogWarning("UserPlan para {SubscriptionId} já existe. Ignorando {EventType} (Id: {EventId})", stripeSubscriptionId, stripeEvent.Type, stripeEvent.Id);
            return;
        }

        // Obter dados da Subscription para preenchimento inicial
        Subscription subscription;
        try
        {
            var subService = new SubscriptionService();
            subscription = await subService.GetAsync(stripeSubscriptionId);
            if (subscription == null) throw new Exception($"Stripe Subscription {stripeSubscriptionId} não encontrada.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao obter Stripe Subscription {SubscriptionId} em {EventType} (Id: {EventId})", stripeSubscriptionId, stripeEvent.Type, stripeEvent.Id);
            return; // Não criar UserPlan sem dados da Subscription
        }

        var currentPeriodEnd = subscription.Items.Data.FirstOrDefault()?.CurrentPeriodEnd;

        var userPlan = new UserPlan(new Guid(userIdStr), planId, subscription.StartDate, currentPeriodEnd,
                                    stripeCustomerId, stripeSubscriptionId, StripeExtension.ConvertStripeSubscriptionStatus(subscription.Status));
        await _userPlanRepository.AddAsync(userPlan);
        _logger.LogInformation("UserPlan criado (Status: {Status}) para UserId {UserId}, PlanId {PlanId}, SubId {SubId}, Evento {EventId}",
            userPlan.Status, userPlan.UserId, userPlan.PlanId, userPlan.StripeSubscriptionId, stripeEvent.Id);
    }

    // HandleInvoicePaidAsync: Ativa/Renova a assinatura, atualiza PeriodEnd
    private async Task HandleInvoicePaidAsync(Event stripeEvent)
    {
        if (stripeEvent.Data.Object is not Invoice invoice) { /* Log e return */ return; }

        var stripeSubscriptionId = invoice.Lines?.Data
                                             .Select(line => line.SubscriptionId)
                                             .FirstOrDefault(subId => !string.IsNullOrEmpty(subId));

        if (string.IsNullOrEmpty(stripeSubscriptionId) || invoice.Status != "paid") { /* Log informativo e return */ return; }

        var userPlan = await _userPlanRepository.GetByStripeSubscriptionIdAsync(stripeSubscriptionId);

        if (userPlan == null)
        {
            _logger.LogWarning("UserPlan não encontrado para {SubscriptionId} em {EventType} (Id: {EventId}). Tentando recuperação...", stripeSubscriptionId, stripeEvent.Type, stripeEvent.Id);
            await TryRecoverUserPlanFromSubscription(stripeSubscriptionId, invoice.CustomerId);
            userPlan = await _userPlanRepository.GetByStripeSubscriptionIdAsync(stripeSubscriptionId);
            if (userPlan == null)
            {
                _logger.LogError("FALHA RECUPERAÇÃO: UserPlan para {SubscriptionId} não encontrado após {EventType} (Id: {EventId})", stripeSubscriptionId, stripeEvent.Type, stripeEvent.Id);
                return;
            }
        }

        // Obter dados atualizados da Subscription Stripe
        Subscription subscription;
        try
        {
            var subService = new SubscriptionService();
            subscription = await subService.GetAsync(stripeSubscriptionId);
            if (subscription == null) throw new Exception($"Stripe Subscription {stripeSubscriptionId} não encontrada.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao obter Stripe Subscription {SubscriptionId} em {EventType} (Id: {EventId})", stripeSubscriptionId, stripeEvent.Type, stripeEvent.Id);
            return; // Não atualizar sem dados confiáveis
        }

        // Atualizar UserPlan
        var currentPeriodEnd = subscription.Items.Data.FirstOrDefault()?.CurrentPeriodEnd;
        userPlan.Renew(currentPeriodEnd ?? default, StripeExtension.ConvertStripeSubscriptionStatus(subscription.Status));

        await _userPlanRepository.UpdateAsync(userPlan);
        _logger.LogInformation("UserPlan atualizado (Status: {Status}, PeriodEnd: {PeriodEnd}) para SubId {SubId}, Evento {EventId}",
             userPlan.Status, userPlan.CurrentPeriodEnd, userPlan.StripeSubscriptionId, stripeEvent.Id);

        // TODO: Lógica de negócio adicional (liberar acesso, email, etc.)
    }

    private async Task HandleInvoicePaymentFailedAsync(Event stripeEvent)
    {
        if (stripeEvent.Data.Object is not Invoice invoice) { /* Log e return */ return; }

        var stripeSubscriptionId = invoice.Lines?.Data
                                     .Select(line => line.SubscriptionId)
                                     .FirstOrDefault(subId => !string.IsNullOrEmpty(subId));

        if (string.IsNullOrEmpty(stripeSubscriptionId)) { /* Log informativo e return */ return; }

        var userPlan = await _userPlanRepository.GetByStripeSubscriptionIdAsync(stripeSubscriptionId);

        if (userPlan == null) { /* Log warning e return (recuperação opcional) */ return; }

        SubscriptionStatusEnum newStatus;
        try
        {
            var subService = new SubscriptionService();
            var sub = await subService.GetAsync(stripeSubscriptionId);
            newStatus = StripeExtension.ConvertStripeSubscriptionStatus(sub?.Status ?? "Unknown");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha obter status Subscription {SubscriptionId} em {EventType}. Usando 'PaymentFailed'.", stripeSubscriptionId, stripeEvent.Type);
            newStatus = SubscriptionStatusEnum.PaymentFailed; // Fallback
        }

        userPlan.UpdateStatus(newStatus);
        await _userPlanRepository.UpdateAsync(userPlan);
        _logger.LogInformation("UserPlan atualizado (Status: {Status}) para SubId {SubId} devido a {EventType} (Id: {EventId})",
            userPlan.Status, userPlan.StripeSubscriptionId, stripeEvent.Type, stripeEvent.Id);

        // TODO: Lógica de notificação ao usuário / período de carência?
    }

    private async Task HandleSubscriptionUpdatedAsync(Event stripeEvent)
    {
        if (stripeEvent.Data.Object is not Subscription subscription) { /* Log e return */ return; }

        var stripeSubscriptionId = subscription.Id;
        var userPlan = await _userPlanRepository.GetByStripeSubscriptionIdAsync(stripeSubscriptionId);

        // Recuperação se não encontrado
        if (userPlan == null)
        {
            _logger.LogWarning("UserPlan não encontrado para {SubscriptionId} em {EventType} (Id: {EventId}). Tentando recuperação...", stripeSubscriptionId, stripeEvent.Type, stripeEvent.Id);
            await TryRecoverUserPlanFromSubscription(stripeSubscriptionId, subscription.CustomerId);
            userPlan = await _userPlanRepository.GetByStripeSubscriptionIdAsync(stripeSubscriptionId);
            if (userPlan == null)
            {
                _logger.LogError("FALHA RECUPERAÇÃO: UserPlan para {SubscriptionId} não encontrado após {EventType} (Id: {EventId})", stripeSubscriptionId, stripeEvent.Type, stripeEvent.Id);
                return;
            }
        }

        var oldStatus = userPlan.Status;
        var oldPeriodEnd = userPlan.CurrentPeriodEnd;

        var currentPeriodEnd = subscription.Items.Data.FirstOrDefault()?.CurrentPeriodEnd;
        userPlan.Renew(currentPeriodEnd ?? default, StripeExtension.ConvertStripeSubscriptionStatus(subscription.Status));

        // Simplificado: não inclui lógica de mudança de plano aqui, mas pode ser adicionada se necessário
        // verificando subscription.Items.Data[0].Price.Id

        if (userPlan.Status != oldStatus || userPlan.CurrentPeriodEnd != oldPeriodEnd)
        {
            await _userPlanRepository.UpdateAsync(userPlan);
            _logger.LogInformation("UserPlan atualizado via {EventType} para SubId {SubId}. Status: {OldStatus}->{NewStatus}, PeriodEnd: {OldPeriodEnd}->{NewPeriodEnd}. Evento {EventId}",
                stripeEvent.Type, stripeSubscriptionId, oldStatus, userPlan.Status, oldPeriodEnd, userPlan.CurrentPeriodEnd, stripeEvent.Id);
        }
        else
        {
            _logger.LogInformation("Evento {EventType} (Id: {EventId}) para SubId {SubId} não resultou em mudanças no UserPlan.", stripeEvent.Type, stripeEvent.Id, stripeSubscriptionId);
        }
    }

    private async Task HandleSubscriptionDeletedAsync(Event stripeEvent)
    {
        if (stripeEvent.Data.Object is not Subscription subscription) { /* Log e return */ return; }

        var stripeSubscriptionId = subscription.Id;
        var userPlan = await _userPlanRepository.GetByStripeSubscriptionIdAsync(stripeSubscriptionId);

        if (userPlan == null) { /* Log warning e return */ return; }
        if (userPlan.Status == SubscriptionStatusEnum.Canceled) { /* Log info (idempotência) e return */ return; }

        userPlan.Cancel();

        await _userPlanRepository.UpdateAsync(userPlan);
        _logger.LogInformation("UserPlan marcado como 'Canceled' para SubId {SubId} via {EventType} (Id: {EventId}). Acesso expira {PeriodEnd}.",
            userPlan.StripeSubscriptionId, stripeEvent.Type, stripeEvent.Id, userPlan.CurrentPeriodEnd);

        // Lembrete: Precisa de lógica externa para revogar acesso após CurrentPeriodEnd
    }

    private Task HandleUnknownEvent(Event stripeEvent)
    {
        _logger.LogWarning("Webhook Stripe: Evento não tratado recebido: Type={EventType}, Id={EventId}", stripeEvent.Type, stripeEvent.Id);
        return Task.CompletedTask;
    }

    private async Task TryRecoverUserPlanFromSubscription(string stripeSubscriptionId, string stripeCustomerId)
    {
        // Idempotência dentro da recuperação
        if (await _userPlanRepository.GetByStripeSubscriptionIdAsync(stripeSubscriptionId) != null) { return; }

        _logger.LogInformation("Tentando recuperar UserPlan para {SubscriptionId}", stripeSubscriptionId);
        try
        {
            var subService = new SubscriptionService();
            var sub = await subService.GetAsync(stripeSubscriptionId);
            if (sub == null) { /* Log warning e return */ return; }

            if (!sub.Metadata.TryGetValue("userId", out var userIdStr) || string.IsNullOrEmpty(userIdStr) ||
                !sub.Metadata.TryGetValue("planId", out var planIdStr) || !Guid.TryParse(planIdStr, out var planId))
            {
                _logger.LogError("Recuperação falhou: Metadados ausentes/inválidos em {SubscriptionId}", stripeSubscriptionId);
                return;
            }

            var currentPeriodEnd = sub.Items.Data.FirstOrDefault()?.CurrentPeriodEnd;

            var recoveredPlan = new UserPlan(new Guid(userIdStr), planId, sub.StartDate, currentPeriodEnd,
                                            stripeCustomerId ?? sub.CustomerId, stripeSubscriptionId, StripeExtension.ConvertStripeSubscriptionStatus(sub.Status));
            await _userPlanRepository.AddAsync(recoveredPlan);
            _logger.LogInformation("UserPlan RECUPERADO (Status: {Status}) para UserId {UserId}, PlanId {PlanId}, SubId {SubId}",
                 recoveredPlan.Status, recoveredPlan.UserId, recoveredPlan.PlanId, recoveredPlan.StripeSubscriptionId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro durante recuperação do UserPlan para {SubscriptionId}.", stripeSubscriptionId);
        }
    }
}