using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using GameModManager.Models;
using GameModManager.ViewModels;
using ReactiveUI;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GameModManager.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.WhenActivated(d => d(ViewModel!.ShowDialog.RegisterHandler(DoShowDialogAsync)));
            this.WhenActivated(d => d(ViewModel!.DeleteGame.RegisterHandler(DoShowYesNoDialog)));
            
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async Task DoShowDialogAsync(InteractionContext<AddGameViewModel, GameViewModel?> interaction)
        {
            AddGameWindow gameWindow = new AddGameWindow();
            gameWindow.DataContext = interaction.Input;
            GameViewModel result = await gameWindow.ShowDialog<GameViewModel>(this);
            interaction.SetOutput(result);
        }

        private async Task DoShowYesNoDialog(InteractionContext<YesNoViewModel, YesNoDialogResult> interaction)
        {
            YesNoWindow yesNo = new YesNoWindow();
            yesNo.DataContext = interaction.Input;
            YesNoDialogResult result = await yesNo.ShowDialog<YesNoDialogResult>(this);
            result = result ?? new YesNoDialogResult(false);
            interaction.SetOutput(result);
        }
    }
}
