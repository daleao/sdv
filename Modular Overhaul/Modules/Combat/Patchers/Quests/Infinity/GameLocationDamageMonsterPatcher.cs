namespace DaLion.Overhaul.Modules.Combat.Patchers.Quests.Infinity;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationDamageMonsterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationDamageMonsterPatcher"/> class.</summary>
    internal GameLocationDamageMonsterPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(
            nameof(GameLocation.damageMonster),
            new[]
            {
                typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int),
                typeof(float), typeof(float), typeof(bool), typeof(Farmer),
            });
    }

    #region harmony patches

    /// <summary>Record valor points.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationDamageMonsterTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var incrementMethod = typeof(Shared.Extensions.Stardew.FarmerExtensions)
                                      .GetMethods()
                                      .FirstOrDefault(mi =>
                                          mi.Name.Contains(nameof(Shared.Extensions.Stardew.FarmerExtensions.Increment)) && mi.GetGenericArguments().Length > 0)?
                                      .MakeGenericMethod(typeof(uint)) ??
                                  ThrowHelper.ThrowMissingMethodException<MethodInfo>("Increment method not found.");
            var virtuesQuest = generator.DeclareLocal(typeof(HeroQuest));
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Callvirt, typeof(Stats).RequirePropertySetter(nameof(Stats.MonstersKilled))),
                    })
                .Move()
                .GetOperand(out var resumeExecution)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)10),
                        new CodeInstruction(OpCodes.Ldstr, DataKeys.ProvenValor),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Call, incrementMethod),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.State))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModState).RequirePropertyGetter(nameof(ModState.Combat))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(State).RequirePropertyGetter(nameof(State.HeroQuest))),
                        new CodeInstruction(OpCodes.Stloc_S, virtuesQuest),
                        new CodeInstruction(OpCodes.Ldloc_S, virtuesQuest),
                        new CodeInstruction(OpCodes.Brfalse, (Label)resumeExecution),
                        new CodeInstruction(OpCodes.Ldloc_S, virtuesQuest),
                        new CodeInstruction(OpCodes.Ldsfld, typeof(Virtue).RequireField(nameof(Virtue.Valor))),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Call, typeof(HeroQuest).RequireMethod(nameof(HeroQuest.UpdateTrialProgress))),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed recording player valor.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
