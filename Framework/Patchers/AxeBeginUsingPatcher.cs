namespace DaLion.Chargeable.Framework.Patchers;

#region using directives

using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[HarmonyPatch(typeof(Axe), nameof(Axe.beginUsing))]
internal sealed class AxeBeginUsingPatcher
{
    /// <summary>Enable Axe power level increase.</summary>
    private static bool Prefix(Tool __instance, Farmer who)
    {
        if (!Config.Axe.EnableCharging ||
            (Config.RequireModkey && !Config.Modkey.IsDown()) ||
            __instance.UpgradeLevel < (int)Config.Axe.RequiredUpgradeForCharging)
        {
            return true; // run original logic
        }

        who.Halt();
        __instance.Update(who.FacingDirection, 0, who);
        switch (who.FacingDirection)
        {
            case Game1.up:
                who.FarmerSprite.setCurrentFrame(176);
                __instance.Update(0, 0, who);
                break;

            case Game1.right:
                who.FarmerSprite.setCurrentFrame(168);
                __instance.Update(1, 0, who);
                break;

            case Game1.down:
                who.FarmerSprite.setCurrentFrame(160);
                __instance.Update(2, 0, who);
                break;

            case Game1.left:
                who.FarmerSprite.setCurrentFrame(184);
                __instance.Update(3, 0, who);
                break;
        }

        return false; // don't run original logic
    }
}
