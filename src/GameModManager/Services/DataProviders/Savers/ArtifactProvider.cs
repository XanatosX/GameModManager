using GameModManager.Services.Container;
using GameModManager.Services.DataProviders.Loaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GameModManager.Services.DataProviders.Savers
{
    public class ArtifactProvider : IDataSaver<ReleaseArtifact>, IDataLoader<ReleaseArtifact>
    {
        private const string README_NAME = "readme.md";
        private const string MANIFEST_NAME = "manifest";
        private const string ARTIFACT_NAME = "artifact";
        private const string README_TEXT_RESOURCE = "GameModManager.Resources.ReleaseArtifactReadmeText.md";

        public Task<ReleaseArtifact> LoadDataAsync(string dataSource)
        {
            return Task.Run(() => LoadData(dataSource));
        }

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

                        ZipManifest manifestData = JsonSerializer.Deserialize<ZipManifest>(ReadZipArchiveENtry(manifest));
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

        private string ReadZipArchiveENtry(ZipArchiveEntry? manifest)
        {
            string returnString = string.Empty;
            if (manifest == null)
            {
                return returnString;
            }
            using (Stream stream = manifest.Open())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    returnString = reader.ReadToEnd();
                }
            }

            return returnString;
        }

        public Task<bool> SaveDataAsync(ReleaseArtifact data, string connectionString)
        {
            return Task.Run(() => SaveData(data, connectionString));
        }

        public bool SaveData(ReleaseArtifact data, string connectionString)
        {
            try
            {
                string test = LoadReadmeContent();
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

            return returnData;
        }

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
            /// Create a new instance of this class
            /// </summary>
            /// <param name="artifact">The artifact to create the manifest from</param>
            public ZipManifest(ReleaseArtifact artifact)
            {
                Checksum = artifact.Checksum;
                Version = artifact.Version.ToString();
            }
        }
    }
}
