using GameModManager.Models;
using GameModManager.Services.DataProviders.ModLoader;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using Avalonia.Threading;
using DynamicData.Binding;

namespace GameModManager.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string searchText;
        public string SearchText
        {
            get => searchText;
            set => this.RaiseAndSetIfChanged(ref searchText, value);
        }

        private bool collectionEmpty;
        public bool CollectionEmpty
        {
            get => collectionEmpty;
            set
            {
                this.RaiseAndSetIfChanged(ref collectionEmpty, value);
                this.RaisePropertyChanged("CollectionFilled");
             }
        }

        public bool CollectionFilled
        {
            get => !collectionEmpty;
        }

        public ReadOnlyObservableCollection<GameViewModel> Games => games;
        private ReadOnlyObservableCollection<GameViewModel> games;
        private readonly SourceList<GameViewModel> allAvailableGames;

        public ICommand AddGameCommand { get; }

        public Interaction<AddGameViewModel, GameViewModel?> ShowDialog { get; }

        public Interaction<YesNoViewModel, YesNoDialogResult> DeleteGame { get; }

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
                .Subscribe(x => CollectionEmpty = x == 0);

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
                                     if (result != null && result.accepted)
                                     {
                                         allAvailableGames.Remove(x);
                                     }
                                     if (result == null || !result.accepted)
                                     {
                                         x.IsActive = true;
                                     }
                                 }
                                 this.RaisePropertyChanged("allAvailableGames");
                             });
                             
            /**
            allAvailableGames.Add(new GameViewModel(
                new Game(
                    @"C:\Program Files (x86)\Steam\steamapps\common\Carrier Command 2\carrier_command.exe",
                    "Carrier Command 2",
                    @"C:\Program Files (x86)\Steam\steamapps\common\Carrier Command 2\rom_0\scripts",
                    "https://github.com/AvaloniaUI/Avalonia",
                    new ModProvider(typeof(GithubProvider))
                    )
                ));
            **/


            LoadCovers();
        }

        private Func<GameViewModel, bool> BuildFilter(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return t => true;
            }
            return t => t.Game.Name.Contains(text, StringComparison.OrdinalIgnoreCase);
        }

        private async void LoadCovers()
        {
            foreach (GameViewModel model in allAvailableGames.Items)
            {
                ActivateNewModel(model);
            }
        }

        private void ActivateNewModel(GameViewModel model)
        {
            model.ShowEditDialog.RegisterHandler(DoEditGameEntry);
            model.LoadGameImage();
        }

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
