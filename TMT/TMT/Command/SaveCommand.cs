namespace TMT.Command
{
    using System;
    using System.Windows.Input;
    using TMT.ViewModel;

    class SaveCommand : ICommand
    {
        private IdiomViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the GenerateCommand class.
        /// </summary>
        /// <param name="viewModel"></param>
        public SaveCommand(IdiomViewModel viewModel)
        {
            this._viewModel = viewModel;
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
            _viewModel.SaveIdiom();
        }
    }
}
