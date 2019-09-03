using System;

namespace mscfreshman.Data
{
    public class CrackRecord
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public int ProblemId { get; set; }
        public string? UserId { get; set; }

        ///<summary>
        /// 结果 0 -- 答案错误, 1 -- 答案正确, 2 -- 非法操作: 非连续作答, 3 -- 非法操作: 提交无效题目 Id
        ///</summary>
        public int Result { get; set; }
        public string? Content { get; set; }
    }
}
