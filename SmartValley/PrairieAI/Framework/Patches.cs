namespace DaLion.Stardew.Tools.Framework;

#region using directives

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Minigames;

using Common.Extensions.Reflection;
using Common.Harmony;
using Prairie.Training;

#endregion using directives

/// <summary>Patches the game code to track fitness metrics.</summary>
[UsedImplicitly]
internal static class Patches
{
    #region harmony patches

    [HarmonyPatch(typeof(AbigailGame), nameof(AbigailGame.playerDie))]
    internal class AbigailGamePlayerDiePatch
    {
        [HarmonyPrefix]
        private static void Prefix()
        {
            ++ModEntry.DeathCount;
        }
    }

    [HarmonyPatch(typeof(AbigailGame), nameof(AbigailGame.updateBullets))]
    internal class AbigailGameUpdateBulletsPatch
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            var helper = new ILHelper(original, instructions);

            try
            {
                helper
                    .FindFirst(
                        new CodeInstruction(OpCodes.Ldsfld,
                            typeof(AbigailGame).RequireField(nameof(AbigailGame.monsters))),
                        new CodeInstruction(OpCodes.Ldloc_1),
                        new CodeInstruction(OpCodes.Callvirt,
                            typeof(List<AbigailGame.CowboyMonster>).RequireMethod(
                                nameof(List<AbigailGame.CowboyMonster>.RemoveAt)))
                    )
                    .Insert(
                        new CodeInstruction(OpCodes.Callvirt,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.EnemiesDefeated))),
                        new CodeInstruction(OpCodes.Ldsfld,
                            typeof(AbigailGame).RequireField(nameof(AbigailGame.monsters))),
                        new CodeInstruction(OpCodes.Callvirt,
                            typeof(List<AbigailGame.CowboyMonster>).RequirePropertyGetter(
                                nameof(List<AbigailGame.CowboyMonster>.Count))),
                        new CodeInstruction(OpCodes.Add),
                        new CodeInstruction(OpCodes.Callvirt,
                            typeof(ModEntry).RequirePropertySetter(nameof(ModEntry.EnemiesDefeated)))
                    );
            }
            catch (Exception ex)
            {
                Log.E($"Failed applying enemy kill count to bullet update.\nHelper returned {ex}");
                return null;
            }

            return helper.Flush();
        }
    }

    [HarmonyPatch(typeof(AbigailGame), nameof(AbigailGame.usePowerup))]
    internal class AbigailGameUsePowerupPatch
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            var helper = new ILHelper(original, instructions);

            try
            {
                helper
                    .FindNext(
                        new CodeInstruction(OpCodes.Ldsfld,
                            typeof(AbigailGame).RequireField(nameof(AbigailGame.monsters))),
                        new CodeInstruction(OpCodes.Callvirt,
                            typeof(List<AbigailGame.CowboyMonster>).RequireMethod(
                                nameof(List<AbigailGame.CowboyMonster>.Clear)))
                    )
                    .Advance(3)
                    .Insert(
                        new CodeInstruction(OpCodes.Callvirt,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.EnemiesDefeated))),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Add),
                        new CodeInstruction(OpCodes.Callvirt,
                            typeof(ModEntry).RequirePropertySetter(nameof(ModEntry.EnemiesDefeated)))
                    );
            }
            catch (Exception ex)
            {
                Log.E($"Failed applying enemy kill count to use powerup.\nHelper returned {ex}");
                return null;
            }

            try
            {
                helper
                    .FindNext(
                        new CodeInstruction(OpCodes.Ldfld,
                            typeof(AbigailGame).RequireField(nameof(AbigailGame.coins))),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Add),
                        new CodeInstruction(OpCodes.Stfld,
                            typeof(AbigailGame).RequireField(nameof(AbigailGame.coins)))
                    )
                    .Insert(
                        new CodeInstruction(OpCodes.Callvirt,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.CoinsCollected))),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Add),
                        new CodeInstruction(OpCodes.Callvirt,
                            typeof(ModEntry).RequirePropertySetter(nameof(ModEntry.EnemiesDefeated)))
                    )
                    .FindNext(
                        new CodeInstruction(OpCodes.Ldfld,
                            typeof(AbigailGame).RequireField(nameof(AbigailGame.coins))),
                        new CodeInstruction(OpCodes.Ldc_I4_5),
                        new CodeInstruction(OpCodes.Add),
                        new CodeInstruction(OpCodes.Stfld,
                            typeof(AbigailGame).RequireField(nameof(AbigailGame.coins)))
                    )
                    .Insert(
                        new CodeInstruction(OpCodes.Callvirt,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.CoinsCollected))),
                        new CodeInstruction(OpCodes.Ldc_I4_5),
                        new CodeInstruction(OpCodes.Add),
                        new CodeInstruction(OpCodes.Callvirt,
                            typeof(ModEntry).RequirePropertySetter(nameof(ModEntry.EnemiesDefeated)))
                    );
            }
            catch (Exception ex)
            {
                Log.E($"Failed applying coin count to use powerup.\nHelper returned {ex}");
                return null;
            }

            return helper.Flush();
        }
    }

    [HarmonyPatch(typeof(AbigailGame), nameof(AbigailGame.tick))]
    internal class AbigailGameTickPatch
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            var helper = new ILHelper(original, instructions);

            var i = 0;
            loop:
            try
            {
                helper
                    .FindNext(
                        new CodeInstruction(OpCodes.Ldsfld,
                            typeof(AbigailGame).RequireField(nameof(AbigailGame.monsters))),
                        new CodeInstruction(OpCodes.Ldloc_S),
                        new CodeInstruction(OpCodes.Callvirt,
                            typeof(List<AbigailGame.CowboyMonster>).RequireMethod(
                                nameof(List<AbigailGame.CowboyMonster>.RemoveAt)))
                    )
                    .Insert(
                        new CodeInstruction(OpCodes.Callvirt,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.EnemiesDefeated))),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Add),
                        new CodeInstruction(OpCodes.Callvirt,
                            typeof(ModEntry).RequirePropertySetter(nameof(ModEntry.EnemiesDefeated)))
                    );
            }
            catch (Exception ex)
            {
                Log.E($"Failed applying enemy kill count to game tick.\nHelper returned {ex}");
                return null;
            }

            // repeat once
            if (++i < 1) goto loop;

            return helper.Flush();
        }
    }

    #endregion harmony patches
}