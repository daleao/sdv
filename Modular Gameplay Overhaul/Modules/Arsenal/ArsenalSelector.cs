namespace DaLion.Overhaul.Modules.Arsenal;

#region using directives

using System.Linq;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

/// <summary>Smart <see cref="Tool"/> selector.</summary>
internal static class ArsenalSelector
{
    internal static int SmartSelect(Vector2 actionTile, Farmer who, GameLocation location)
    {
        var selectable = ArsenalModule.State.SelectableArsenal;
        if (selectable is null)
        {
            return -1;
        }

        switch (selectable)
        {
            case MeleeWeapon:
                if (location.characters.OfType<Monster>().Any(m =>
                        m.DistanceTo(who) <= ArsenalModule.Config.Weapons.AutoSelectionRange))
                {
                    return who.Items.IndexOf(selectable);
                }

                // this doesn't work too well unfortunately
                //var tileLocation1 = Vector2.Zero;
                //var tileLocation2 = Vector2.Zero;
                //var aoe = weapon.getAreaOfEffect(
                //    (int)actionTile.X,
                //    (int)actionTile.Y,
                //    who.FacingDirection,
                //    ref tileLocation1,
                //    ref tileLocation2,
                //    who.GetBoundingBox(),
                //    who.FarmerSprite.currentAnimationIndex);

                //if (location.characters.OfType<Monster>().Any(m => m.GetBoundingBox().Intersects(aoe)))
                //{
                //    return who.Items.IndexOf(selectable);
                //}

                break;

            case Slingshot:
                if (location.characters.OfType<Monster>().Any(m =>
                        m.DistanceTo(who) <= ArsenalModule.Config.Slingshots.AutoSelectionRange))
                {
                    return who.Items.IndexOf(selectable);
                }

                break;
        }

        return -1;
    }
}
