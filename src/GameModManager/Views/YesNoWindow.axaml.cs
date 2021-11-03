using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using GameModManager.ViewModels;
using ReactiveUI;
using System;

namespace GameModManager.Views
{
    public partial class YesNoWindow : ReactiveWindow<YesNoViewModel>
    {
        public YesNoWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif


            this.WhenActivated(d => d(ViewModel!.YesCommand.Subscribe(Close)));
            this.WhenActivated(d => d(ViewModel!.NoCommand.Subscribe(Close)));
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
