namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop;

#region using directives

using System.Collections.Generic;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class CombatSavingEvent : SavingEvent
{
    /// <summary>Initializes a new instance of the <see cref="CombatSavingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CombatSavingEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <summary>Gets the cache of weapons with intrinsic enchantments.</summary>
    /// <remarks>For recovery immediately after saving.</remarks>
    internal static List<MeleeWeapon> InstrinsicWeapons { get; } = new();

    /// <inheritdoc />
    protected override void OnSavingImpl(object? sender, SavingEventArgs e)
    {
        Utility.iterateAllItems(item =>
        {
            if (item is not MeleeWeapon weapon || !weapon.HasIntrinsicEnchantment())
            {
                return;
            }

            weapon.RemoveIntrinsicEnchantments();
            InstrinsicWeapons.Add(weapon);
        });

        if (CombatModule.State.AutoSelectableMelee is not null)
        {
            Game1.player.Write(
                DataKeys.SelectableMelee,
                Game1.player.Items.IndexOf(CombatModule.State.AutoSelectableMelee).ToString());
        }

        if (CombatModule.State.AutoSelectableRanged is not null)
        {
            Game1.player.Write(
                DataKeys.SelectableRanged,
                Game1.player.Items.IndexOf(CombatModule.State.AutoSelectableRanged).ToString());
        }
    }
}
