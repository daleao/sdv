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
internal sealed class BushShakePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BushShakePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal BushShakePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Bush>("shake");
    }

    #region harmony patches

    /// <summary>Patch to nerf Ecologist berry quality and increment forage counter for wild berries.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? BushShakeTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: item.Quality = 4;
        // To: item.Quality = Game1.player.GetEcologistForageQuality(); Data.IncrementField<uint>(DataKeys.EcologistItemsForaged);
        try
        {
            helper
                .PatternMatch([
                    new CodeInstruction(OpCodes.Callvirt, typeof(Item).RequirePropertySetter(nameof(Item.Quality)))
                ])
                .Move(-1)
                .ReplaceWith(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.GetEcologistForageQuality))))
                .Insert([
                    // set edibility
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(OpCodes.Dup), // prepare to set quality
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[4]),
                    new CodeInstruction(OpCodes.Call, typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.ApplyEcologistEdibility))),
                    // append to items foraged
                    new CodeInstruction(OpCodes.Call, typeof(ProfessionsMod).RequirePropertyGetter(nameof(Data))),
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[4]),
                    new CodeInstruction(OpCodes.Callvirt, typeof(Item).RequirePropertyGetter(nameof(Item.ItemId))),
                    new CodeInstruction(OpCodes.Ldnull),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ModDataManagerExtensions).RequireMethod(
                            nameof(ModDataManagerExtensions
                                .AppendToEcologistItemsForaged))),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching modded Ecologist wild berry quality and counter increment.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
