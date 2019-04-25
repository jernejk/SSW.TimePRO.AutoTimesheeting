namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure
{
    public abstract class BaseTimeProRequest
    {
        protected BaseTimeProRequest() { }

        protected BaseTimeProRequest(string tenantUrl)
            : this(tenantUrl, null) { }

        protected BaseTimeProRequest(string tenantUrl, string token)
        {
            TenantUrl = tenantUrl;
            Token = token;
        }

        public string TenantUrl { get; }
        public string Token { get; }
    }
}
