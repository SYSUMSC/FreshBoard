using System;

namespace mscfreshman.Data.Identity
{
    public class OtherInfoTemplate
    {
        [Name("自我介绍、个人经历")]
        public string introduction { get; set; }

        [Name("自身优点、爱好特长")]
        public string advantages { get; set; }

        [Name("加入的理由、对 MSC 的期待")]
        public string wishes { get; set; }

        [Name("还想参加的其他部门")]
        public string otherdepartment { get; set; }

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