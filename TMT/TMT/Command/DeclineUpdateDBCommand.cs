using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TMT.ViewModel;

namespace TMT.Command
{
    class DeclineUpdateDBCommand : ICommand
    {
        private MainViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the DeclineUpdateDBCommand class.
        /// </summary>
        /// <param name="viewModel"></param>
        public DeclineUpdateDBCommand(MainViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public void Execute(object parameter)
        {
            this.viewModel.Decline();
        }
    }
}
