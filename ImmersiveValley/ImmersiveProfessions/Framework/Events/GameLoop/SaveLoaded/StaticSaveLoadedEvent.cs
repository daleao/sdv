#nullable enable
namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Linq;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Events;
using StardewValley;

using Extensions;
using Framework.Ultimate;
using Utility;

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

        var superModeIndex = Game1.player.ReadDataAs(DataField.UltimateIndex, UltimateIndex.None);
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

        if (superModeIndex > UltimateIndex.None)
        {
#pragma warning disable CS8509
            ModEntry.PlayerState.RegisteredUltimate = superModeIndex switch
#pragma warning restore CS8509
            {
                UltimateIndex.BruteFrenzy => new Frenzy(),
                UltimateIndex.PoacherAmbush => new Ambush(),
                UltimateIndex.PiperPandemonium => new Pandemonium(),
                UltimateIndex.DesperadoBlossom => new DeathBlossom()
            };
        }

        // revalidate levels
        for (var skill = 0; skill <= 5; ++skill)
        {
            if (skill == 5 && ModEntry.LuckSkillApi is null)
            {
                Log.W(
                    $"Local player {Game1.player.Name} has gained Luck experience, but Luck Skill mod is not installed. The Luck skill will be reset.");
                Game1.player.ResetSkill(SkillType.Luck);
                continue;
            }

            var currentExp = Game1.player.experiencePoints[skill];
            var currentLevel = Game1.player.GetUnmodifiedSkillLevel(skill);
            var canGainPrestigeLevels = ModEntry.Config.EnablePrestige && Game1.player.HasAllProfessionsInSkill(skill);

            switch (currentLevel)
            {
                case >= 10 when !canGainPrestigeLevels:
                {
                    if (currentLevel > 10) Game1.player.SetSkillLevel((SkillType) skill, 10);
                    if (currentExp > Experience.VANILLA_CAP_I) Game1.player.experiencePoints[skill] = Experience.VANILLA_CAP_I;
                    break;
                }
                case >= 20:
                {
                    if (currentLevel > 20) Game1.player.SetSkillLevel((SkillType) skill, 20);
                    if (currentExp > Experience.PrestigeCap) Game1.player.experiencePoints[skill] = Experience.PrestigeCap;
                    break;
                }
                default:
                {
                    var expectedLevel = 0;
                    var i = 0;
                    while (i < 10 && currentExp > Experience.VanillaExpPerLevel[i++]) ++expectedLevel;

                    var remainingExp = currentExp - Experience.VANILLA_CAP_I;
                    if (canGainPrestigeLevels && remainingExp > 0)
                    {
                        i = 1;
                        while (i <= 10 && remainingExp >= ModEntry.Config.RequiredExpPerExtendedLevel * i++)
                            ++expectedLevel;
                    }

                    if (currentLevel != expectedLevel)
                    {
                        if (currentLevel < expectedLevel)
                            for (var level = currentLevel + 1; level <= expectedLevel; ++level)
                            {
                                var newOldLevel = new Point(skill, level);
                                if (!Game1.player.newLevels.Contains(newOldLevel))
                                    Game1.player.newLevels.Add(newOldLevel);
                            }

                        Game1.player.SetSkillLevel((SkillType) skill, expectedLevel);
                    }

                    currentLevel = expectedLevel;
                    Game1.player.experiencePoints[skill] = currentLevel switch
                    {
                        >= 10 when !canGainPrestigeLevels => Experience.VANILLA_CAP_I,
                        >= 20 => Experience.PrestigeCap,
                        _ => Game1.player.experiencePoints[skill]
                    };

                    break;
                }
            }
        }

        // prepare to check for prestige achievement
        EventManager.Enable(typeof(PrestigeAchievementOneSecondUpdateTickedEvent));
    }
}