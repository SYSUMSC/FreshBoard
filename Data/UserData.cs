using mscfreshman.Data.Identity;

namespace mscfreshman.Data
{
    public class UserData
    {
        public string UserId { get; set; }
        public string Value { get; set; }

        public int DataTypeId { get; set; }

        public virtual FreshBoardUser User { get; set; }
        public virtual UserDataType DataType { get; set; }
    }
}
