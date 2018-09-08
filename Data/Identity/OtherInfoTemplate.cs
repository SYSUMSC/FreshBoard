using System;

namespace mscfreshman.Data.Identity
{
    public class OtherInfoTemplate
    {
        [Name("自我介绍")]
        public string introduction { get; set; }

        [Name("自身优点")]
        public string advantages { get; set; }

        [Name("对 MSC 的期待")]
        public string wishes { get; set; }

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