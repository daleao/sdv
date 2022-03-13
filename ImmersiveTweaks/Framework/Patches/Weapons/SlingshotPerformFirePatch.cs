namespace DaLion.Stardew.Tweaks.Framework.Patches.Weapons;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal class SlingshotPerformFirePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal SlingshotPerformFirePatch()
    {
        Original = RequireMethod<Slingshot>(nameof(Slingshot.PerformFire));
    }

    #region harmony patches

    /// <summary>Adds stamina cost to slingshots.</summary>
    [HarmonyPostfix]
    private static void SlingshotPerformFirePostfix(Farmer who)
    {
        if (ModEntry.Config.WeaponsCostStamina)
            who.Stamina -= 2 - who.CombatLevel * 0.1f;
    }

    #endregion harmony patches
}