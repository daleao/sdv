#nullable enable
namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Extensions;
using Framework.Ultimate;

#endregion using directives

[UsedImplicitly]
internal sealed class StaticSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Construct an instance.</summary>
    internal StaticSaveLoadedEvent()
    {
        Enable();
    }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object sender, SaveLoadedEventArgs e)
    {
        // enable events
        EventManager.EnableAllForLocalPlayer();

        // load and initialize Ultimate index
        Log.T("Initializing Ultimate...");

        var superModeIndex = Game1.player.ReadDataAs(ModData.UltimateIndex, UltimateIndex.None);
        switch (superModeIndex)
        {
            case UltimateIndex.None when Game1.player.professions.Any(p => p is >= 26 and < 30):
                Log.W("Player eligible for Ultimate but not currently registered to any. Setting to a default value.");
                superModeIndex = (UltimateIndex) Game1.player.professions.First(p => p is >= 26 and < 30);
                Game1.player.WriteData(ModData.UltimateIndex, superModeIndex.ToString());

                break;

            case > UltimateIndex.None when !Game1.player.professions.Contains((int) superModeIndex):
                Log.W($"Missing corresponding profession for {superModeIndex} Ultimate. Resetting to a default value.");
                if (Game1.player.professions.Any(p => p is >= 26 and < 30))
                {
                    superModeIndex = (UltimateIndex) Game1.player.professions.First(p => p is >= 26 and < 30);
                    Game1.player.WriteData(ModData.UltimateIndex, superModeIndex.ToString());
                }
                else
                {
                    superModeIndex = UltimateIndex.None;
                    Game1.player.WriteData(ModData.UltimateIndex, null);
                }

                break;
        }

        if (superModeIndex > UltimateIndex.None)
        {
#pragma warning disable CS8509
            ModEntry.PlayerState.RegisteredUltimate = superModeIndex switch
#pragma warning restore CS8509
            {
                UltimateIndex.BruteFrenzy => new UndyingFrenzy(),
                UltimateIndex.PoacherAmbush => new Ambush(),
                UltimateIndex.PiperPandemic => new Enthrall(),
                UltimateIndex.DesperadoBlossom => new DeathBlossom()
            };
        }

        // revalidate levels
        Game1.player.RevalidateLevels();

        // prepare to check for prestige achievement
        EventManager.Enable(typeof(PrestigeAchievementOneSecondUpdateTickedEvent));
    }
}