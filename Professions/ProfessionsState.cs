namespace DaLion.Professions;

#region using directives

using System.Collections.Generic;
using DaLion.Professions.Framework.Events.GameLoop.TimeChanged;
using DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;
using DaLion.Professions.Framework.Events.Player.Warped;
using DaLion.Professions.Framework.Hunting;
using DaLion.Professions.Framework.Limits;
using DaLion.Professions.Framework.UI;
using DaLion.Shared.Extensions;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;

#endregion using directives

internal sealed class ProfessionsState
{
    internal List<int> OrderedProfessions
    {
        get
        {
            if (field is not null)
            {
                return field;
            }

            var player = Game1.player;
            var storedProfessions = Data.Read(player, DataKeys.OrderedProfessions);
            if (string.IsNullOrEmpty(storedProfessions))
            {
                Data.Write(player, DataKeys.OrderedProfessions, string.Join(',', player.professions));
                field = [.. player.professions];
            }
            else
            {
                var professionsList = storedProfessions.ParseList<int>();
                if (professionsList.Count != player.professions.Count || !professionsList.All(player.professions.Contains))
                {
                    Log.W(
                        $"Player {player.Name}'s professions does not match the stored list of professions. The stored professions will be reset.");
                    Data.Write(player, DataKeys.OrderedProfessions, string.Join(',', player.professions));
                    field = [.. player.professions];
                }
                else
                {
                    field = professionsList;
                }
            }

            return field;
        }
    }

    internal LimitBreak? LimitBreak
    {
        get;
        set
        {
            if (value is null)
            {
                field = null;
                Data.Write(Game1.player, DataKeys.LimitBreakId, null);
                EventManager.DisableWithAttribute<LimitEventAttribute>();
                Log.I($"{Game1.player.Name}'s Limit Break was removed.");
                return;
            }

            field = value;
            Data.Write(Game1.player, DataKeys.LimitBreakId, value.Id.ToString());
            if (Config.Masteries.EnableLimitBreaks)
            {
                EventManager.Enable<LimitWarpedEvent>();
            }

            Log.I($"{Game1.player.Name}'s LimitBreak was set to {value}.");
        }
    }

    internal ProspectorHunt? ProspectorHunt
    {
        get;
        set
        {
            if (value is null && this.ScavengerHunt is null)
            {
                EventManager.Disable<TreasureHuntPoolTrackerTimeChangedEvent>();
            }
            else
            {
                EventManager.Enable<TreasureHuntPoolTrackerTimeChangedEvent>();
            }

            field = value;
        }
    }

    internal ScavengerHunt? ScavengerHunt
    {
        get;
        set
        {
            if (value is null && this.ProspectorHunt is null)
            {
                EventManager.Disable<TreasureHuntPoolTrackerTimeChangedEvent>();
            }
            else
            {
                EventManager.Enable<TreasureHuntPoolTrackerTimeChangedEvent>();
            }

            field = value;
        }
    }

    internal Dictionary<string, int> EcologistBuffsLookup
    {
        get
        {
            field ??= Data
                    .Read(Game1.player, DataKeys.PrestigedEcologistBuffLookup)
                    .ParseDictionary<string, int>();
            return field;
        }
    }

    internal int SpelunkerLadderStreak { get; set; }

    internal int SpelunkerClusterStreak { get; set; }

    internal Point? SpelunkerLastStoneDestroyedAt { get; set; }

    internal List<(string ItemId, double ChanceToRecover)> SpelunkerUncollectedItems { get; } = [];

    internal SObject? SpelunkerFlag { get; set; }

    internal int SpelunkerFlagLevel { get; set; }

    internal bool HasSpelunkerRevivedAtCheckpointToday { get; set; }

    internal bool UsingSpelunkerCheckpoint { get; set; }

    internal int DemolitionistAdrenaline { get; set; }

    internal bool IsManualDetonationModeEnabled { get; set; }

    internal List<ChainedExplosion> ChainedExplosions { get; } = [];

    internal int FishingChain
    {
        get;
        set
        {
            field = value;
            if (value > 0)
            {
                EventManager.Enable<AnglerWarpedEvent>();
            }
            else
            {
                EventManager.Disable<AnglerWarpedEvent>();
            }
        }
    }

    internal int BruteRageCounter
    {
        get;
        set
        {
            field = value switch
            {
                >= 100 => 100,
                <= 0 => 0,
                _ => value,
            };
        }
    }

    internal Monster? LastDesperadoTarget
    {
        get;
        set
        {
            field = value;
            if (value is not null)
            {
                EventManager.Enable<DesperadoQuickshotUpdateTickedEvent>();
            }
        }
    }

    internal int SlimeFluteCooldown { get; set; }

    internal float SlimeFluteAddedScale { get; set; }

    internal CraftingRecipe? TapperCraftingRecipeBeingHovered { get; set; }

    internal List<KeyValuePair<string, int>> OriginalRecipeList { get; set; } = [];

    internal Point TapperCraftingMenuCursorLockPosition { get; set; }

    internal int TapperCraftingIngredientSelected
    {
        get;
        set
        {
            if (this.TapperCraftingRecipeBeingHovered is null)
            {
                field = 0;
                return;
            }

            var newValue = Math.Clamp(value, 0, this.TapperCraftingRecipeBeingHovered.recipeList.Count - 1);
            if (field != newValue)
            {
                this.TapperCraftingRecipeBeingHovered.ResetCraftingRecipe();
                field = newValue;
                Game1.playSound("smallSelect");
                this.TapperCraftingRecipeBeingHovered.AlterCraftingRecipeForTapper();
            }
        }
    }

    internal Queue<ISkill> SkillsToReset { get; } = [];

    internal MasteryWarningBox? WarningBox { get; set; }

    internal SiloMenuWrapper? MenuWrapper { get; set; }
}
