using FreshBoard.Data.Identity;

namespace FreshBoard.Data
{
    public class UserData
    {
        public string? UserId { get; set; }
        public string? Value { get; set; }

        public int DataTypeId { get; set; }
#nullable disable
        public virtual FreshBoardUser User { get; set; }
        public virtual UserDataType DataType { get; set; }
#nullable enable
    }
}
