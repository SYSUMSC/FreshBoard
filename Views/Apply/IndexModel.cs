using System.Collections.Generic;

namespace mscfreshman.Views.Apply
{
    public class IndexModel
    {
        public class PeriodData
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public bool Editable { get; set; }
            public string Value { get; set; }
        }

        public class Period
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Summary { get; set; }
            public string Description { get; set; }
            public bool UserApproved { get; set; }

            public IEnumerable<PeriodData> Datas { get; set; }
        }

        public int CurrentPeriod { get; set; }
        public bool? ApplicationIsSuccessful { get; set; }

        public IEnumerable<Period> Periods { get; set; }
    }
}
