using System;

namespace mscfreshman.Data
{
    public class Notification
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// 接收模式
        /// 1 -- 全体成员, Targets 留空
        /// 2 -- 特定部门, Targets 填部门编号
        /// 3 -- 特定用户, Targets 填写用户 uid
        /// </summary>
        public int Mode { get; set; }
        public string Targets { get; set; }
    }
}
