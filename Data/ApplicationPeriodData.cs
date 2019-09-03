using System;
using System.Collections.Generic;

namespace FreshBoard.Data
{
    public class ApplicationPeriodData
    {
        public string? ApplicationId { get; set; }
        public Application? Application { get; set; }

        public int DataTypeId { get; set; }
        public ApplicationPeriodDataType? DataType { get; set; }

        public string? Value { get; set; }
    }
}
