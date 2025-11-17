using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Cadastro.Models;

public partial class Evento : INotifyPropertyChanged
{
    private string _nome = string.Empty;
    private DateTime _dataInicio = DateTime.Today;
    private DateTime _dataFim = DateTime.Today;
    private int _numeroParticipantes;
    private string _local = string.Empty;
    private decimal _custoPorParticipante;

    public string Nome
    {
        get => _nome;
        set
        {
            if (value == _nome) return;
            _nome = value ?? string.Empty;
            OnPropertyChanged();
        }
    }

    public DateTime DataInicio
    {
        get => _dataInicio;
        set
        {
            var newDate = value.Date;
            if (newDate == _dataInicio) return;

            _dataInicio = newDate;

            if (_dataFim < _dataInicio)
            {
                _dataFim = _dataInicio;
                OnPropertyChanged(nameof(DataFim));
            }

            OnPropertyChanged();
            OnPropertyChanged(nameof(Duracao));
            OnPropertyChanged(nameof(DuracaoDias));
        }
    }

    public DateTime DataFim
    {
        get => _dataFim;
        set
        {
            var newDate = value.Date;
            if (newDate == _dataFim) return;

            _dataFim = newDate;

            if (_dataInicio > _dataFim)
            {
                _dataInicio = _dataFim;
                OnPropertyChanged(nameof(DataInicio));
            }

            OnPropertyChanged();
            OnPropertyChanged(nameof(Duracao));
            OnPropertyChanged(nameof(DuracaoDias));
        }
    }

    public int NumeroParticipantes
    {
        get => _numeroParticipantes;
        set
        {
            var normalized = Math.Max(0, value);
            if (normalized == _numeroParticipantes) return;

            _numeroParticipantes = normalized;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CustoTotal));
        }
    }

    public string Local
    {
        get => _local;
        set
        {
            if (value == _local) return;
            _local = value ?? string.Empty;
            OnPropertyChanged();
        }
    }

    public decimal CustoPorParticipante
    {
        get => _custoPorParticipante;
        set
        {
            var normalized = value < 0 ? 0m : value;
            if (normalized == _custoPorParticipante) return;

            _custoPorParticipante = normalized;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CustoTotal));
        }
    }

    public TimeSpan Duracao => DataFim.Date - DataInicio.Date;

    public int DuracaoDias => (DataFim.Date - DataInicio.Date).Days + 1;

    public decimal CustoTotal => NumeroParticipantes * CustoPorParticipante;

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}