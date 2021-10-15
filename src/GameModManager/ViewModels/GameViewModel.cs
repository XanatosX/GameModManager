using System;
using GameModManager.Models;
using ReactiveUI;
using Avalonia.Media.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Reactive;
using System.Reactive.Linq;

namespace GameModManager.ViewModels
{
    /// <summary>
    /// View model for the game class
    /// </summary>
    public class GameViewModel : ViewModelBase
    {
        /// <summary>
        /// The game instance which the model should show
        /// </summary>
        public Game Game
        {
            get => game;
            set => this.RaiseAndSetIfChanged(ref game, value);
        }

        /// <summary>
        /// Private accessor for the game instance the model should show
        /// </summary>
        private Game game;

        /// <summary>
        /// Is this game set to active, if not it should be deleted from the list
        /// </summary>
        public bool IsActive
        {
            get => isActive;
            set => this.RaiseAndSetIfChanged(ref isActive, value);
        }

        /// <summary>
        /// Private accessor if this game is set to active, if not it should be deleted from the list
        /// </summary>
        private bool isActive;

        /// <summary>
        /// Is the current game set
        /// </summary>
        public bool GameSet
        {
            get => gameSet;
            set => this.RaiseAndSetIfChanged(ref gameSet, value);
        }

        /// <summary>
        /// Private accessor if the game is set
        /// </summary>
        private bool gameSet;

        /// <summary>
        /// Private accesor for the game cover
        /// </summary>
        private Bitmap cover;

        /// <summary>
        /// The game cover to show
        /// </summary>
        public Bitmap Cover
        {
            get => cover;
            set => this.RaiseAndSetIfChanged(ref cover, value);
        }

        /// <summary>
        /// Command to remove the entry
        /// </summary>
        public ReactiveCommand<Unit, Unit> RemoveEntry { get; }

        /// <summary>
        /// Command to edit the entry
        /// </summary>
        public ICommand EditEntry { get; private set; }

        /// <summary>
        /// Command to open the url
        /// </summary>
        public ICommand OpenUrlCommand { get; }

        /// <summary>
        /// Interaction to delete the entry
        /// </summary>
        public Interaction<GameViewModel, bool> DeleteEntry { get; }

        /// <summary>
        /// Interaction to show the edit dialog for the entry
        /// </summary>
        public Interaction<AddGameViewModel, GameViewModel?> ShowEditDialog { get; }

        /// <summary>
        /// Create a new instance of this game view
        /// </summary>
        /// <param name="game">The game to show</param>
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

        /// <summary>
        /// Load the game image async
        /// </summary>
        /// <returns>A awaitable task without return value</returns>
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
