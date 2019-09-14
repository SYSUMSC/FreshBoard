using System.Collections.Generic;

namespace FreshBoard.Views.Admin
{
    public class UsersModel
    {
        public IEnumerable<UserItem> Users { get; set; } = new HashSet<UserItem>();
        public IEnumerable<PeriodItem> PossiblePeriods { get; set; } = new HashSet<PeriodItem>();

        public class UserItem
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public string Period { get; set; } = string.Empty;
            public bool HasPrivilege { get; set; }
        }

        public class PeriodItem
        {
            public string Name { get; set; } = string.Empty;
            public int Id { get; set; }

            public IEnumerable<PeriodItemDataType> DataTypes { get; set; } = new HashSet<PeriodItemDataType>();
        }

        public class PeriodItemDataType
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}
