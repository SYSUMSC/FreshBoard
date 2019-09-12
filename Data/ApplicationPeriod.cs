using System.Collections.Generic;

namespace FreshBoard.Data
{
    public class ApplicationPeriod
    {
        public ApplicationPeriod()
        {

        }
        public ApplicationPeriod(uint order,
                                 string title = "",
                                 string summary = "",
                                 string description = "",
                                 bool userApproved = false)
        {
            Order = order;
            Title = title;
            Summary = summary;
            Description = description;
            UserApproved = userApproved;
        }

        public int Id { get; set; }

        public uint Order { get; set; }

        public string Title { get; set; } = "";

        public string Summary { get; set; } = "";

        public string Description { get; set; } = "";

        public bool UserApproved { get; set; }

        public virtual ICollection<Application> Applications { get; set; }
            = new HashSet<Application>();
        public virtual ICollection<ApplicationPeriodDataType> PeriodDataTypes { get; set; }
            = new HashSet<ApplicationPeriodDataType>();
    }
}
