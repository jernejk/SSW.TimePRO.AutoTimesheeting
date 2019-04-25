namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.Crm
{
    public class CrmAppointmentModel
    {
        public string id { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string title { get; set; }
        public bool allDay { get; set; }
        public bool editable { get; set; }
        public string clientId { get; set; }
        public string projectId { get; set; }
        public string iterationId { get; set; }
    }
}
