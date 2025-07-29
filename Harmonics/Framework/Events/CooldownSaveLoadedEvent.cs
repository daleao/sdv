namespace DaLion.Harmonics.Framework.Events;

#region using directives

using DaLion.Harmonics.Framework.Integrations;
using DaLion.Harmonics.Framework.VirtualProperties;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="CooldownSaveLoadedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class CooldownSaveLoadedEvent(EventManager? manager = null)
    : SaveLoadedEvent(manager ?? HarmonicsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        if (WearMoreRingsIntegration.Instance?.IsLoaded ?? false)
        {
            var api = WearMoreRingsIntegration.Instance.ModApi;
            for (var slot = 0; slot < api.RingSlotCount(); slot++)
            {
                var ring = api.GetRing(slot);
                if (ring?.ItemId == GarnetRingId)
                {
                    Game1.player.Get_CooldownReduction().Value += 0.1f;
                }
            }

            return;
        }

        foreach (var ring in Game1.player.leftRing.Value.Collect(Game1.player.rightRing.Value))
        {
            if (ring?.ItemId == GarnetRingId)
            {
                Game1.player.Get_CooldownReduction().Value += 0.1f;
            }
        }
    }
}
