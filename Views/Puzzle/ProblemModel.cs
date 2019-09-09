namespace FreshBoard.Views.Puzzle
{
    public enum MessageType
    {
        None, Success, Warning, Error
    }
    public class ProblemModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Script { get; set; }
        public string? Message { get; set; }
        public int Level { get; set; }
        public MessageType MessageType { get; set; }
        public bool ShowContent { get; set; }
        public string? Answer { get; set; }
    }
}
