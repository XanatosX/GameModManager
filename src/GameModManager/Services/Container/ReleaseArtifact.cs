using System;

namespace GameModManager.Services.Container
{
    /// <summary>
    /// Record for a single release artifact to store on the compuiter
    /// </summary>
    /// <param name="Version">The version of the artifact</param>
    /// <param name="ArtifactFile">The path to the artifact file</param>
    /// <param name="Checksum">The checksum for the artifact</param>
    public record ReleaseArtifact(Version Version, string ArtifactFile, string Checksum);
}
