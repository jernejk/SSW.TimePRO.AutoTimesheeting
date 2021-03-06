﻿using SSW.TimePRO.AutoTimeSheeting.Infrastructure.ClientRate;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.CollectData;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.TimeSheets.SuggestTimeSheet;

namespace SSW.TimePRO.AzureFunctions.Models
{
    class AutoCreateTimeSheetModel
    {
        public SuggestTimeSheetModel SuggestTimeSheet { get; set; }
        public ClientRateModel ClientRate { get; set; }
        public bool IsCreated { get; set; }
        public CollectDataModel Debug { get; set; }
    }
}
