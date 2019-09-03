namespace FreshBoard.Data
{
    public class Problem
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Script { get; set; }
        public int Level { get; set; }
        public string? Answer { get; set; }
    }
}
