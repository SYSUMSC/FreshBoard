using Microsoft.AspNetCore.Identity;

namespace mscfreshman.Data.Identity
{
    //如需为用户信息添加字段，请在这里添加对应属性
    //添加完成后需要更新数据库定义：
    //1. 创建一个 migration：dotnet ef migrations add 名称
    //2. 使用创建的 migration 自动修改数据库定义：dotnet ef database update
    public class FreshBoardUser : IdentityUser //IdentityUser 中已有 PhoneNumber、Email、UserName 字段，无需再建立
    {
        [PersonalData]
        public string Name { get; set; } //姓名

        [PersonalData]
        public int Grade { get; set; } //年级

        [PersonalData]
        public string QQ { get; set; } //QQ

        [PersonalData]
        public int CPCLevel { get; set; } //政治面貌 0 -- 群众, 1 -- 共青团员, 2 -- 党员

        [PersonalData]
        public int Sexual { get; set; } //性别 1 -- Male, 2 -- Female

        [PersonalData]
        public int SchoolNumber { get; set; } //学号

        [PersonalData]
        public string Institute { get; set; } //学院

        [PersonalData]
        public string Majority { get; set; } //专业


    }
}