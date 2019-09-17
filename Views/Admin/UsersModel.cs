using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FreshBoard.Views.Admin
{
    public class UsersModel
    {
        public IEnumerable<UserItem> Users { get; set; } = new HashSet<UserItem>();
        public IEnumerable<PeriodItem> PossiblePeriods { get; set; } = new HashSet<PeriodItem>();

        public IEnumerable<SelectListItem> ApplicationStates
        {
            get => new List<SelectListItem>()
            {
                new SelectListItem("待定", "null"),
                new SelectListItem("已通过", "true"),
                new SelectListItem("未通过", "false"),
            };
        }

        public IEnumerable<SelectListItem> PeriodSelections
        {
            get => PossiblePeriods.Select(d => new SelectListItem(d.Name, d.Id.ToString()));
        }

        public class UserItem
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public string Period { get; set; } = string.Empty;
            public int PuzzleProgress { get; set; }
            public bool HasPrivilege { get; set; }
        }

        public class PeriodItem
        {
            public string Name { get; set; } = string.Empty;
            public int Id { get; set; }

            // public IEnumerable<PeriodItemDataType> DataTypes { get; set; } = new HashSet<PeriodItemDataType>();
        }

        public class PeriodItemDataType
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}
