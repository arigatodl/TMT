namespace TMT.Command
{
    using System;
    using System.Windows.Input;
    using TMT.ViewModel;

    class GenerateCommand : ICommand
    {
        private MongolianGeneratorViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the GenerateCommand class.
        /// </summary>
        /// <param name="viewModel"></param>
        public GenerateCommand(MongolianGeneratorViewModel viewModel)
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
            _viewModel.Generate();
        }
    }
}
