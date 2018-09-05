using System;

namespace mscfreshman.Data.Identity
{
    public class OtherInfoTemplate
    {
        [Name("自我介绍")]
        public string introduction { get; set; }

        [Name("优点")]
        public string advantages { get; set; }

    }

    public class NameAttribute : Attribute
    {
        public NameAttribute(string v)
        {
            Name = v;
        }

        public string Name { get; }
    }
}