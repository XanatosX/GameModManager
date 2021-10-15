using GameModManager.Services.Container;
using GameModManager.Services.DataProviders.Loaders;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace GameModManager.Services.DataProviders.Savers
{
    /// <summary>
    /// Class to save and load artifacts from cache
    /// </summary>
    public class ArtifactProvider : IDataSaver<ReleaseArtifact>, IDataLoader<ReleaseArtifact>
    {
        private const string README_NAME = "readme.md";
        private const string MANIFEST_NAME = "manifest";
        private const string ARTIFACT_NAME = "artifact";
        private const string README_TEXT_RESOURCE = "GameModManager.Resources.ReleaseArtifactReadmeText.md";

        /// <inheritdoc/>
        public Task<ReleaseArtifact> LoadDataAsync(string dataSource)
        {
            return Task.Run(() => LoadData(dataSource));
        }

        /// <inheritdoc/>
        public ReleaseArtifact LoadData(string dataSource)
        {
            ReleaseArtifact returnArtifact = null;
            if (!File.Exists(dataSource))
            {
                return returnArtifact;
            }

            try
            {
                using (FileStream reader = new FileStream(dataSource, FileMode.Open))
                {
                    using (ZipArchive zipArchive = new ZipArchive(reader, ZipArchiveMode.Read))
                    {
                        ZipArchiveEntry? manifest = zipArchive.Entries
                                                              .FirstOrDefault(entry => entry.Name == MANIFEST_NAME);
                        ZipArchiveEntry? artifact = zipArchive.Entries
                                                              .FirstOrDefault(entry => entry.Name == ARTIFACT_NAME);

                        if (artifact == null || manifest == null)
                        {
                            return returnArtifact;
                        }

                        ZipManifest manifestData = JsonSerializer.Deserialize<ZipManifest>(ReadZipArchiveEntry(manifest));
                        string localFile = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());

                        artifact.ExtractToFile(localFile, true);
                        returnArtifact = new ReleaseArtifact(new Version(manifestData.Version), localFile, manifestData.Checksum);
                    }
                }

                return returnArtifact;
            }
            catch (Exception e)
            {
                return returnArtifact;
            }
        }

        /// <summary>
        /// Read a zip archive entry as a string
        /// </summary>
        /// <param name="entry">The entry to read</param>
        /// <returns>The string read from the entry or an empty one if there was an error</returns>
        private string ReadZipArchiveEntry(ZipArchiveEntry? entry)
        {
            string returnString = string.Empty;
            if (entry == null)
            {
                return returnString;
            }
            using (Stream stream = entry.Open())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    returnString = reader.ReadToEnd();
                }
            }

            return returnString;
        }

        /// <inheritdoc/>
        public Task<bool> SaveDataAsync(ReleaseArtifact data, string connectionString)
        {
            return Task.Run(() => SaveData(data, connectionString));
        }

        /// <inheritdoc/>
        public bool SaveData(ReleaseArtifact data, string connectionString)
        {
            try
            {
                if (File.Exists(connectionString))
                {
                    File.Delete(connectionString);
                }
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        ZipArchiveEntry manifest = archive.CreateEntry(MANIFEST_NAME);
                        using (StreamWriter writer = new StreamWriter(manifest.Open()))
                        {
                            writer.Write(JsonSerializer.Serialize(new ZipManifest(data)));
                        }
                        ZipArchiveEntry artifact = archive.CreateEntry(ARTIFACT_NAME);
                        using (StreamWriter writer = new StreamWriter(artifact.Open()))
                        {
                            byte[] bytes = File.ReadAllBytes(data.ArtifactFile);
                            writer.BaseStream.Write(bytes, 0, bytes.Length);
                        }

                        ZipArchiveEntry readme = archive.CreateEntry(README_NAME);
                        using (StreamWriter writer = new StreamWriter(readme.Open()))
                        {
                            writer.Write(LoadReadmeContent());
                        }
                    }

                    using (FileStream fileStream = new FileStream(connectionString, FileMode.CreateNew))
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        memoryStream.CopyTo(fileStream);
                    }
                }
            }
            catch (Exception)
            {
                if (File.Exists(connectionString))
                {
                    File.Delete(connectionString);
                }

                return false;
            }

            return true;

        }

        /// <summary>
        /// Load the data of the readme file
        /// </summary>
        /// <returns>The string ready to write to the readme file</returns>
        private string LoadReadmeContent()
        {
            string returnData = string.Empty;
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(README_TEXT_RESOURCE))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    returnData = reader.ReadToEnd();
                }
            }
            returnData = returnData.Replace("{DATE}", DateTime.Now.ToString("g"));
            return returnData;
        }

        /// <summary>
        /// Internal struct to save as manifest into the file
        /// </summary>
        internal struct ZipManifest
        {
            /// <summary>
            /// The version of this artifact
            /// </summary>
            public string Version { get; set; }

            /// <summary>
            /// The checksum of this artifact
            /// </summary>
            public string Checksum { get; set; }     
            
            /// <summary>
            /// The initial fiel ending of the artifact
            /// </summary>
            public string ArtifactFileEnding { get; set; }

            /// <summary>
            /// The initial fiel ending of the artifact
            /// </summary>
            public string ArtifactInitalName { get; set; }

            /// <summary>
            /// Create a new instance of this class
            /// </summary>
            /// <param name="artifact">The artifact to create the manifest from</param>
            public ZipManifest(ReleaseArtifact artifact)
            {
                Checksum = artifact.Checksum;
                Version = artifact.Version.ToString();
                FileInfo info = new FileInfo(artifact.ArtifactFile);
                ArtifactFileEnding = info.Extension;
                ArtifactInitalName = info.Name.Replace(ArtifactFileEnding, string.Empty);
            }
        }
    }
}
