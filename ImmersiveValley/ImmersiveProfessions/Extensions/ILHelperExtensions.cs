namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using System.Collections.Generic;
using System.Reflection.Emit;
using DaLion.Common.Extensions;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using HarmonyLib;
using Netcode;

#endregion using directives

/// <summary>Extensions for the <see cref="IlHelper"/> class.</summary>
public static class IlHelperExtensions
{
    /// <summary>
    ///     Finds the first or next occurrence of the pattern corresponding to `player.professions.Contains()` in the
    ///     active <see cref="CodeInstruction"/> list and moves the index pointer to it.
    /// </summary>
    /// <param name="helper">The <see cref="IlHelper"/> instance.</param>
    /// <param name="whichProfession">The profession id.</param>
    /// <param name="fromCurrentIndex">Whether to begin search from currently pointed index.</param>
    /// <returns>The <paramref name="helper"/> instance.</returns>
    public static IlHelper FindProfessionCheck(this IlHelper helper, int whichProfession, bool fromCurrentIndex = false)
    {
        return fromCurrentIndex
            ? helper.FindNext(
                new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).RequireField(nameof(Farmer.professions))),
                LoadConstantIntegerIl(whichProfession),
                new CodeInstruction(
                    OpCodes.Callvirt,
                    typeof(NetList<int, NetInt>).RequireMethod(nameof(NetList<int, NetInt>.Contains))))
            : helper.FindFirst(
                new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).RequireField(nameof(Farmer.professions))),
                LoadConstantIntegerIl(whichProfession),
                new CodeInstruction(
                    OpCodes.Callvirt,
                    typeof(NetList<int, NetInt>).RequireMethod(nameof(NetList<int, NetInt>.Contains))));
    }

    /// <summary>
    ///     Inserts a sequence of <see cref="CodeInstruction"/>s at the currently pointed index to test if the local player
    ///     has a given
    ///     profession.
    /// </summary>
    /// <param name="helper">The <see cref="IlHelper"/> instance.</param>
    /// <param name="professionIndex">The profession id.</param>
    /// <param name="labels">Branch labels to add to the inserted sequence.</param>
    /// <param name="forLocalPlayer">Whether to load the local player.</param>
    /// <returns>The <paramref name="helper"/> instance.</returns>
    public static IlHelper InsertProfessionCheck(
        this IlHelper helper, int professionIndex, Label[]? labels = null, bool forLocalPlayer = true)
    {
        var toInsert = new List<CodeInstruction>();
        if (forLocalPlayer)
        {
            toInsert.Add(new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))));
        }

        toInsert.AddRange(
            new CodeInstruction(OpCodes.Ldfld, typeof(Farmer)
                .RequireField(nameof(Farmer.professions)))
                .Collect(
                LoadConstantIntegerIl(professionIndex),
                new CodeInstruction(
                    OpCodes.Callvirt,
                    typeof(NetList<int, NetInt>).RequireMethod(nameof(NetList<int, NetInt>.Contains)))));

        if (labels is not null)
        {
            toInsert[0].labels.AddRange(labels);
        }

        return helper.InsertInstructions(toInsert.ToArray());
    }

    /// <summary>Inserts a sequence of <see cref="CodeInstruction"/>s at the currently pointed index to roll a random double.</summary>
    /// <param name="helper">The <see cref="IlHelper"/> instance.</param>
    /// <param name="chance">The threshold for a successful roll.</param>
    /// <param name="labels">Branch labels to add to the inserted sequence.</param>
    /// <param name="forStaticRandom">Whether to load the static <see cref="Game1.random"/>.</param>
    /// <returns>The <paramref name="helper"/> instance.</returns>
    public static IlHelper InsertDiceRoll(
        this IlHelper helper, double chance, Label[]? labels = null, bool forStaticRandom = true)
    {
        var toInsert = new List<CodeInstruction>();
        if (forStaticRandom)
        {
            toInsert.Add(new CodeInstruction(OpCodes.Ldsfld, typeof(Game1).RequireField(nameof(Game1.random))));
        }

        toInsert.AddRange(
            new CodeInstruction(OpCodes.Callvirt, typeof(Random)
                .RequireMethod(nameof(Random.NextDouble)))
                .Collect(
                new CodeInstruction(OpCodes.Ldc_R8, chance)));

        if (labels is not null)
        {
            toInsert[0].labels.AddRange(labels);
        }

        return helper.InsertInstructions(toInsert.ToArray());
    }

    /// <summary>Inserts a sequence of <see cref="CodeInstruction"/>s at the currently pointed index to roll a random integer.</summary>
    /// <param name="helper">The <see cref="IlHelper"/> instance.</param>
    /// <param name="minValue">The lower limit, inclusive.</param>
    /// <param name="maxValue">The upper limit, inclusive.</param>
    /// <param name="labels">Branch labels to add to the inserted sequence.</param>
    /// <param name="forStaticRandom">Whether to load the static <see cref="Game1.random"/>.</param>
    /// <returns>The <paramref name="helper"/> instance.</returns>
    public static IlHelper InsertDiceRoll(
        this IlHelper helper, int minValue, int maxValue, Label[]? labels = null, bool forStaticRandom = true)
    {
        var toInsert = new List<CodeInstruction>();
        if (forStaticRandom)
        {
            toInsert.Add(new CodeInstruction(OpCodes.Ldsfld, typeof(Game1).RequireField(nameof(Game1.random))));
        }

        toInsert.AddRange(LoadConstantIntegerIl(minValue).Collect(
            LoadConstantIntegerIl(maxValue + 1),
            new CodeInstruction(OpCodes.Callvirt, typeof(Random).RequireMethod(nameof(Random.Next)))));

        if (labels is not null)
        {
            toInsert[0].labels.AddRange(labels);
        }

        return helper.InsertInstructions(toInsert.ToArray());
    }

    /// <summary>Gets the corresponding <see cref="CodeInstruction"/> which loads a given integer.</summary>
    /// <param name="number">An integer.</param>
    /// <returns>A correct <see cref="CodeInstruction"/> which loads <paramref name="number"/>.</returns>
    private static CodeInstruction LoadConstantIntegerIl(int number)
    {
        if (number > byte.MaxValue)
        {
            ThrowHelper.ThrowArgumentException($"Number is too large. Should be less than {byte.MaxValue}.");
        }

        return number switch
        {
            0 => new CodeInstruction(OpCodes.Ldc_I4_0),
            1 => new CodeInstruction(OpCodes.Ldc_I4_1),
            2 => new CodeInstruction(OpCodes.Ldc_I4_2),
            3 => new CodeInstruction(OpCodes.Ldc_I4_3),
            4 => new CodeInstruction(OpCodes.Ldc_I4_4),
            5 => new CodeInstruction(OpCodes.Ldc_I4_5),
            6 => new CodeInstruction(OpCodes.Ldc_I4_6),
            7 => new CodeInstruction(OpCodes.Ldc_I4_7),
            8 => new CodeInstruction(OpCodes.Ldc_I4_8),
            _ => new CodeInstruction(OpCodes.Ldc_I4_S, number),
        };
    }
}
