namespace DaLion.Shared.Extensions.SMAPI;

/// <summary>Extensions for the <see cref="Mod"/> class.</summary>
public static class ModExtensions
{
    /// <summary>Compares the local <see cref="IManifest"/> to that of the host.</summary>
    /// <param name="mod">The <see cref="Mod"/>.</param>
    public static void ValidateMultiplayer(this Mod mod)
    {
        if (!Context.IsMultiplayer || Context.IsOnHostComputer)
        {
            return;
        }

        var host = mod.Helper.Multiplayer.GetConnectedPlayer(Game1.MasterPlayer.UniqueMultiplayerID)!;
        var uniqueId = mod.ModManifest.UniqueID;
        var thisVersion = mod.ModManifest.Version;
        var hostMod = host.GetMod(uniqueId);
        if (hostMod is null)
        {
            Log.W(
                $"{uniqueId} was not installed by the session host. " +
                "Mod features may not work correctly.");
        }
        else
        {
            var hostVersion = hostMod.Version;
            if (!thisVersion.Equals(hostVersion))
            {
                Log.W(
                    $"The session host has a different version of {uniqueId} installed. " +
                    "Mod features may not work correctly." +
                    $"\n\tHost version: {hostMod.Version}" +
                    $"\n\tLocal version: {thisVersion}");
            }
        }
    }
}
