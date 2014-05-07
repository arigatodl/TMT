﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TMT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public class RelayCommand : ICommand
        {
            #region Fields
            readonly Action<object> _execute;
            readonly Predicate<object> _canExecute;
            #endregion // Fields

            #region Constructors
            public RelayCommand(Action<object> execute) : this(execute, null) { }
            public RelayCommand(Action<object> execute, Predicate<object> canExecute)
            {
                if (execute == null) throw new ArgumentNullException("execute");
                _execute = execute;
                _canExecute = canExecute;
            }
            #endregion // Constructors

            #region ICommand Members [DebuggerStepThrough]
            public bool CanExecute(object parameter)
            {
                return _canExecute == null ? true : _canExecute(parameter);
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public void Execute(object parameter)
            {
                _execute(parameter);
            }

            #endregion // ICommand Members
        }
    }
}
