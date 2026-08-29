namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationCheckInspectPetAnimalPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationCheckInspectPetAnimalPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GameLocationCheckInspectPetAnimalPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
    }

    /// <inheritdoc />
    protected override bool ApplyImpl(Harmony harmony)
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.CheckPetAnimal), [typeof(Rectangle), typeof(Farmer)]);
        if (!base.ApplyImpl(harmony))
        {
            return false;
        }

        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.CheckPetAnimal), [typeof(Vector2), typeof(Farmer)]);
        if (!base.ApplyImpl(harmony))
        {
            return false;
        }

        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.CheckInspectAnimal), [typeof(Rectangle), typeof(Farmer)]);
        if (!base.ApplyImpl(harmony))
        {
            return false;
        }

        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.CheckInspectAnimal), [typeof(Vector2), typeof(Farmer)]);
        return base.ApplyImpl(harmony);
    }

    #region harmony patches

    /// <summary>Patch to implement animal crop feeding.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? GameLocationCheckInspectPetAnimalTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var didntFeed = generator.DefineLabel();
            helper
                .PatternMatch([
                    new CodeInstruction(OpCodes.Stloc_2)
                ])
                .Move()
                .AddLabels(didntFeed)
                .PatternMatch([
                    new CodeInstruction(OpCodes.Leave_S)
                ])
                .GetOperand(out var didFeed)
                .Return()
                .Insert([
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(OpCodes.Call, typeof(GameLocationCheckInspectPetAnimalPatcher).RequireMethod(nameof(CheckFeedCrop))),
                    new CodeInstruction(OpCodes.Brfalse_S, didntFeed),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Stloc_S, helper.Locals[4]),
                    new CodeInstruction(OpCodes.Leave_S, (Label)didFeed),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting animal crop feed.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected

    private static bool CheckFeedCrop(FarmAnimal animal, Farmer who)
    {
        if (!who.HasProfession(Profession.Rancher) || State.WasFedCropToday.Contains(animal) ||
            who.ActiveObject is null || !Lookups.CategoryByFeedCrop.TryGetValue(who.ActiveObject.QualifiedItemId, out var cropCategory))
        {
            return false;
        }

        var type = animal.GetAnimalType();
        if (!Lookups.AnimalFavoredFeeds.TryGetValue(type, out var favoriteFeeds))
        {
            return false;
        }

        if (favoriteFeeds.Contains(cropCategory))
        {
            who.FeedCrop(animal);
            return true;
        }

        return false;
    }

    #endregion injected
}
