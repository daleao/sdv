namespace DaLion.Stardew.Professions;

#region using directives

using System.Collections.Generic;
using StardewValley;

#endregion using directives

internal class HostState
{
    internal HashSet<long> PlayersHuntingTreasure { get; } = new();
    internal HashSet<long> PoachersInAmbush { get; } = new();
    internal Dictionary<long, Farmer> FakeFarmers { get; } = new();
}