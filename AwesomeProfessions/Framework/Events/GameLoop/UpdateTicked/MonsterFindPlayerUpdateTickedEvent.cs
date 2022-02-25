namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class MonsterFindPlayerUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        if (!e.IsOneSecond) return;
        
        if (!Context.IsMultiplayer)
        {
            var playerId = Game1.player.UniqueMultiplayerID;
            var location = Game1.player.currentLocation;
            foreach (var monster in location.characters.OfType<Monster>())
            {
                Farmer target;
                if (monster is GreenSlime && Game1.player.HasProfession(Profession.Piper))
                {
                    if (ModEntry.HostState.AggressivePipers.Contains(playerId))
                    {
                        var otherMonsters = location.characters.OfType<Monster>()
                            .Where(m => !m.IsSlime() && m.IsWithinPlayerThreshold()).ToArray();
                        if (otherMonsters.Any())
                        {
                            var closestTarget = Game1.player.GetClosestCharacter(out _, otherMonsters)!;

                            if (!ModEntry.HostState.FakeFarmers.TryGetValue(playerId + 1, out var fakeFarmer))
                            {
                                fakeFarmer = ModEntry.HostState.FakeFarmers[playerId + 1] =
                                    new() {UniqueMultiplayerID = playerId + 1};
                                Log.D($"Created fake farmer with id {playerId + 1}.");
                            }
                            fakeFarmer.currentLocation = location;
                            fakeFarmer.Position = closestTarget.Position;
                            target = fakeFarmer;
                        }
                        else
                        {
                            target = Game1.player;
                        }
                    }
                    else
                    {
                        target = Game1.player;
                    }
                }
                else
                {
                    target = Game1.player;
                }

                monster.WriteData("Target", target?.UniqueMultiplayerID.ToString());
            }

            return;
        }

        var checkedLocations = new HashSet<GameLocation>();
        foreach (var farmer in Game1.getAllFarmers())
        {
            var location = farmer.currentLocation;
            if (checkedLocations.Contains(location)) continue;

            foreach (var monster in location.characters.OfType<Monster>())
            {
                Farmer target;
                if (monster is GreenSlime slime1 && location.DoesAnyPlayerHereHaveProfession(Profession.Piper, out var pipers))
                {
                    var theOneWhoPipedMe = Game1.getFarmer(slime1.ReadDataAs<long>("Piper"));
                    if (theOneWhoPipedMe is null)
                    {
                        theOneWhoPipedMe =
                            slime1.GetClosestFarmer(out _, pipers, f => slime1.IsWithinPlayerThreshold(f))!;
                        slime1.WriteData("Piper", theOneWhoPipedMe.UniqueMultiplayerID.ToString());
                    }

                    var piperId = theOneWhoPipedMe.UniqueMultiplayerID;
                    if (ModEntry.HostState.AggressivePipers.Contains(piperId))
                    {
                        var otherMonsters = location.characters.OfType<Monster>()
                            .Where(m => !m.IsSlime() && m.IsWithinPlayerThreshold(theOneWhoPipedMe)).ToArray();
                        if (otherMonsters.Any())
                        {
                            var closestTarget = theOneWhoPipedMe.GetClosestCharacter(out _, otherMonsters)!;

                            if (!ModEntry.HostState.FakeFarmers.TryGetValue(piperId + 1, out var fakeFarmer))
                            {
                                fakeFarmer = ModEntry.HostState.FakeFarmers[piperId + 1] =
                                    new() {UniqueMultiplayerID = piperId + 1};
                                Log.D($"Created fake farmer with id {piperId + 1}.");
                            }
                            fakeFarmer.currentLocation = location;
                            fakeFarmer.Position = closestTarget.Position;
                            target = fakeFarmer;
                        }
                        else
                        {
                            target = theOneWhoPipedMe;
                        }
                    }
                    else
                    {
                        target = theOneWhoPipedMe;
                    }
                }
                else
                {
                    target = monster.GetClosestFarmer(out _,
                        predicate: f => !ModEntry.HostState.FakeFarmers.ContainsKey(f.UniqueMultiplayerID) &&
                                        !ModEntry.HostState.PoachersInAmbush.Contains(f.UniqueMultiplayerID));
                    if (monster is GreenSlime slime2) slime2.WriteData("Piper", null);
                }

                monster.WriteData("Target", target?.UniqueMultiplayerID.ToString());
            }

            checkedLocations.Add(location);
        }
    }
}