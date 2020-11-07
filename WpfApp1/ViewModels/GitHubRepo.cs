using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;
using WpfApp1.Models;

namespace WpfApp1.ViewModels
{
    class GitHubRepo
    {
        private string _githubIdentity;
        private string _githubProject;
        private List<GitHub> _gitHubAttributes = new List<GitHub>();

        public GitHubRepo(string GHIdentity, string GHProject) { _githubIdentity = GHIdentity; _githubProject = GHProject; }

        internal async Task<List<GitHub>> GetGitHubRepoAsync()
        {
            Repository repository;
            var productInformation = new ProductHeaderValue(_githubIdentity);
            var client = new GitHubClient(productInformation);
            repository = await client.Repository.Get(_githubIdentity, _githubProject);

            _gitHubAttributes.Clear();
            _gitHubAttributes.Add(new GitHub("created at", repository.CreatedAt.ToString()));
            _gitHubAttributes.Add(new GitHub("description", repository.Description.ToString()));
            _gitHubAttributes.Add(new GitHub("fullname", repository.FullName.ToString()));
            _gitHubAttributes.Add(new GitHub("owner", repository.Owner.Login.ToString()));
            _gitHubAttributes.Add(new GitHub("access", DateTime.Now.ToString("H:mm:ss")));

            return _gitHubAttributes;
        }
    }
}