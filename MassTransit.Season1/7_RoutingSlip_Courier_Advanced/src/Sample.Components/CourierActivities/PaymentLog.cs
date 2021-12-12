namespace Sample.Components.CourierActivities
{
    public record PaymentLog
    {
        public string AuthorizationCode { get; init; } = string.Empty;
    }
}