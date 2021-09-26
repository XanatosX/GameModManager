using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using GameModManager.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;

namespace GameModManager.Views
{
    public partial class AddGameWindow : ReactiveWindow<AddGameViewModel>
    {
        public AddGameWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            this.WhenActivated(d => d(ViewModel!.SaveGame.Subscribe(Close)));
            this.WhenActivated(d => d(ViewModel!.AbortAdding.Subscribe(Close)));
            this.WhenActivated(d => d(ViewModel!.OpenFileInteraction.RegisterHandler(DoOpenFileInteraction)));
            this.WhenActivated(d => d(ViewModel!.OpenFolderInteraction.RegisterHandler(DoOpenFolderInteraction)));
        }

        private async Task DoOpenFileInteraction(InteractionContext<List<FileDialogFilter>, string> context)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filters = context.Input
            };
            string[] files = await openFileDialog.ShowAsync(this);
            context.SetOutput(files.Length > 0 ? files[0] : string.Empty);
        }

        private async Task DoOpenFolderInteraction(InteractionContext<string, string> context)
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog()
            {
                Directory = context.Input
            };
            string folder = await openFolderDialog.ShowAsync(this);
            context.SetOutput(folder);
            
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
