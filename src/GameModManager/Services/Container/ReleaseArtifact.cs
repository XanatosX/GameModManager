using System;

namespace GameModManager.Services.Container
{
    public record ReleaseArtifact(Version Version, string ArtifactFile, string Checksum);
}
