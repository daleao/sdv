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

        var whereType = original.GetParameters()[0].ParameterType;
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
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Box, whereType),
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(GameLocationCheckInspectPetAnimalPatcher).RequireMethod(nameof(CheckFeedCrop))),
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

    private static bool CheckFeedCrop(FarmAnimal animal, object where, Farmer who)
    {
        if (!who.HasProfession(Profession.Rancher) || who.ActiveObject is null ||
            !Lookups.CategoryByFeed.TryGetValue(who.ActiveObject.QualifiedItemId, out var feedCategory))
        {
            return false;
        }

        switch (where)
        {
            case Vector2 position:
                if (!animal.GetCursorPetBoundingBox().Contains((int)position.X, (int)position.Y))
                {
                    return false;
                }

                break;
            case Rectangle rect:
                if (!animal.GetBoundingBox().Intersects(rect))
                {
                    return false;
                }

                break;
            default:
                return false;
        }

        if (Data.ReadAs<bool>(animal, DataKeys.WasSupplementedToday))
        {
            animal.doEmote(40);
            return true;
        }

        if (!Lookups.FavoredFeedsByAnimalType.TryGetValue(animal.type.Value, out var favoriteFeeds))
        {
            if (animal.type.Value.Contains("Chicken", StringComparison.OrdinalIgnoreCase))
            {
                favoriteFeeds = Lookups.FavoredFeedsByAnimalType["Chicken"];
            }
            else if (animal.type.Value.Contains("Cow", StringComparison.OrdinalIgnoreCase))
            {
                favoriteFeeds = Lookups.FavoredFeedsByAnimalType["Cow"];
            }
            else
            {
                return false;
            }
        }

        if (favoriteFeeds.Contains(feedCategory))
        {
            who.FeedCrop(animal);
            return true;
        }
        else
        {
            animal.doEmote(36);
            //Game1.drawDialogueBox(I18n.Animals_CantEat(animal.Name));
            return true;
        }
    }

    #endregion injected
}
