using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TMT.ViewModel;

namespace TMT.Command
{
    using TMT.Model;

    class ShowDetailsCommand : ICommand
    {
        private DandS dandS;

        /// <summary>
        /// Initializes a new instance of the TranslateViaTextCommand class.
        /// </summary>
        /// <param name="viewModel"></param>
        public ShowDetailsCommand(DandS dandS)
        {
            this.dandS = dandS;
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
            this.dandS.IsShown = !this.dandS.IsShown;
        }
    }
}
