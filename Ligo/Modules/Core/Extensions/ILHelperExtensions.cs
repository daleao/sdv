namespace DaLion.Ligo.Modules.Core.Extensions;

#region using directives

using System.Collections.Generic;
using System.Reflection.Emit;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

/// <summary>Extensions for the <see cref="ILHelper"/> class.</summary>
internal static class IlHelperExtensions
{
    /// <summary>
    ///     Inserts a sequence of <see cref="CodeInstruction"/>s at the currently pointed index to log a message to the console.
    /// </summary>
    /// <param name="helper">The <see cref="ILHelper"/> instance.</param>
    /// <param name="message">The message to be logged.</param>
    /// <param name="logLevel">The <see cref="LogLevel"/>.</param>
    /// <returns>The <paramref name="helper"/> instance.</returns>
    internal static ILHelper InsertLogMessage(this ILHelper helper, string message, LogLevel? logLevel)
    {
        var logger = logLevel switch
        {
            LogLevel.Alert => typeof(Log).RequireMethod(nameof(Log.A)),
            LogLevel.Debug => typeof(Log).RequireMethod(nameof(Log.D)),
            LogLevel.Error => typeof(Log).RequireMethod(nameof(Log.E)),
            LogLevel.Info => typeof(Log).RequireMethod(nameof(Log.I)),
            LogLevel.Trace => typeof(Log).RequireMethod(nameof(Log.T)),
            LogLevel.Warn => typeof(Log).RequireMethod(nameof(Log.W)),
            _ => typeof(Log).RequireMethod(nameof(Log.T)),
        };

        return helper.Insert(
            new[] { new CodeInstruction(OpCodes.Ldstr, message), new CodeInstruction(OpCodes.Call, logger) });
    }

    /// <summary>Inserts a sequence of <see cref="CodeInstruction"/>s at the currently pointed index to roll a random double.</summary>
    /// <param name="helper">The <see cref="ILHelper"/> instance.</param>
    /// <param name="chance">The threshold for a successful roll.</param>
    /// <param name="labels">Branch labels to add to the inserted sequence.</param>
    /// <param name="forStaticRandom">Whether to load the static <see cref="Game1.random"/>.</param>
    /// <returns>The <paramref name="helper"/> instance.</returns>
    internal static ILHelper InsertDiceRoll(
        this ILHelper helper, double chance, Label[]? labels = null, bool forStaticRandom = true)
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

        return helper.Insert(toInsert.ToArray());
    }

    /// <summary>Inserts a sequence of <see cref="CodeInstruction"/>s at the currently pointed index to roll a random integer.</summary>
    /// <param name="helper">The <see cref="ILHelper"/> instance.</param>
    /// <param name="minValue">The lower limit, inclusive.</param>
    /// <param name="maxValue">The upper limit, inclusive.</param>
    /// <param name="labels">Branch labels to add to the inserted sequence.</param>
    /// <param name="forStaticRandom">Whether to load the static <see cref="Game1.random"/>.</param>
    /// <returns>The <paramref name="helper"/> instance.</returns>
    internal static ILHelper InsertDiceRoll(
        this ILHelper helper, int minValue, int maxValue, Label[]? labels = null, bool forStaticRandom = true)
    {
        var toInsert = new List<CodeInstruction>();
        if (forStaticRandom)
        {
            toInsert.Add(new CodeInstruction(OpCodes.Ldsfld, typeof(Game1).RequireField(nameof(Game1.random))));
        }

        toInsert.AddRange(helper.LdcFromInt(minValue).Collect(
            helper.LdcFromInt(maxValue + 1),
            new CodeInstruction(OpCodes.Callvirt, typeof(Random).RequireMethod(nameof(Random.Next)))));

        if (labels is not null)
        {
            toInsert[0].labels.AddRange(labels);
        }

        return helper.Insert(toInsert.ToArray());
    }
}
