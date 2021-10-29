using Avalonia.Controls;
using Avalonia.Media.Imaging;
using DynamicData;
using GameModManager.Models;
using GameModManager.Services.DataProviders.Loaders;
using GameModManager.Services.DataProviders.Loaders.Mod;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GameModManager.ViewModels
{
    /// <summary>
    /// Model view class to add a new game
    /// </summary>
    public class AddGameViewModel : ViewModelBase, IDisposable
    {
        /// <summary>
        /// Namespace to search for data providers
        /// </summary>
        private const string LOADER_NAMESPACE = "GameModManager.Services.DataProviders.ModLoader";

        /// <summary>
        /// Tooltip text to use if url is not reachable
        /// </summary>
        private const string URL_NOT_REACHABLE = "The url is not reachable!";

        /// <summary>
        /// Tooltip text to use if url is reachable
        /// </summary>
        private const string URL_NOT_VALID_FOR_PROVIDER = "The given url is not valid for the selected provider";

        /// <summary>
        /// private bitmap of the game cover
        /// </summary>
        private Bitmap? gameCover;

        /// <summary>
        /// Bitmap of the game cover
        /// </summary>
        public Bitmap? GameCover
        {
            get => gameCover;
            private set => this.RaiseAndSetIfChanged(ref gameCover, value);
        }

        /// <summary>
        /// private is there a game cover to show
        /// </summary>
        private bool coverExists;

        /// <summary>
        /// Is there a game cover to show
        /// </summary>
        public bool CoverExists
        {
            get => coverExists;
            private set
            {
                this.RaiseAndSetIfChanged(ref coverExists, value);
                this.RaisePropertyChanged("CoverMissing");
            }
        }

        /// <summary>
        /// All the providers selectable by the user
        /// </summary>
        public ObservableCollection<ModProvider> Providers { get; }

        /// <summary>
        /// Currently selected provider
        /// </summary>
        public ModProvider SelectedProvider
        {
            get => selectedProvider;
            set
            {
                this.RaiseAndSetIfChanged(ref selectedProvider, value);
                this.RaisePropertyChanged("ProviderExampleText");
            }
        }

        /// <summary>
        /// Private currently selected provider
        /// </summary>
        private ModProvider selectedProvider;

        /// <summary>
        /// Text for the currently selected url
        /// </summary>
        public string UrlErrorText
        {
            get => urlErrorText;
            set => this.RaiseAndSetIfChanged(ref urlErrorText, value);
        }

        /// <summary>
        /// Private text for the currently selected url
        /// </summary>
        private string urlErrorText;

        /// <summary>
        /// Example text for the provider
        /// </summary>
        public string ProviderExampleText
        {
            get => providerExampleText;
            set => this.RaiseAndSetIfChanged(ref providerExampleText, value);
        }

        /// <summary>
        /// Private provider example text
        /// </summary>
        private string providerExampleText;

        /// <summary>
        /// The selected game exe for image extraction
        /// </summary>
        public string GameExe
        {
            get => gameExe;
            set => this.RaiseAndSetIfChanged(ref gameExe, value);
        }

        /// <summary>
        /// Private select game exe for image extraction
        /// </summary>
        private string gameExe;

        /// <summary>
        /// Url to the project to get the newest data from
        /// </summary>
        public string ProjectUrl
        {
            get => projectUrl;
            set => this.RaiseAndSetIfChanged(ref projectUrl, value);
        }
        /// <summary>
        /// Private url to the project
        /// </summary>
        private string projectUrl;

        /// <summary>
        /// The name of the game to display
        /// </summary>
        public string GameDisplayName
        {
            get => gameDisplayName;
            set => this.RaiseAndSetIfChanged(ref gameDisplayName, value);
        }

        /// <summary>
        /// Private name of the game to display
        /// </summary>
        private string gameDisplayName;

        /// <summary>
        /// The target folder to load the data to
        /// </summary>
        public string TargetFolder
        {
            get => targetFolder;
            set => this.RaiseAndSetIfChanged(ref targetFolder, value);
        }

        /// <summary>
        /// Private target folder to load the data to
        /// </summary>
        private string targetFolder;

        /// <summary>
        /// Is the given url valid
        /// </summary>
        public bool UrlValid
        {
            get => urlValid;
            set => this.RaiseAndSetIfChanged(ref urlValid, value);
        }

        /// <summary>
        /// Private accessor of the is the url valid field
        /// </summary>
        private bool urlValid;

        /// <summary>
        /// Token to cancel image loading
        /// </summary>
        private CancellationTokenSource? cancellationTokenSource;

        /// <summary>
        /// Command to save the currently created game
        /// </summary>
        public ReactiveCommand<Unit, GameViewModel> SaveGame { get; }

        /// <summary>
        /// Command to abort adding the current game
        /// </summary>
        public ReactiveCommand<Unit, GameViewModel> AbortAdding { get; }

        /// <summary>
        /// Command to open a file
        /// </summary>
        public ICommand OpenFileCommand { get; }

        /// <summary>
        /// Command to open a folder
        /// </summary>
        public ICommand OpenFolderCommand { get; }

        /// <summary>
        /// Interaction to open a file
        /// </summary>
        public Interaction<List<FileDialogFilter>, string> OpenFileInteraction { get; }

        /// <summary>
        /// Interaction to open a folder
        /// </summary>
        public Interaction<string, string> OpenFolderInteraction { get; }

        /// <summary>
        /// Create a new instance of this class
        /// </summary>
        public AddGameViewModel()
        {
            GameExe = string.Empty;
            ProjectUrl = string.Empty;
            GameDisplayName = string.Empty;
            TargetFolder = string.Empty;

            Providers = new ObservableCollection<ModProvider>();
            IDataLoader<IReadOnlyList<ModProvider>> modProviderLoader = new ModLoaderProvider();
            Task<IReadOnlyList<ModProvider>> modLoadingTask = modProviderLoader.LoadDataAsync(LOADER_NAMESPACE);
            modLoadingTask.ContinueWith(t => {
                Providers.Clear();
                foreach(ModProvider provider in t.Result)
                {
                    Providers.Add(provider);
                }
            });

            var canExecute = this.WhenAnyValue(
                x => x.GameDisplayName,
                x => x.TargetFolder,
                x => x.ProjectUrl,
                x => x.SelectedProvider,
                x => x.UrlValid,
                (name, folder, url, provider, reachable) =>
                {
                    bool valid = reachable;
                    valid &= !string.IsNullOrEmpty(name);
                    valid &= !string.IsNullOrEmpty(folder);
                    valid &= Directory.Exists(folder);
                    valid &= !string.IsNullOrEmpty(url);
                    valid &= provider != null;
                    if (provider != null)
                    {
                        bool urlIsValid = provider.GetClassInstance().CheckUrlIsValid(url);
                        if (!urlIsValid)
                        {
                            cancellationTokenSource?.Cancel();
                            UrlErrorText = URL_NOT_VALID_FOR_PROVIDER;
                        }
                        UrlValid &= urlIsValid;
                        valid &= urlIsValid;
                    }
                    return valid;
                }
                );

            SaveGame = ReactiveCommand.Create<Unit, GameViewModel>(data =>
            {
                Game game = new Game(
                    GameExe,
                    GameDisplayName,
                    RemoveDirectoryEnding(TargetFolder),
                    ProjectUrl,
                    SelectedProvider
                    );
                return new GameViewModel(game);
            }, canExecute);

            AbortAdding = ReactiveCommand.Create(() =>
            {
                return new GameViewModel(null);
            });

            OpenFileInteraction = new Interaction<List<FileDialogFilter>, string>();
            OpenFileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                string selectedFile = await OpenFileInteraction.Handle(new List<FileDialogFilter>());
                if (string.IsNullOrEmpty(selectedFile))
                {
                    return;
                }
                GameExe = selectedFile;
            });
            OpenFolderInteraction = new Interaction<string, string>();
            OpenFolderCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                string selectedFolder = await OpenFolderInteraction.Handle("");
                if (string.IsNullOrEmpty(selectedFolder))
                {
                    return;
                }
                TargetFolder = selectedFolder;
            });

            this.WhenAnyValue(
                            x => x.ProjectUrl)
                            .Subscribe(async (data) => {
                                cancellationTokenSource?.Cancel();
                                cancellationTokenSource = new CancellationTokenSource();

                                if (!cancellationTokenSource.IsCancellationRequested)
                                {
                                    UrlValid = await CheckUrlReachable(cancellationTokenSource.Token, data).ConfigureAwait(false);
                                }

                            });

            this.WhenAnyValue(x => x.GameExe)
                .Throttle(TimeSpan.FromMilliseconds(400))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => LoadGameCover(x));

            this.WhenAnyValue(x => x.GameDisplayName)
                .Throttle(TimeSpan.FromMilliseconds(400))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => GameDisplayName = x.Trim());

            this.WhenAnyValue(x => x.TargetFolder)
                .Throttle(TimeSpan.FromMilliseconds(400))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => TargetFolder = RemoveDirectoryEnding(x));
                
            this.WhenAnyValue(x => x.GameCover).Subscribe(x => CoverExists = x != null);

            this.WhenAnyValue(x => x.SelectedProvider).Subscribe(provider => ProviderExampleText = provider != null ? provider.GetExampleText() : String.Empty);
        }

        /// <summary>
        /// Create a new instance of this class with a already created game (Edit mode)
        /// </summary>
        /// <param name="game">The game to edit</param>
        public AddGameViewModel(Game game)
            : this()
        {
            GameExe = game.GameExe;
            ProjectUrl = game.RemoteUrl;
            TargetFolder = game.TargetFolderPath;
            GameDisplayName = game.Name;
            if (Providers != null && game.DataProviderToUse != null)
            {
                SelectedProvider = Providers.Where(prov => prov.GetType() == game.DataProviderToUse.GetType()).FirstOrDefault();
            }
        }

        /// <summary>
        /// Check async if the url is reachable
        /// </summary>
        /// <param name="token">The cancelation token</param>
        /// <param name="url">The url to check</param>
        /// <returns>True if the url is valid</returns>
        private async Task<bool> CheckUrlReachable(CancellationToken token, string url)
        {
            return await Task.Run(() =>
            {
                HttpWebRequest request = default;
                try
                {
                    request = (HttpWebRequest)WebRequest.Create(url);
                }
                catch (Exception)
                {
                    return false;
                }
                request.Timeout = 5000;
                request.Method = "HEAD";
                try
                {
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (!token.IsCancellationRequested && response.StatusCode == HttpStatusCode.OK)
                        {
                            UrlErrorText = URL_NOT_REACHABLE;
                            return true;
                        }
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }

                
            });
        }

        /// <summary>
        /// Async load the game cover
        /// </summary>
        /// <param name="exePath">The path to the exe</param>
        /// <returns>A task you can await</returns>
        private async Task LoadGameCover(string exePath)
        {
            if (!File.Exists(exePath))
            {
                GameCover = null;
                return;
            }

            System.Drawing.Icon loadedIcon = System.Drawing.Icon.ExtractAssociatedIcon(exePath);
            using (MemoryStream mStream = new MemoryStream())
            {
                loadedIcon.ToBitmap().Save(mStream, ImageFormat.Png);
                mStream.Position = 0;
                GameCover = Bitmap.DecodeToWidth(mStream, 32);
            }
        }

        /// <summary>
        /// Remove the last char of a directory if this is a separator
        /// </summary>
        /// <param name="input">The path to edit</param>
        /// <returns>The edited path</returns>
        private string RemoveDirectoryEnding(string input)
        {
            if (input.EndsWith(Path.DirectorySeparatorChar))
            {   
                return RemoveDirectoryEnding(input.Remove(input.Length - 1));
            }
            return input;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            cancellationTokenSource?.Dispose();
        }
    }
}
