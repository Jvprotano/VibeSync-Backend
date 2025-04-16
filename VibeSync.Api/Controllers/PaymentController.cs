using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using VibeSync.Api.Controllers.Base;
using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Application.UseCases;
using VibeSync.Domain.Domains;
using VibeSync.Infrastructure.Services;

namespace VibeSync.Api.Controllers;

[Route("api/[controller]")]
public class PaymentController(
    ILogger<PaymentController> logger,
    IUserPlanRepository userPlanRepository,
    CreateCheckoutSessionUseCase createCheckoutSessionUseCase) : BaseController(logger)
{
    [HttpPost("create-checkout-session")]
    [ProducesResponseType(typeof(CheckoutResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CheckoutRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new ErrorResponse("User ID not found in token.", StatusCodes.Status401Unauthorized));

        return await Handle(() => createCheckoutSessionUseCase.Execute(request with { UserId = Guid.Parse(userId) }));
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
                var session = stripeEvent.Data.Object as Session;

                var userId = session!.Metadata["userId"] ?? string.Empty;
                var planId = session.Metadata["planId"];
                var subscriptionId = session.SubscriptionId;

                var subscriptionService = new SubscriptionService();
                var subscription = await subscriptionService.GetAsync(subscriptionId);

                var userPlan = new UserPlan(
                    userId,
                    subscription.CustomerId,
                    subscriptionId,
                    new Guid(planId),
                    subscription.StartDate,
                    subscription.StartDate.AddMonths(1),
                    true);

                await userPlanRepository.AddAsync(userPlan);
            }
            else if (stripeEvent.Type == EventTypes.InvoicePaid)
            {
                var invoice = stripeEvent.Data.Object as Invoice;
                var userPlan = await userPlanRepository.GetByUserIdAsync(invoice!.Metadata["userId"]);

                userPlan!.Renew(userPlan.StripeSubscriptionId, DateTime.UtcNow.AddMonths(1));

                await userPlanRepository.UpdateAsync(userPlan);
            }
            else if (stripeEvent.Type == EventTypes.InvoicePaymentFailed)
            {
                var invoice = stripeEvent.Data.Object as Invoice;

                var userPlan = await userPlanRepository.GetByUserIdAsync(invoice!.Metadata["userId"]);
                userPlan!.Cancel();
                await userPlanRepository.UpdateAsync(userPlan);
            }
            else if (stripeEvent.Type == EventTypes.CustomerSubscriptionDeleted)
            {
                var subscription = stripeEvent.Data.Object as Subscription;

                var userPlan = await userPlanRepository.GetByUserIdAsync(subscription!.Metadata["userId"]);
                userPlan!.Cancel();
                await userPlanRepository.UpdateAsync(userPlan);
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
