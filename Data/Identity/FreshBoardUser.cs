using Microsoft.AspNetCore.Identity;

namespace mscfreshman.Data.Identity
{
    //如需为用户信息添加字段，请在这里添加对应属性
    //添加完成后需要更新数据库定义：
    //1. 创建一个 migration：dotnet ef migrations add 名称
    //2. 使用创建的 migration 自动修改数据库定义：dotnet ef database update
    public class FreshBoardUser : IdentityUser
    {
        [PersonalData]
        public int Grade { get; set; }

        [PersonalData]
        public string Name { get; set; }

        [PersonalData]
        public string QQ { get; set; }
    }
}