namespace DaLion.Harmonics.Framework.Events;

#region using directives

using DaLion.Harmonics.Framework.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="HarmonicsSaveLoadedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class HarmonicsSaveLoadedEvent(EventManager? manager = null)
    : SaveLoadedEvent(manager ?? HarmonicsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        Utility.ForEachItem(item =>
        {
            if (item is not MeleeWeapon weapon)
            {
                return true;
            }

            weapon.Get_CooldownReduction().Value = Data.ReadAs<float>(weapon, DataKeys.CooldownReduction);
            return true;
        });
    }
}
