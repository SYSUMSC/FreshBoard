using Octokit;

namespace FreshBoard.Models
{
    public class GitItemModel
    {
        public string Name { get; set; } = "";
        public string Path { get; set; } = "";
        public string Uri { get; set; } = "";
        public int Size { get; set; }
        public ContentType Type { get; set; }
    }
}
