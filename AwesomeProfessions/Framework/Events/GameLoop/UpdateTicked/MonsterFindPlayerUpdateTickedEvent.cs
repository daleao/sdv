namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;

using Extensions;
using SuperMode;

#endregion using directives

[UsedImplicitly]
internal class MonsterFindPlayerUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        if (!e.IsOneSecond) return;

        foreach (var monster in Game1.player.currentLocation.characters.OfType<Monster>().Where(m => m.IsWithinPlayerThreshold()))
        {
            if (monster.currentLocation is null || !Context.IsMultiplayer && monster is not GreenSlime)
            {
                monster.WriteData("Player", null);
                continue;
            }

            Farmer target = null;
            if (monster is GreenSlime slime)
            {
                Farmer theOneWhoPipedMe = null;
                var isPiped = false;
                switch (Context.IsMultiplayer)
                {
                    case false when Game1.player.HasProfession(Profession.Piper):
                        theOneWhoPipedMe = Game1.player;
                        isPiped = true;
                        slime.WriteData("Piper", Game1.player.UniqueMultiplayerID.ToString());
                        break;
                    case true when slime.currentLocation.DoesAnyPlayerHereHaveProfession(Profession.Piper, out var pipers):
                    {
                        theOneWhoPipedMe = Game1.getFarmer(slime.ReadDataAs<long>("Piper"));
                        if (theOneWhoPipedMe is null)
                        {
                            var closestPiper = slime.GetClosestCharacter(out _, pipers,
                                f => slime.IsWithinPlayerThreshold(f));
                            theOneWhoPipedMe = closestPiper;
                            slime.WriteData("Piper", closestPiper?.UniqueMultiplayerID.ToString());
                        }

                        if (theOneWhoPipedMe is not null) isPiped = true;
                        break;
                    }
                }

                if (isPiped && ModEntry.State.Value.PipeMode == TargetMode.Aggressive)
                {
                    var otherMonsters = monster.currentLocation.characters.OfType<Monster>()
                        .Where(m => !m.IsSlime() && m.IsWithinPlayerThreshold()).ToArray();
                    if (otherMonsters.Any())
                    {
                        var closestTarget = theOneWhoPipedMe.GetClosestCharacter(out _, otherMonsters);
                        if (closestTarget is not null)
                        {
                            if (!ModEntry.State.Value.FakeFarmers.TryGetValue(theOneWhoPipedMe.UniqueMultiplayerID,
                                    out var fakeFarmer))
                                fakeFarmer = ModEntry.State.Value.FakeFarmers[theOneWhoPipedMe.UniqueMultiplayerID] =
                                    new();

                            fakeFarmer.currentLocation = closestTarget.currentLocation;
                            fakeFarmer.Position = closestTarget.Position;
                            target = fakeFarmer;
                        }
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
                target = monster.GetClosestCharacter<Farmer>(out _,
                    predicate: f => !ModEntry.State.Value.ActivePeerSuperModes.TryGetValue(SuperModeIndex.Poacher, out var peerIds) ||
                                    peerIds.All(id => id != f.UniqueMultiplayerID));
            }

            monster.WriteData("Player", target?.UniqueMultiplayerID.ToString());
        }
    }
}