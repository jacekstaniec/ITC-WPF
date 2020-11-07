using System.Dynamic;

namespace WpfApp1.Models
{
    public class GitHub
    {
        private string _gitAttr;
        private string _gitValue;

        private GitHub() { }
        public GitHub(string GitHubAttr, string GitHubVal) { _gitAttr = GitHubAttr; _gitValue = GitHubVal; }
        public string GitAttr { get { return _gitAttr; } }
        public string GitValue { get { return _gitValue; } }
    }
}