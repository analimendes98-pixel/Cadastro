using System;
using Microsoft.Maui.Controls;
using Cadastro.Models;
using Cadastro.Views;

namespace Cadastro
{
    public partial class MainPage : ContentPage
    {
        private readonly Evento _evento = new();

        public MainPage()
        {
            InitializeComponent();
            BindingContext = _evento;
        }

        private async void onConcluirCadastroClicked(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new ResumoEventoPage(_evento));
        }
    }
}






