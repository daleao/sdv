namespace DaLion.Overhaul.Modules.Combat;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Smart <see cref="Tool"/> selector.</summary>
internal static class ToolSelector
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
            distance <= CombatModule.Config.ControlsUi.MeleeAutoSelectionRange)
        {
            index = who.Items.IndexOf(CombatModule.State.AutoSelectableMelee);
        }
        else if (CombatModule.State.AutoSelectableRanged is not null &&
                 distance <= CombatModule.Config.ControlsUi.RangedAutoSelectionRange)
        {
            index = who.Items.IndexOf(CombatModule.State.AutoSelectableRanged);
        }

        return index >= 0;
    }
}
