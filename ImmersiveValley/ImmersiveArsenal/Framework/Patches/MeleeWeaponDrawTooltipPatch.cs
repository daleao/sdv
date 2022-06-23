namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Tools;

using Common;
using Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDrawTooltipPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal MeleeWeaponDrawTooltipPatch()
    {
        Target = RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.drawTooltip));
    }

    #region harmony patches

    /// <summary>Make weapon stats human-readable..</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> MeleeWeaponDrawTooltipTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            throw new NotImplementedException("Please implement.");
        }
        catch (Exception ex)
        {
            Log.E($"Failed xxx.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}