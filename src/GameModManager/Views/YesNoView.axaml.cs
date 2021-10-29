using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GameModManager.Views
{
    public partial class YesNoView : UserControl
    {
        public YesNoView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
