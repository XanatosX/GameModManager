using GameModManager.Models;
using ReactiveUI;
using System.Reactive;

namespace GameModManager.ViewModels
{
    /// <summary>
    /// Model view for the yes no dialog
    /// </summary>
    public class YesNoViewModel : ViewModelBase
    {
        /// <summary>
        /// The title for the dialog
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// The dialog text
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Command for the yes button
        /// </summary>
        public ReactiveCommand<Unit, YesNoDialogResult> YesCommand { get; }

        /// <summary>
        /// Command for the no button
        /// </summary>
        public ReactiveCommand<Unit, YesNoDialogResult> NoCommand { get; }

        /// <summary>
        /// Create a new instance of this class
        /// </summary>
        /// <param name="title">The title for the dialog</param>
        /// <param name="text">The text for the dialog</param>
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
