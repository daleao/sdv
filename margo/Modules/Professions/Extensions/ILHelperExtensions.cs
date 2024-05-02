namespace DaLion.Overhaul.Modules.Professions.Extensions;

#region using directives

using System.Collections.Generic;
using System.Reflection.Emit;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;

#endregion using directives

/// <summary>Extensions for the <see cref="ILHelper"/> class.</summary>
internal static class IlHelperExtensions
{
    /// <summary>
    ///     Finds the first or next occurrence of the pattern corresponding to `player.professions.Contains()` in the
    ///     active <see cref="CodeInstruction"/> list and moves the index pointer to it.
    /// </summary>
    /// <param name="helper">The <see cref="ILHelper"/> instance.</param>
    /// <param name="index">The profession index.</param>
    /// <param name="search">The <see cref="ILHelper.SearchOption"/>.</param>
    /// <returns>The <paramref name="helper"/> instance.</returns>
    internal static ILHelper MatchProfessionCheck(this ILHelper helper, int index, ILHelper.SearchOption search = ILHelper.SearchOption.Next)
    {
        return helper
            .Match(
                new[]
                {
                    new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).RequireField(nameof(Farmer.professions))),
                    helper.LdcFromInt(index),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(NetList<int, NetInt>).RequireMethod(nameof(NetList<int, NetInt>.Contains))),
                },
                search);
    }

    /// <summary>
    ///     Inserts a sequence of <see cref="CodeInstruction"/>s at the currently pointed index to test if the local player
    ///     has a given
    ///     profession.
    /// </summary>
    /// <param name="helper">The <see cref="ILHelper"/> instance.</param>
    /// <param name="professionIndex">The profession id.</param>
    /// <param name="labels">Branch labels to add to the inserted sequence.</param>
    /// <param name="forLocalPlayer">Whether to load the local player.</param>
    /// <returns>The <paramref name="helper"/> instance.</returns>
    internal static ILHelper InsertProfessionCheck(
        this ILHelper helper, int professionIndex, Label[]? labels = null, bool forLocalPlayer = true)
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
                helper.LdcFromInt(professionIndex),
                new CodeInstruction(
                    OpCodes.Callvirt,
                    typeof(NetList<int, NetInt>).RequireMethod(nameof(NetList<int, NetInt>.Contains)))));

        if (labels is not null)
        {
            toInsert[0].labels.AddRange(labels);
        }

        return helper.Insert(toInsert.ToArray());
    }
}
