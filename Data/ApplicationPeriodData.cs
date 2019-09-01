using System;
using System.Collections.Generic;

namespace mscfreshman.Data
{
    public class ApplicationPeriodData
    {
        public int ApplicationId { get; set; }
        public Application Application { get; set; }

        public int DataTypeId { get; set; }
        public ApplicationPeriodDataType DataType { get; set; }

        public string Value { get; set; }
    }
}
