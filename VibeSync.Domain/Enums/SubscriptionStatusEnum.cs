namespace VibeSync.Domain.Enums;

public enum SubscriptionStatusEnum
{
    Unknown = 0,
    Active = 1,
    PastDue = 2,
    Canceled = 3,
    Pending = 4,
    Expired = 5,
    PaymentFailed = 6,
}