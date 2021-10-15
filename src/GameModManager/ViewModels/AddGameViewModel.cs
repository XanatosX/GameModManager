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
    public class AddGameViewModel : ViewModelBase, IDisposable
    {
        private const string LOADER_NAMESPACE = "GameModManager.Services.DataProviders.ModLoader";
        private const string URL_NOT_REACHABLE = "The url is not reachable!";
        private const string URL_NOT_VALID_FOR_PROVIDER = "The given url is not valid for the selected provider";

        private Bitmap? gameCover;
        public Bitmap? GameCover
        {
            get => gameCover;
            private set => this.RaiseAndSetIfChanged(ref gameCover, value);
        }

        private bool coverExists;
        public bool CoverExists
        {
            get => coverExists;
            private set
            {
                this.RaiseAndSetIfChanged(ref coverExists, value);
                this.RaisePropertyChanged("CoverMissing");
            }
        }
        public bool CoverMissing => !coverExists;

        public ObservableCollection<ModProvider> Providers { get; }

        public ModProvider SelectedProvider
        {
            get => selectedProvider;
            set
            {
                this.RaiseAndSetIfChanged(ref selectedProvider, value);
                this.RaisePropertyChanged("ProviderExampleText");
            }
        }
        private ModProvider selectedProvider;
        public string UrlErrorText
        {
            get => urlErrorText;
            set => this.RaiseAndSetIfChanged(ref urlErrorText, value);
        }

        public string ProviderExampleText => SelectedProvider == null ? string.Empty : SelectedProvider.GetExampleText();

        public string GameExe
        {
            get => gameExe;
            set => this.RaiseAndSetIfChanged(ref gameExe, value);
        }
        private string gameExe;

        public string ProjectUrl
        {
            get => projectUrl;
            set => this.RaiseAndSetIfChanged(ref projectUrl, value);
        }
        private string projectUrl;

        public string GameDisplayName
        {
            get => gameDisplayName;
            set => this.RaiseAndSetIfChanged(ref gameDisplayName, value);
        }

        private string gameDisplayName;

        public string TargetFolder
        {
            get => targetFolder;
            set => this.RaiseAndSetIfChanged(ref targetFolder, value);
        }

        private string targetFolder;

        public bool UrlValid
        {
            get => urlValid;
            set => this.RaiseAndSetIfChanged(ref urlValid, value);
        }

        private bool urlValid;
        private CancellationTokenSource? cancellationTokenSource;
        private string urlErrorText;

        public ReactiveCommand<Unit, GameViewModel> SaveGame { get; }
        public ReactiveCommand<Unit, GameViewModel> AbortAdding { get; }

        public ICommand OpenFileCommand { get; }

        public ICommand OpenFolderCommand { get; }

        public Interaction<List<FileDialogFilter>, string> OpenFileInteraction { get; }
        public Interaction<string, string> OpenFolderInteraction { get; }

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
        }

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

        private string RemoveDirectoryEnding(string input)
        {
            if (input.EndsWith(Path.DirectorySeparatorChar))
            {   
                return RemoveDirectoryEnding(input.Remove(input.Length - 1));
            }
            return input;
        }

        public void Dispose()
        {
            cancellationTokenSource?.Dispose();
        }
    }
}
