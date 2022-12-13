namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class PickaxeBeginUsingPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="PickaxeBeginUsingPatcher"/> class.</summary>
    internal PickaxeBeginUsingPatcher()
    {
        this.Target = this.RequireMethod<Pickaxe>("beginUsing");
    }

    #region harmony patches

    /// <summary>Enable Pick power level increase.</summary>
    [HarmonyPrefix]
    private static bool PickaxeBeginUsingPrefix(Tool __instance, Farmer who)
    {
        if (!ToolsModule.Config.Pick.EnableCharging ||
            (ToolsModule.Config.RequireModkey && !ToolsModule.Config.Modkey.IsDown()) ||
            __instance.UpgradeLevel < (int)ToolsModule.Config.Pick.RequiredUpgradeForCharging)
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

    #endregion harmony patches
}
