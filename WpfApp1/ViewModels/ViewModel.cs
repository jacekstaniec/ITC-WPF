using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using WpfApp1.Commands;

namespace WpfApp1.ViewModels
{
    public class ViewModel
    {
        public ICommand AddHeaderCommand { get; private set; }

        public ViewModel()
        {
            AddHeaderCommand = new Command(() => RepoManager.AddHeader(), true);
        }
    }
}
