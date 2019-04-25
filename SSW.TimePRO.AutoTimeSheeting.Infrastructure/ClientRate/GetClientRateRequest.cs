namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.ClientRate
{
    public class GetClientRateRequest : BaseTimeProRequest
    {
        public GetClientRateRequest(string tenantUrl, string empID, string clientID, string token)
            : base(tenantUrl, token)
        {
            EmpID = empID;
            ClientID = clientID;
        }

        public string EmpID { get; }
        public string ClientID { get; }
    }
}
