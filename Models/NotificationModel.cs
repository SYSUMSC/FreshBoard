using System;

namespace mscfreshman.Models
{
    public class NotificationModel
    {
        public int Id { get; set; }
        public string Time { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool HasRead { get; set; }
        public string Preview { get; set; }
    }
}