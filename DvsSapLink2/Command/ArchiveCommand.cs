using System;
using System.Windows.Input;
using DvsSapLink2.Model;
using DvsSapLink2.ViewModel;

namespace DvsSapLink2.Command
{
    public class ArchiveCommand : ICommand
    {
        private readonly Configuration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveCommand"/> class.
        /// </summary>
        public ArchiveCommand(Configuration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Event raised to inform that the execute status of the command has changed
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        
        /// <summary>
        /// Returns a value indicating whether the command can be executed or not
        /// </summary>
        /// <param name="parameter"><see cref="AttributeFile"/> to be copied</param>
        /// <returns><c>true</c> if the command can be executed; <c>false</c> otherwise</returns>
        public bool CanExecute(object parameter)
        {
            if (!(parameter is MainViewModel)) return false;
            var viewModel = (MainViewModel) parameter;
            return viewModel.IsValid;
        }

        /// <summary>
        /// Executes the prepare archive command
        /// </summary>
        /// <param name="parameter"><see cref="AttributeFile"/> to be copied</param>
        public void Execute(object parameter)
        {
            var viewModel = (MainViewModel)parameter;
        }
    }
}