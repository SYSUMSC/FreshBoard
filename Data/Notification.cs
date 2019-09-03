using System;

namespace FreshBoard.Data
{
    public class Notification
    {
        public int Id { get; set; }
        /// <summary>
        /// 消息展示时间
        /// </summary>
        public DateTime Time { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        /// <summary>
        /// 接收模式
        /// 0 -- 无推送, 仅管理后台可见
        /// 1 -- 全体成员, Targets 留空
        /// 2 -- 特定部门, Targets 填部门编号
        /// 3 -- 特定用户, Targets 填写用户 uid
        /// 4 -- 特定权限, Targets 填写权限编号
        /// 多个 Targets 可用 | 分割
        /// </summary>
        public int Mode { get; set; }
        public string? Targets { get; set; }
        public bool? HasPushed { get; set; }
    }
}
