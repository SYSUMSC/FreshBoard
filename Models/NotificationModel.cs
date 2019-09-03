using System;

namespace FreshBoard.Models
{
    public class NotificationModel
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public bool HasRead { get; set; }
        public string? Preview { get; set; }
    }
}