namespace DaLion.Stardew.Tweaks.Framework.Patches.Fishing;

#region using directives

using Common;
using Common.Extensions.Reflection;
using Common.Harmony;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class BobberBarUpdatePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal BobberBarUpdatePatch()
    {
        Target = RequireMethod<BobberBar>(nameof(BobberBar.update));
    }

    #region harmony patches

    /// <summary>Patch to fix vanilla pirate treasure bug.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? BobberBarUpdateTranspiler(IEnumerable<CodeInstruction> instructions,
        MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: treasurePosition = ((bobberBarPos > 274f) ? Game1.random.Next(8, (int)bobberBarPos - 20) : Game1.random.Next(Math.Min(528, (int)bobberBarPos + bobberBarHeight), 500));
        /// To: treasurePosition = Game1.random.Next(8, (int)bobberBarPos - 20);

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(BobberBar).RequireField("bobberBarPos")),
                    new CodeInstruction(OpCodes.Ldc_R4, 274f)
                )
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Br_S)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching vanilla pirate treasure bug.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}