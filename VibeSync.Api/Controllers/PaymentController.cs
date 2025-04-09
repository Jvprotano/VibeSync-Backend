using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using VibeSync.Api.Controllers.Base;
using VibeSync.Application.Responses;
using VibeSync.Domain.Domains;
using VibeSync.Infrastructure.Services;

namespace VibeSync.Api.Controllers;

[Route("api/[controller]")]
public class PaymentController(ILogger<PaymentController> logger, StripeService stripeService) : BaseController(logger)
{
    [Authorize]
    [HttpPost("create-checkout-session")]
    public async Task<IActionResult> CreateCheckoutSession()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new ErrorResponse("User ID not found in token.", StatusCodes.Status401Unauthorized));

        // var priceId = get from table plans

        var session = await stripeService.CreateCheckoutSessionAsync("price_1RBlW7QTScSq3LFhzyNd64Ky", Guid.NewGuid(), userId);

        return Ok(new { sessionId = session.Id, url = session.Url });
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            var stripeEvent = EventUtility.ParseEvent(json);

            if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
            {
                // Criar assinatura e vincular ao usuário

                var session = stripeEvent.Data.Object as Session;

                var userId = session!.Metadata["userId"] ?? string.Empty;
                var planId = session.Metadata["planId"];
                var subscriptionId = session.SubscriptionId;

                var subscriptionService = new SubscriptionService();
                var subscription = await subscriptionService.GetAsync(subscriptionId);

                var userPlan = new UserPlan(
                    new Guid(userId),
                    subscription.CustomerId,
                    subscriptionId,
                    new Guid(planId),
                    subscription.StartDate,
                    subscription.StartDate.AddMonths(1),
                    true);


                // Salve no banco de dados
            }
            if (stripeEvent.Type == EventTypes.InvoicePaid)
            {
                // Fatura paga. Renovar assinatura. 
                var invoice = stripeEvent.Data.Object as Invoice;
                // Salve no banco de dados
            }
            else if (stripeEvent.Type == EventTypes.InvoicePaymentFailed)
            {
                var invoice = stripeEvent.Data.Object as Invoice;
                // Then define and call a method to handle the successful payment intent.
                // handleInvoicePaymentFailed(invoice);
            }
            else if (stripeEvent.Type == EventTypes.InvoicePaymentSucceeded)
            {
                var invoice = stripeEvent.Data.Object as Invoice;
                // Then define and call a method to handle the successful payment intent.
                // handleInvoicePaymentSucceeded(invoice);
            }
            else if (stripeEvent.Type == EventTypes.CustomerSubscriptionUpdated)
            {
                // Atualizar plano do usuário
                var subscription = stripeEvent.Data.Object as Subscription;
                // Then define and call a method to handle the successful payment intent.
                // handleCustomerSubscriptionUpdated(subscription);
            }
            else if (stripeEvent.Type == EventTypes.CustomerSubscriptionDeleted)
            {
                // Cancelar assinatura do usuário
                var subscription = stripeEvent.Data.Object as Subscription;
                // Then define and call a method to handle the successful payment intent.
                // handleCustomerSubscriptionDeleted(subscription);
            }
            else
            {
                // Unexpected event type
                Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
            }
            return Ok();
        }
        catch (StripeException e)
        {
            logger.LogError(e, "Stripe webhook error: {Message}", e.Message);
            return BadRequest(new ErrorResponse("Stripe webhook error", StatusCodes.Status400BadRequest));
        }
    }

}
