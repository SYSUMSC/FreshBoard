namespace FreshBoard.Views.Problem
{
    public class GroundModel
    {
        public int Id { get; set; }
        public int Level { get; set; }
        public int ProblemId { get; set; }
        public string? ProblemName { get; set; }
        
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = "";
        public string? Answer { get; set; }
    }
}