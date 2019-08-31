using System.Collections.Generic;

namespace mscfreshman.Views.User
{
    public class IndexModel
    {
        public class PersonalDataRow
        {
            public int DataTypeId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Value { get; set; }
        }

        public IEnumerable<PersonalDataRow> PersonalData { get; set; }
    }
}
