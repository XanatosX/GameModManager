using System;

namespace GameModManager.Services.Container
{
    public class ReleaseArtifact
    {
        public Version Version { get; }

        public string ArtifactFile { get; }

        public string Checksum { get; }

        public ReleaseArtifact(Version version, string artifactFile, string checksum)
        {
            Version = version;
            ArtifactFile = artifactFile;
            Checksum = checksum;
        }
    }
}
