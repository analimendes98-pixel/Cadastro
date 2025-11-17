using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Cadastro.Models;
namespace Cadastro.ViewModels;

public class ResumoEventoViewModel : INotifyPropertyChanged
{
    private readonly Evento _evento;

    public ResumoEventoViewModel(Evento evento)
    {
        _evento = evento ?? throw new ArgumentNullException(nameof(evento));
        _evento.PropertyChanged += Evento_PropertyChanged;

        EditCommand = new AsyncCommand(OnEditAsync);
        CompartilharCommand = new AsyncCommand(OnCompartilharAsync);
        VoltarCommand = new RelayCommand(() => VoltarRequested?.Invoke());

        UpdateDerived();
    }

    public event Func<Task>? EditRequested;
    public event Func<Task>? CompartilharRequested;
    public event Action? VoltarRequested;

    public string Nome
    {
        get => _evento.Nome;
        set
        {
            if (_evento.Nome == value) return;
            _evento.Nome = value;
            OnPropertyChanged();
        }
    }

    public DateTime DataInicio
    {
        get => _evento.DataInicio;
        set
        {
            if (_evento.DataInicio == value) return;
            _evento.DataInicio = value;
            OnPropertyChanged();
            UpdateDerived();
        }
    }

    public DateTime DataFim
    {
        get => _evento.DataFim;
        set
        {
            if (_evento.DataFim == value) return;
            _evento.DataFim = value;
            OnPropertyChanged();
            UpdateDerived();
        }
    }

    public int NumeroParticipantes
    {
        get => _evento.NumeroParticipantes;
        set
        {
            if (_evento.NumeroParticipantes == value) return;
            _evento.NumeroParticipantes = value;
            OnPropertyChanged();
            UpdateDerived();
        }
    }

    public string Local
    {
        get => _evento.Local;
        set
        {
            if (_evento.Local == value) return;
            _evento.Local = value;
            OnPropertyChanged();
        }
    }

    public decimal CustoPorParticipante
    {
        get => _evento.CustoPorParticipante;
        set
        {
            if (_evento.CustoPorParticipante == value) return;
            _evento.CustoPorParticipante = value;
            OnPropertyChanged();
            UpdateDerived();
        }
    }

 
    public TimeSpan Duracao { get; private set; }
    public int DuracaoDias { get; private set; }
    public decimal CustoTotal { get; private set; }

 
    public ICommand EditCommand { get; }
    public ICommand CompartilharCommand { get; }
    public ICommand VoltarCommand { get; }

    private void Evento_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {

        if (e.PropertyName is null) return;

        OnPropertyChanged(e.PropertyName);
        if (e.PropertyName == nameof(Evento.DataInicio) || e.PropertyName == nameof(Evento.DataFim) ||
            e.PropertyName == nameof(Evento.NumeroParticipantes) || e.PropertyName == nameof(Evento.CustoPorParticipante))
        {
            UpdateDerived();
        }
    }

    private void UpdateDerived()
    {
        Duracao = DataFim.Date - DataInicio.Date;
        DuracaoDias = Math.Max(1, (int)Duracao.TotalDays + 1);
        CustoTotal = CustoPorParticipante * NumeroParticipantes;

        OnPropertyChanged(nameof(Duracao));
        OnPropertyChanged(nameof(DuracaoDias));
        OnPropertyChanged(nameof(CustoTotal));
    }

    private async Task OnEditAsync()
    {
        if (EditRequested != null) await EditRequested.Invoke();
    }

    private async Task OnCompartilharAsync()
    {
        if (CompartilharRequested != null) await CompartilharRequested.Invoke();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}


public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;
    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }
    public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
    public void Execute(object? parameter) => _execute();
    public event EventHandler? CanExecuteChanged;
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

public class AsyncCommand : ICommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool>? _canExecute;
    private bool _isExecuting;
    public AsyncCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => !_isExecuting && (_canExecute?.Invoke() ?? true);

    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter)) return;
        try
        {
            _isExecuting = true;
            RaiseCanExecuteChanged();
            await _execute();
        }
        finally
        {
            _isExecuting = false;
            RaiseCanExecuteChanged();
        }
    }

    public event EventHandler? CanExecuteChanged;
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}