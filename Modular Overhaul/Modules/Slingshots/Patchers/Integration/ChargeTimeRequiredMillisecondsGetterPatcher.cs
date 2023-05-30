namespace DaLion.Overhaul.Modules.Slingshots.Patchers.Integration;

#region using directives

using DaLion.Overhaul.Modules.Slingshots.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[RequiresMod("PeacefulEnd.Archery", "Archery", "2.1.0")]
internal sealed class ChargeTimeRequiredMillisecondsGetterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ChargeTimeRequiredMillisecondsGetterPatcher"/> class.</summary>
    internal ChargeTimeRequiredMillisecondsGetterPatcher()
    {
        this.Target = "Archery.Framework.Models.Weapons.WeaponModel"
            .ToType()
            .RequirePropertyGetter("ChargeTimeRequiredMilliseconds");
        this.Postfix!.after = new[] { OverhaulModule.Slingshots.Namespace };
    }

    #region harmony patches

    /// <summary>Patch to reduce Bow charge time for Desperado.</summary>
    [HarmonyPostfix]
    [HarmonyAfter("DaLion.Overhaul.Modules.Slingshots")]
    private static void ChargeTimeRequiredMillisecondsGetterPatcherPostfix(ref float __result)
    {
        var player = Game1.player;
        var tool = Game1.player.CurrentTool;
        if (player.IsLocalPlayer && tool is Slingshot slingshot)
        {
            __result *= player.GetTotalFiringSpeedModifier(slingshot);
        }
    }

    #endregion harmony patches
}
