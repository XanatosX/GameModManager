using GameModManager.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using Avalonia.Media.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Reactive;
using System.Reactive.Linq;

namespace GameModManager.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        public Game Game
        {
            get => game;
            set => this.RaiseAndSetIfChanged(ref game, value);
        }
        private Game game;

        public bool IsActive
        {
            get => isActive;
            set => this.RaiseAndSetIfChanged(ref isActive, value);
        }

        private bool isActive;

        public bool GameSet
        {
            get => gameSet;
            set => this.RaiseAndSetIfChanged(ref gameSet, value);
        }
        private bool gameSet;

        private Bitmap cover;

        public Bitmap Cover
        {
            get => cover;
            set => this.RaiseAndSetIfChanged(ref cover, value);
        }

        public ReactiveCommand<Unit, Unit> RemoveEntry { get; }

        public ICommand EditEntry { get; private set; }

        public ICommand OpenUrlCommand { get; }
        public Interaction<GameViewModel, bool> DeleteEntry { get; }

        public Interaction<AddGameViewModel, GameViewModel?> ShowEditDialog { get; }

        public GameViewModel(Game game)
        {
            this.game = game;
            IsActive = true;

            RemoveEntry = ReactiveCommand.CreateFromTask(async () =>
            {
                this.IsActive = false;
            });

            ShowEditDialog = new Interaction<AddGameViewModel, GameViewModel?>();
            OpenUrlCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await game.OpenLinkInBrowser();
            });
            EditEntry = ReactiveCommand.CreateFromTask(async () =>
            {
                Game test = this.Game;
                AddGameViewModel model = new AddGameViewModel(this.Game);
                GameViewModel gameView = await ShowEditDialog.Handle(model);

                if (gameView == null || !gameView.GameSet)
                {
                    return;
                }

                Game = gameView.Game;
                LoadGameImage();
            });

            this.WhenAnyValue(x => x.Game).Subscribe(g => GameSet = g != null);

            
        }

        public async Task LoadGameImage()
        {
            await using(Stream imageStream = await game.LoadGameImage())
            {
                if (!imageStream.CanRead)
                {
                    Cover = null;
                    return;
                }
                Cover = await Task.Run(() => Bitmap.DecodeToWidth(imageStream, 20));
            }
        }
    }
}
