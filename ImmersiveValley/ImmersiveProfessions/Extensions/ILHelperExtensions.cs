namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using Common.Extensions;
using Common.Extensions.Reflection;
using Common.Harmony;
using HarmonyLib;
using Netcode;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

#endregion using directives

/// <summary>Extensions for the <see cref="ILHelper"/> class.</summary>
public static class ILHelperExtensions
{
    /// <summary>
    ///     Find the first or next occurrence of the pattern corresponding to `player.professions.Contains()` in the
    ///     active code instruction list and move the index pointer to it.
    /// </summary>
    /// <param name="whichProfession">The profession id.</param>
    /// <param name="fromCurrentIndex">Whether to begin search from currently pointed index.</param>
    public static ILHelper FindProfessionCheck(this ILHelper helper, int whichProfession, bool fromCurrentIndex = false)
    {
        return fromCurrentIndex
            ? helper.FindNext(
                new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).RequireField(nameof(Farmer.professions))),
                LoadConstantIntegerIL(whichProfession),
                new CodeInstruction(OpCodes.Callvirt,
                    typeof(NetList<int, NetInt>).RequireMethod(nameof(NetList<int, NetInt>.Contains)))
            )
            : helper.FindFirst(
                new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).RequireField(nameof(Farmer.professions))),
                LoadConstantIntegerIL(whichProfession),
                new CodeInstruction(OpCodes.Callvirt,
                    typeof(NetList<int, NetInt>).RequireMethod(nameof(NetList<int, NetInt>.Contains)))
            );
    }

    /// <summary>
    ///     InsertWithLabels a sequence of code instructions at the currently pointed index to test if the local player has a given
    ///     profession.
    /// </summary>
    /// <param name="professionIndex">The profession id.</param>
    /// <param name="labels">Branch labels to add to the inserted sequence.</param>
    /// <param name="forLocalPlayer">Whether to load the local player.</param>
    public static ILHelper InsertProfessionCheck(this ILHelper helper, int professionIndex, Label[]? labels = null, bool forLocalPlayer = true)
    {
        var toInsert = new List<CodeInstruction>();
        if (forLocalPlayer)
            toInsert.Add(new(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))));

        toInsert.AddRange(
            new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).RequireField(nameof(Farmer.professions))).Collect(
                LoadConstantIntegerIL(professionIndex),
                new CodeInstruction(OpCodes.Callvirt,
                    typeof(NetList<int, NetInt>).RequireMethod(nameof(NetList<int, NetInt>.Contains)))));

        if (labels is not null) toInsert[0].labels.AddRange(labels);

        return helper.Insert(toInsert.ToArray());
    }

    /// <summary>Insert a sequence of code instructions at the currently pointed index to roll a random double.</summary>
    /// <param name="chance">The threshold for a successful roll.</param>
    /// <param name="labels">Branch labels to add to the inserted sequence.</param>
    /// <param name="forStaticRandom">Whether to load the static <see cref="Game1.random"/>.</param>
    public static ILHelper InsertDiceRoll(this ILHelper helper, double chance, Label[]? labels = null, bool forStaticRandom = true)
    {
        var toInsert = new List<CodeInstruction>();
        if (forStaticRandom)
            toInsert.Add(new(OpCodes.Ldsfld, typeof(Game1).RequireField(nameof(Game1.random))));

        toInsert.AddRange(
            new CodeInstruction(OpCodes.Callvirt, typeof(Random).RequireMethod(nameof(Random.NextDouble))).Collect(
                new CodeInstruction(OpCodes.Ldc_R8, chance)));

        if (labels is not null) toInsert[0].labels.AddRange(labels);

        return helper.Insert(toInsert.ToArray());
    }

    /// <summary>Insert a sequence of code instructions at the currently pointed index to roll a random integer.</summary>
    /// <param name="minValue">The lower limit, inclusive.</param>
    /// <param name="maxValue">The upper limit, inclusive.</param>
    /// <param name="labels">Branch labels to add to the inserted sequence.</param>
    /// <param name="forStaticRandom">Whether to load the static <see cref="Game1.random"/>.</param>
    public static ILHelper InsertDiceRoll(this ILHelper helper, int minValue, int maxValue, Label[]? labels = null, bool forStaticRandom = true)
    {
        var toInsert = new List<CodeInstruction>();
        if (forStaticRandom)
            toInsert.Add(new(OpCodes.Ldsfld, typeof(Game1).RequireField(nameof(Game1.random))));

        toInsert.AddRange(LoadConstantIntegerIL(minValue).Collect(LoadConstantIntegerIL(maxValue + 1),
            new CodeInstruction(OpCodes.Callvirt, typeof(Random).RequireMethod(nameof(Random.Next)))));

        if (labels is not null) toInsert[0].labels.AddRange(labels);

        return helper.Insert(toInsert.ToArray());
    }


    /// <summary>Get the corresponding IL code instruction which loads a given integer.</summary>
    /// <param name="number">An integer.</param>
    private static CodeInstruction LoadConstantIntegerIL(int number)
    {
        if (number > byte.MaxValue)
            throw new ArgumentException($"Number is too large. Should be less than {byte.MaxValue}.");

        return number switch
        {
            0 => new(OpCodes.Ldc_I4_0),
            1 => new(OpCodes.Ldc_I4_1),
            2 => new(OpCodes.Ldc_I4_2),
            3 => new(OpCodes.Ldc_I4_3),
            4 => new(OpCodes.Ldc_I4_4),
            5 => new(OpCodes.Ldc_I4_5),
            6 => new(OpCodes.Ldc_I4_6),
            7 => new(OpCodes.Ldc_I4_7),
            8 => new(OpCodes.Ldc_I4_8),
            _ => new(OpCodes.Ldc_I4_S, number)
        };
    }
}