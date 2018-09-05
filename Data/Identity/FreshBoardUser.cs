using Microsoft.AspNetCore.Identity;
using System;

namespace mscfreshman.Data.Identity
{
    //如需为用户信息添加字段，请在这里添加对应属性
    //添加完成后需要更新数据库定义：
    //1. 创建一个 migration：dotnet ef migrations add 名称
    //2. 使用创建的 migration 自动修改数据库定义：dotnet ef database update
    public class FreshBoardUser : IdentityUser //IdentityUser 中已有 PhoneNumber、Email、UserName 字段，无需再建立
    {
        [PersonalData]
        public string Name { get; set; }

        [PersonalData]
        public int Grade { get; set; }

        [PersonalData]
        public string QQ { get; set; }

        [PersonalData]
        public string WeChat { get; set; }

        /// <summary>
        /// 政治面貌 0 -- 群众, 1 -- 共青团员, 2 -- 共产党员, 3 -- 中共预备党员, 4 -- 无党派人士, 5 -- 其他
        /// </summary>
        [PersonalData]
        public int CPCLevel { get; set; }

        /// <summary>
        /// 性别 1 -- 男, 2 -- 女
        /// </summary>
        [PersonalData]
        public int Sexual { get; set; }

        [PersonalData]
        public int SchoolNumber { get; set; }

        [PersonalData]
        public string Institute { get; set; }

        [PersonalData]
        public string Major { get; set; }

        [PersonalData]
        public DateTime DOB { get; set; }

        [PersonalData]
        public string OtherInfo { get; set; }

        /// <summary>
        /// 部门 0 -- 暂无, 1 -- 行策, 2 -- 媒传, 3 -- 技术
        /// </summary>
        public int Department { get; set; }

        /// <summary>
        /// 录取状态 0 -- 暂无, 1 -- 等待一面, 2 -- 等待二面, 3 -- 录取失败, 4 -- 录取成功
        /// </summary>
        public int ApplyStatus { get; set; }

        /// <summary>
        /// 解谜进度
        /// </summary>
        public int CrackProgress { get; set; }
        
        /// <summary>
        /// 用户权限 1 -- admin, other -- 普通权限
        /// </summary>
        public int Privilege { get; set; }

    }
}