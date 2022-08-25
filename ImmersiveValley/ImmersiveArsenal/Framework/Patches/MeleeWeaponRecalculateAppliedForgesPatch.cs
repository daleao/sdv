namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Common;
using Common.Extensions.Reflection;
using Common.Harmony;
using HarmonyLib;
using Netcode;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponRecalculateAppliedForgesPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MeleeWeaponRecalculateAppliedForgesPatch()
    {
        Target = RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.RecalculateAppliedForges));
    }

    #region harmony patches

    /// <summary>Prevent the game from overriding stabby swords.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MeleeWeaponRecalculateAppliedForgedTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Removed: if ((int)type == 0) { type.Set(3); }

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldfld, typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.type)))
                )
                .FindNext(
                    new CodeInstruction(OpCodes.Ldfld, typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.type)))
                )
                .Retreat()
                .RemoveInstructionsUntil(
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(NetFieldBase<int, NetInt>).RequireMethod(nameof(NetFieldBase<int, NetInt>.Set)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing stabby sword override.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}