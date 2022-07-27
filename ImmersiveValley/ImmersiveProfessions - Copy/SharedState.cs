namespace DaLion.Stardew.Professions;

#region using directives

using Netcode;
using StardewValley;

#endregion using directives

internal class SharedState
{
    internal NetCollection<Farmer> PlayersHuntingTreasure { get; } = new();
    internal NetCollection<Farmer> PlayersInAmbush { get; } = new();
}