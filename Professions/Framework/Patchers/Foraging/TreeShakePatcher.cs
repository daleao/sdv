namespace DaLion.Professions.Framework.Patchers.Foraging;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class TreeShakePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="TreeShakePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal TreeShakePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Tree>(nameof(Tree.shake));
    }

    #region harmony patches

    /// <summary>Patch to apply Ecologist perk to shaken trees.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? TreeShakeTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .ForEach(
                    [
                        new CodeInstruction(OpCodes.Callvirt, typeof(Item).RequirePropertySetter(nameof(Item.Quality)))
                    ],
                    i =>
                    {
                        helper
                            .Move(-1)
                            .ReplaceWith(
                                new CodeInstruction(
                                    OpCodes.Call,
                                    typeof(FarmerExtensions).RequireMethod(
                                        nameof(FarmerExtensions.GetEcologistForageQuality))))
                            .Insert([
                                new CodeInstruction(
                                    OpCodes.Call,
                                    typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                                new CodeInstruction(
                                    OpCodes.Call,
                                    typeof(ProfessionsMod).RequirePropertyGetter(nameof(Data))),
                                new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[i == 0 ? 13 : 15]),
                                new CodeInstruction(
                                    OpCodes.Callvirt,
                                    typeof(Item).RequirePropertyGetter(nameof(Item.ItemId))),
                                new CodeInstruction(OpCodes.Ldnull),
                                new CodeInstruction(
                                    OpCodes.Call,
                                    typeof(ModDataManagerExtensions).RequireMethod(
                                        nameof(ModDataManagerExtensions
                                            .AppendToEcologistItemsForaged))),
                            ])
                            .Move();
                    }); // otherwise the patcher gets stuck infinite looping on the same instruction
        }
        catch (Exception ex)
        {
            Log.E($"Failed applying Ecologist perk to shaken trees.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
