using System.Collections.Generic;

namespace FreshBoard.Data
{
    public class ApplicationPeriodDataType
    {
        public ApplicationPeriodDataType()
        {

        }
        public ApplicationPeriodDataType(
            ApplicationPeriod period,
            string title = "",
            string description = "",
            bool userVisible = true,
            bool userEditable = true)
        {
            Title = title;
            Description = description;
            UserVisible = userVisible;
            UserEditable = userEditable;
            Period = period;
            PeriodId = period.Id;
        }

        public int Id { get; set; }

        public string Title { get; set; } = "";

        public string Description { get; set; } = "";

        public bool? UserVisible { get; set; }

        public bool? UserEditable { get; set; }

        public int PeriodId { get; set; }
        public ApplicationPeriod Period { get; set; } = new ApplicationPeriod();

        public ICollection<ApplicationPeriodData> PeriodDatas { get; set; }
            = new HashSet<ApplicationPeriodData>();
    }
}
