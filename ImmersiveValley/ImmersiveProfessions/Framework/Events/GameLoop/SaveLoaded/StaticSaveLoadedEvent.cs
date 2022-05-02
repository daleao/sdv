namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Common.Extensions;
using Extensions;
using Framework.Ultimate;

#endregion using directives

[UsedImplicitly]
internal class StaticSaveLoadedEvent : SaveLoadedEvent
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

            // load
        var superModeIndex = Game1.player.ReadDataAs(DataField.UltimateIndex, UltimateIndex.None);

            // validate
        switch (superModeIndex)
        {
            case UltimateIndex.None when Game1.player.professions.Any(p => p is >= 26 and < 30):
                Log.W("Player eligible for Ultimate but not currently registered to any. Setting to a default value.");
                superModeIndex = (UltimateIndex) Game1.player.professions.First(p => p is >= 26 and < 30);
                Game1.player.WriteData(DataField.UltimateIndex, superModeIndex.ToString());

                break;

            case > UltimateIndex.None when !Game1.player.professions.Contains((int) superModeIndex):
                Log.W($"Missing corresponding profession for {superModeIndex} Ultimate. Resetting to a default value.");
                if (Game1.player.professions.Any(p => p is >= 26 and < 30))
                {
                    superModeIndex = (UltimateIndex) Game1.player.professions.First(p => p is >= 26 and < 30);
                    Game1.player.WriteData(DataField.UltimateIndex, superModeIndex.ToString());
                }
                else
                {
                    superModeIndex = UltimateIndex.None;
                    Game1.player.WriteData(DataField.UltimateIndex, null);
                }

                break;
        }

            // initialize
        if (superModeIndex > UltimateIndex.None)
        {
            ModEntry.PlayerState.RegisteredUltimate =
#pragma warning disable CS8509
                ModEntry.PlayerState.RegisteredUltimate = superModeIndex switch
#pragma warning restore CS8509
                {
                    UltimateIndex.Brute => new Frenzy(),
                    UltimateIndex.Poacher => new Ambush(),
                    UltimateIndex.Piper => new Pandemonia(),
                    UltimateIndex.Desperado => new DeathBlossom()
                };
        }

        // check for prestige achievements
        if (Game1.player.HasAllProfessions())
        {
            string name =
                ModEntry.ModHelper.Translation.Get("prestige.achievement.name." +
                                                   (Game1.player.IsMale ? "male" : "female"));
            if (!Game1.player.achievements.Contains(name.GetDeterministicHashCode()))
                EventManager.Enable(typeof(AchievementUnlockedDayStartedEvent));
        }
    }
}