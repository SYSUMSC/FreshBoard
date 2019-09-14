using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FreshBoard.Views.Admin
{
    public class UserModel
    {
        public class PeriodData
        {
            public int Id { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public bool Editable { get; set; }
            public string? Value { get; set; }
        }

        public class Period
        {
            public int Id { get; set; }
            public string? Title { get; set; }
            public string? Summary { get; set; }
            public string? Description { get; set; }
            public bool UserApproved { get; set; }

            public IEnumerable<PeriodData>? Datas { get; set; }
        }

        public class PersonalDataRow
        {
            public int DataTypeId { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? Value { get; set; }
        }

        public string Id { get; set; } = string.Empty;
        public int CurrentPeriod { get; set; }
        public bool? ApplicationIsSuccessful { get; set; }
        public string ApplicationIsSuccessfulStr { get => ApplicationIsSuccessful?.ToString() ?? "null"; }
        public IEnumerable<SelectListItem> ApplicationStates
        {
            get => new List<SelectListItem>()
            {
                new SelectListItem("待定", "null"),
                new SelectListItem("已通过", "true"),
                new SelectListItem("未通过", "false"),
            };
        }

        public IEnumerable<Period> Periods { get; set; } = new HashSet<Period>();
        public IEnumerable<SelectListItem> PeriodSelections
        {
            get => Periods.Select(d => new SelectListItem(d.Title, d.Id.ToString()));
        }
        public string PeriodSelection { get => CurrentPeriod.ToString(); }
        public IEnumerable<PersonalDataRow> PersonalData { get; set; } = new HashSet<PersonalDataRow>();
    }
}
