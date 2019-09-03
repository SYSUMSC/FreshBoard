using System;
using System.Collections.Generic;

namespace mscfreshman.Data
{
    public class Application
    {
        public string? UserId { get; set; }
        public Identity.FreshBoardUser? User { get; set; }

        public int PeriodId { get; set; }
        public ApplicationPeriod? Period { get; set; }

        public bool? IsSuccessful { get; set; }

        public ICollection<ApplicationPeriodData> Datas { get; set; }
            = new HashSet<ApplicationPeriodData>();
    }
}
