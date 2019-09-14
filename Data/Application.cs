#nullable disable

using System.Collections.Generic;
using FreshBoard.Data.Identity;

namespace FreshBoard.Data
{
    public class Application
    {
        public string UserId { get; set; }
        public virtual FreshBoardUser User { get; set; }

        public int PeriodId { get; set; }
        public virtual ApplicationPeriod Period { get; set; }

        public bool? IsSuccessful { get; set; }

        public virtual ICollection<ApplicationPeriodData> Datas { get; set; }
            = new HashSet<ApplicationPeriodData>();
    }
}
