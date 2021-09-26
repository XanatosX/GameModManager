using GameModManager.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GameModManager.ViewModels
{
    public class YesNoViewModel : ViewModelBase
    {
        public string Title { get; }
        public string Text { get; }

        public ReactiveCommand<Unit, YesNoDialogResult> YesCommand { get; }
        public ReactiveCommand<Unit, YesNoDialogResult> NoCommand { get; }

        public YesNoViewModel(string title, string text)
        {
            Title = title;
            Text = text;

            YesCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                return new YesNoDialogResult(true);
            });

            NoCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                return new YesNoDialogResult(false);
            });
        }
    }
}
