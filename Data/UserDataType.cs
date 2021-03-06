using System.Collections.Generic;

namespace FreshBoard.Data
{
    public class UserDataType
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<UserData> UserDatas { get; set; } = new HashSet<UserData>();
    }
}
