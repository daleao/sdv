namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using Common;
using Common.Events;
using Common.Extensions.Collections;
using Common.ModData;
using Extensions;
using StardewModdingAPI.Events;
using StardewValley.Buildings;
using System.Linq;
using Ultimates;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class StaticSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal StaticSaveLoadedEvent(ProfessionEventManager manager)
        : base(manager)
    {
        AlwaysEnabled = true;
    }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        // enable events
        Manager.EnableForLocalPlayer();

        // load and initialize Ultimate index
        Log.T("Initializing Ultimate...");

        var ultimateIndex = ModDataIO.Read(Game1.player, "UltimateIndex", UltimateIndex.None);
        switch (ultimateIndex)
        {
            case UltimateIndex.None when Game1.player.professions.Any(p => p is >= 26 and < 30):
                Log.W($"{Game1.player.Name} is eligible for an Ultimate but is not currently registered to any. A default one will be chosen.");
                ultimateIndex = (UltimateIndex)Game1.player.professions.First(p => p is >= 26 and < 30);
                Log.W($"{Game1.player.Name}'s Ultimate was set to {ultimateIndex}.");

                break;

            case > UltimateIndex.None when !Game1.player.professions.Contains((int)ultimateIndex):
                Log.W($"Missing corresponding profession for {ultimateIndex} Ultimate. Resetting to a default value.");
                if (Game1.player.professions.Any(p => p is >= 26 and < 30))
                    ultimateIndex = (UltimateIndex)Game1.player.professions.First(p => p is >= 26 and < 30);
                else
                    ultimateIndex = UltimateIndex.None;

                break;
        }

        if (ultimateIndex > UltimateIndex.None)
            Game1.player.set_Ultimate(Ultimate.FromIndex(ultimateIndex));

        // revalidate levels
        Game1.player.RevalidateLevels();

        // revalidate fish pond populations
        Game1.getFarm().buildings.OfType<FishPond>()
            .Where(p => (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                        !p.isUnderConstruction()).ForEach(p => p.UpdateMaximumOccupancy());

        // prepare to check for prestige achievement
        Manager.Enable<PrestigeAchievementOneSecondUpdateTickedEvent>();
    }
}