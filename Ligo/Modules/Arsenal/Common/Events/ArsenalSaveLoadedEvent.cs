namespace DaLion.Ligo.Modules.Arsenal.Common.Events;

#region using directives

using System.Linq;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class ArsenalSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ArsenalSaveLoadedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArsenalSaveLoadedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        if (Context.IsMainPlayer)
        {
            Utility.iterateAllItems(item =>
            {
                if (item is not MeleeWeapon weapon || !weapon.isScythe())
                {
                    return;
                }

                if (ModEntry.Config.Arsenal.InfinityPlusOne)
                {
                    Utils.AddEnchantments(weapon);
                }
            });
        }
        else
        {
            foreach (var weapon in Game1.player.Items.OfType<MeleeWeapon>())
            {
                if (weapon.isScythe())
                {
                    continue;
                }

                if (ModEntry.Config.Arsenal.InfinityPlusOne)
                {
                    Utils.AddEnchantments(weapon);
                }
            }
        }
    }
}
