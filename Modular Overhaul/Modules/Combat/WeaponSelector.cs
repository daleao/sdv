namespace DaLion.Overhaul.Modules.Combat;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Smart <see cref="Tool"/> selector.</summary>
internal static class WeaponSelector
{
    internal static bool TryFor(Farmer who, out int index)
    {
        index = -1;

        var closest = who.GetClosestCharacter<Monster>(out var distance);
        if (closest is null)
        {
            return false;
        }

        if (CombatModule.State.AutoSelectableMelee is not null &&
            distance <= CombatModule.Config.MeleeAutoSelectionRange)
        {
            index = who.Items.IndexOf(CombatModule.State.AutoSelectableMelee);
        }
        else if (CombatModule.State.AutoSelectableRanged is not null &&
                 distance <= CombatModule.Config.RangedAutoSelectionRange)
        {
            index = who.Items.IndexOf(CombatModule.State.AutoSelectableRanged);
        }

        return index >= 0;
    }
}
