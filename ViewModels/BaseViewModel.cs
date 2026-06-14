using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Ubad.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        // ── INotifyPropertyChanged ────────────────────────────────

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected bool SetProperty<T>(ref T field, T value,
            [CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(name);
            return true;
        }

        // ── Loading State ─────────────────────────────────────────

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        private bool _hasError;
        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private bool _isEmpty;
        public bool IsEmpty
        {
            get => _isEmpty;
            set => SetProperty(ref _isEmpty, value);
        }

        public bool ShowContent => !IsLoading && !HasError && !IsEmpty;

        // ── Navigation ────────────────────────────────────────────

        protected async Task NavigateToAsync(string route) =>
            await Shell.Current.GoToAsync(route);

        protected async Task NavigateBackAsync() =>
            await Shell.Current.GoToAsync("..");

        // ── Commands ──────────────────────────────────────────────

        protected ICommand CreateCommand(Func<Task> execute, Func<bool>? canExecute = null) =>
            new AsyncCommand(execute, canExecute);

        protected ICommand CreateCommand<T>(Func<T, Task> execute, Func<T, bool>? canExecute = null) =>
            new AsyncCommand<T>(execute, canExecute);

        protected void SetError(string message)
        {
            HasError     = true;
            ErrorMessage = message;
            IsLoading    = false;
        }

        protected void ClearState()
        {
            HasError     = false;
            ErrorMessage = string.Empty;
            IsEmpty      = false;
        }

        protected void UpdateShowContent() =>
            OnPropertyChanged(nameof(ShowContent));
    }

    // ── Async Command ─────────────────────────────────────────────

    public class AsyncCommand : ICommand
    {
        private readonly Func<Task>  _execute;
        private readonly Func<bool>? _canExecute;
        private bool                 _isExecuting;

        public AsyncCommand(Func<Task> execute, Func<bool>? canExecute = null)
        {
            _execute    = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? p) =>
            !_isExecuting && (_canExecute?.Invoke() ?? true);

        public async void Execute(object? p)
        {
            if (!CanExecute(p)) return;
            _isExecuting = true;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            try   { await _execute(); }
            finally
            {
                _isExecuting = false;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void RaiseCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public class AsyncCommand<T> : ICommand
    {
        private readonly Func<T, Task>  _execute;
        private readonly Func<T, bool>? _canExecute;
        private bool                    _isExecuting;

        public AsyncCommand(Func<T, Task> execute, Func<T, bool>? canExecute = null)
        {
            _execute    = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? p) =>
            !_isExecuting && (_canExecute?.Invoke((T)p!) ?? true);

        public async void Execute(object? p)
        {
            if (p is not T param) return;
            _isExecuting = true;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            try   { await _execute(param); }
            finally
            {
                _isExecuting = false;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}