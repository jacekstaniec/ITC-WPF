using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;
using WpfApp1.Models;

namespace WpfApp1.ViewModels
{
    class GitHubRepoVM
    {
        private string _githubIdentity;
        private string _githubProject;
        private List<GitHubAttribute> _gitHubAttributes = new List<GitHubAttribute>();

        public GitHubRepoVM(string GHIdentity, string GHProject) { _githubIdentity = GHIdentity; _githubProject = GHProject; }

        internal async Task<List<GitHubAttribute>> GetGitHubRepoAsync()
        {
            Repository repository;
            var productInformation = new ProductHeaderValue(_githubIdentity);
            var client = new GitHubClient(productInformation);
            //try
            //{
            repository = await client.Repository.Get(_githubIdentity, _githubProject);
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.ToString());
            //}

            _gitHubAttributes.Clear();
            GitHubAttribute _createdat = new GitHubAttribute() { GitAttr = "created at", GitValue = repository.CreatedAt.ToString() };
            GitHubAttribute _description = new GitHubAttribute() { GitAttr = "description", GitValue = repository.Description.ToString() };
            GitHubAttribute _fullname = new GitHubAttribute() { GitAttr = "fullname", GitValue = repository.FullName.ToString() };
            GitHubAttribute _owner = new GitHubAttribute() { GitAttr = "owner", GitValue = repository.Owner.Login.ToString() };
            GitHubAttribute _access = new GitHubAttribute() { GitAttr = "access", GitValue = DateTime.Now.ToString("H:mm:ss") };
            _gitHubAttributes.Add(_createdat);
            _gitHubAttributes.Add(_description);
            _gitHubAttributes.Add(_fullname);
            _gitHubAttributes.Add(_owner);
            _gitHubAttributes.Add(_access);
            return _gitHubAttributes;
        }
    }
}
