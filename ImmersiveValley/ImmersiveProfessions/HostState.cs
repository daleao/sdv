namespace DaLion.Stardew.Professions;

#region using directives

using StardewValley;
using System.Collections.Generic;

#endregion using directives

internal class HostState
{
    internal HashSet<long> PlayersHuntingTreasure { get; } = new();
    internal HashSet<long> PoachersInAmbush { get; } = new();
    internal Dictionary<long, Farmer> FakeFarmers { get; } = new();
}