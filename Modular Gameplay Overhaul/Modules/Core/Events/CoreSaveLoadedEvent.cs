namespace DaLion.Overhaul.Modules.Core.Events;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class CoreSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Initializes a new instance of the <see cref="CoreSaveLoadedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CoreSaveLoadedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        var checksum = Game1.player.Read("checksum", -1);
        var data = Config.ToString() + Manifest.Version;
        var hash = data.GetDeterministicHashCode();
        if (hash == checksum)
        {
            Log.T("[Core]: Config file passed checksum validation.");
            return;
        }

        Log.T("[Core]: Config file failed checksum validation. Revalidating all modules...");
        EnumerateModules().ForEach(module => module.Revalidate());
        Log.T("[Core]: Done.");
    }
}
