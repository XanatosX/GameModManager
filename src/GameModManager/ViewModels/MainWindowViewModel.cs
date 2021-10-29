using GameModManager.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using Avalonia.Threading;
using DynamicData.Binding;

namespace GameModManager.ViewModels
{
    /// <summary>
    /// View model for the main window
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// private accessor for the current search text
        /// </summary>
        private string searchText;

        /// <summary>
        /// The current search text
        /// </summary>
        public string SearchText
        {
            get => searchText;
            set => this.RaiseAndSetIfChanged(ref searchText, value);
        }

        /// <summary>
        /// Private accesstor if the current collection is empty
        /// </summary>
        private bool isCollectionEmpty;

        /// <summary>
        /// Is the current collection empty
        /// </summary>
        public bool IsCollectionEmpty
        {
            get => isCollectionEmpty;
            set
            {
                this.RaiseAndSetIfChanged(ref isCollectionEmpty, value);
                this.RaisePropertyChanged("CollectionFilled");
             }
        }

        /// <summary>
        /// Collection with all the game configurations
        /// </summary>
        public ReadOnlyObservableCollection<GameViewModel> Games => games;

        /// <summary>
        /// Private collection with all the games
        /// </summary>
        private readonly ReadOnlyObservableCollection<GameViewModel> games;

        /// <summary>
        /// Private accessor for all the available games
        /// </summary>
        private readonly SourceList<GameViewModel> allAvailableGames;

        /// <summary>
        /// Command to add a new game
        /// </summary>
        public ICommand AddGameCommand { get; }

        /// <summary>
        /// Interaction to show a dialog
        /// </summary>
        public Interaction<AddGameViewModel, GameViewModel?> ShowDialog { get; }

        /// <summary>
        /// Interaction to delete a game
        /// </summary>
        public Interaction<YesNoViewModel, YesNoDialogResult> DeleteGame { get; }

        /// <summary>
        /// Create a new instance of this view model
        /// </summary>
        public MainWindowViewModel()
        {
            SearchText = string.Empty;
            allAvailableGames = new();

            ShowDialog = new Interaction<AddGameViewModel, GameViewModel?>();
            DeleteGame = new Interaction<YesNoViewModel, YesNoDialogResult>();

            AddGameCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                AddGameViewModel addGameView = new AddGameViewModel();
                GameViewModel gameView = await ShowDialog.Handle(addGameView);

                if (gameView == null || !gameView.GameSet)
                {
                    return;
                }
                ActivateNewModel(gameView);
                allAvailableGames.Add(gameView);
            });

            this.WhenAnyValue(x => x.allAvailableGames.Count)
                .Subscribe(x => IsCollectionEmpty = x == 0);

            IObservable<Func<GameViewModel, bool>> filter = this.WhenAnyValue(x => x.SearchText)
                                                                .Throttle(TimeSpan.FromMilliseconds(400))
                                                                .ObserveOn(RxApp.MainThreadScheduler)
                                                                .Select(BuildFilter);

            allAvailableGames.Connect()
                             .Filter(filter)
                             .Sort(SortExpressionComparer<GameViewModel>.Ascending(g => g.Game.Name))
                             .ObserveOn(AvaloniaScheduler.Instance)
                             .Bind(out games)
                             .AutoRefreshOnObservable(x => x.RemoveEntry)
                             .Select(_ => this.allAvailableGames.Items.SingleOrDefault(x => !x.IsActive))
                             .Subscribe(async x =>
                             {
                                 if (x != null)
                                 {
                                     YesNoDialogResult result = await DeleteGame.Handle(new YesNoViewModel("Delete Game", string.Format("Do you want to delete the game ", x.Game.Name)));
                                     if (result != null && result.Accepted)
                                     {
                                         allAvailableGames.Remove(x);
                                     }
                                     if (result == null || !result.Accepted)
                                     {
                                         x.IsActive = true;
                                     }
                                 }
                                 this.RaisePropertyChanged("allAvailableGames");
                             });
            LoadCovers();
        }

        /// <summary>
        /// Method to build the search text filter
        /// </summary>
        /// <param name="text">The text to search for</param>
        /// <returns>A func which can be used for filtering</returns>
        private Func<GameViewModel, bool> BuildFilter(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return t => true;
            }
            return t => t.Game.Name.Contains(text, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Load all the covers async
        /// </summary>
        private async void LoadCovers()
        {
            foreach (GameViewModel model in allAvailableGames.Items)
            {
                ActivateNewModel(model);
            }
        }

        /// <summary>
        /// Activate the newly added model
        /// </summary>
        /// <param name="model"></param>
        private void ActivateNewModel(GameViewModel model)
        {
            model.ShowEditDialog.RegisterHandler(DoEditGameEntry);
            model.LoadGameImage();
        }

        /// <summary>
        /// Edit the game entry
        /// </summary>
        /// <param name="interactionContext">The interaction context</param>
        /// <returns>A awaitable task</returns>
        private async Task DoEditGameEntry(InteractionContext<AddGameViewModel, GameViewModel?> interactionContext)
        {
            GameViewModel gameView = await ShowDialog.Handle(interactionContext.Input);
            if (gameView == null || !gameView.GameSet)
            {
                interactionContext.SetOutput(null);
                return;
            }
            interactionContext.SetOutput(gameView);
        }
    }
}
