namespace DaLion.Overhaul.Modules.Arsenal;

#region using directives

using System.Linq;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

/// <summary>Smart <see cref="Tool"/> selector.</summary>
internal static class ArsenalSelector
{
    internal static bool TryFor(Farmer who, out int index)
    {
        index = -1;

        var selectable = ArsenalModule.State.SelectableArsenal;
        if (selectable is null)
        {
            return false;
        }

        switch (selectable)
        {
            case MeleeWeapon:
                if (who.currentLocation.characters.OfType<Monster>().Any(m =>
                        m.DistanceTo(who) <= ArsenalModule.Config.Weapons.AutoSelectionRange))
                {
                    index = who.Items.IndexOf(selectable);
                }

                break;

            case Slingshot:
                if (who.currentLocation.characters.OfType<Monster>().Any(m =>
                        m.DistanceTo(who) <= ArsenalModule.Config.Slingshots.AutoSelectionRange))
                {
                    index = who.Items.IndexOf(selectable);
                }

                break;
        }

        return index >= 0;
    }
}
