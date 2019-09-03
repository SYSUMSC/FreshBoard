using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace mscfreshman.Data.Identity
{
    // 如需为用户信息添加字段，请在这里添加对应属性
    // 添加完成后需要更新数据库定义：
    // 1. 创建一个 migration：dotnet ef migrations add 名称
    // 2. 使用创建的 migration 自动修改数据库定义：dotnet ef database update
    public class FreshBoardUser : IdentityUser // IdentityUser 中已有 PhoneNumber、Email、UserName 字段，无需再建立
    {
        // /// <summary>
        // /// 解谜进度
        // /// </summary>
        // public int CrackProgress { get; set; }

        /// <summary>
        /// 用户权限 1 -- admin, other -- 普通权限
        /// </summary>
        public int Privilege { get; set; }

        public virtual ICollection<UserData> UserData { get; set; }
            = new HashSet<UserData>();
#nullable disable
        public virtual Application Application { get; set; }
#nullable enable
    }
}