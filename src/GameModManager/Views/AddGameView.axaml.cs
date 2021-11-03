using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GameModManager.Views
{
    public partial class AddGameView : UserControl
    {
        public AddGameView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
