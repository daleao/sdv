﻿namespace DaLion.Arsenal.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Arsenal.Framework;
using DaLion.Arsenal.Framework.Configs;
using DaLion.Arsenal.Framework.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MineShaftMonsterDropPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MineShaftMonsterDropPatcher"/> class.</summary>
    internal MineShaftMonsterDropPatcher()
    {
        this.Target = this.RequireMethod<MineShaft>(nameof(MineShaft.monsterDrop));
    }

    #region harmony patches

    /// <summary>Better monster weapons.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MineShaftMonsterDropTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(MineShaft).RequireMethod(nameof(MineShaft.getSpecialItemForThisMineLevel))),
                    })
                .Move()
                .AddLabels(resumeExecution)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Dup), new CodeInstruction(OpCodes.Isinst, typeof(MeleeWeapon)),
                        new CodeInstruction(OpCodes.Brfalse_S, resumeExecution), new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Combat))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(CombatConfig).RequirePropertyGetter(nameof(CombatConfig.WeaponsSlingshots))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(WeaponsSlingshotsConfig).RequirePropertyGetter(nameof(WeaponsSlingshotsConfig.EnableOverhaul))),
                        new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                        new CodeInstruction(OpCodes.Dup),
                        new CodeInstruction(OpCodes.Isinst, typeof(MeleeWeapon)),
                        new CodeInstruction(OpCodes.Ldc_R8, 2d),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(MeleeWeaponExtensions).RequireMethod(nameof(MeleeWeaponExtensions.RandomizeDamage))),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed to boost monster-dropped weapon.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
