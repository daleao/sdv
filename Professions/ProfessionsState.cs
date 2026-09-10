namespace DaLion.Professions;

#region using directives

using System.Collections.Generic;
using DaLion.Professions.Framework.Events.GameLoop.TimeChanged;
using DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;
using DaLion.Professions.Framework.Events.Player.Warped;
using DaLion.Professions.Framework.Hunting;
using DaLion.Professions.Framework.Integrations;
using DaLion.Professions.Framework.Limits;
using DaLion.Professions.Framework.UI;
using DaLion.Shared.Extensions;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
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

    internal uint StepsTakenUntilPreviousTimeChange { get; set; }

    internal uint ItemsForagedUntilPreviousTimeChange { get; set; }

    internal uint TreesChoppedUntilPreviousTimeChange { get; set; }

    internal uint RocksCrushedUntilPreviousTimeChange { get; set; }

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

    internal List<int> TapperValidIngredientsForSubstitution { get; } = [];

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

            var valid = this.TapperValidIngredientsForSubstitution;
            if (valid.Count == 0)
            {
                field = 0;
                return;
            }
            else if (valid.Count == 1)
            {
                field = valid[0];
                return;
            }

            var currentIndex = valid.IndexOf(field);
            if (currentIndex < 0)
            {
                field = valid[0];
                return;
            }

            if (value > field)
            {
                currentIndex = Math.Min(currentIndex + 1, valid.Count - 1);
            }
            else if (value < field)
            {
                currentIndex = Math.Max(currentIndex - 1, 0);
            }

            var newValue = valid[currentIndex];
            if (field == newValue)
            {
                return;
            }

            this.TapperCraftingRecipeBeingHovered.ResetTapperCraftingRecipe();
            field = newValue;
            Log.D($"Trapper selected ingredient {field}.");
            Game1.playSound("smallSelect");
            this.TapperCraftingRecipeBeingHovered.AlterCraftingRecipeForTapper();
        }
    }

    internal CraftingRecipe? LuremasterCraftingRecipeBeingHovered { get; set; }

    internal List<int> LuremasterValidIngredientsForSubstitution { get; } = [];

    internal int LuremasterCraftingIngredientSelected
    {
        get;
        set
        {
            if (this.LuremasterCraftingRecipeBeingHovered is null)
            {
                field = 0;
                return;
            }

            var valid = this.LuremasterValidIngredientsForSubstitution;
            if (valid.Count == 0)
            {
                field = 0;
                return;
            }
            else if (valid.Count == 1)
            {
                field = valid[0];
                return;
            }

            var currentIndex = valid.IndexOf(field);
            if (currentIndex < 0)
            {
                field = valid[0];
                return;
            }

            if (value > field)
            {
                if (value - field >= 10)
                {
                    currentIndex = Math.Min(currentIndex + 10, valid.Count - 1);
                }

                currentIndex = Math.Min(currentIndex + 1, valid.Count - 1);
            }
            else if (value < field)
            {
                currentIndex = Math.Max(currentIndex - 1, 0);
            }

            var newValue = valid[currentIndex];
            if (field == newValue)
            {
                return;
            }

            field = newValue;
            Log.D($"Luremaster selected ingredient {field}.");
            Game1.playSound("smallSelect");
            var inventoryMenu = BetterCraftingIntegration.Instance?.Menu is not null
                ? BetterCraftingIntegration.Instance.GetInventoryMenu()
                : ((CraftingPage)((GameMenu)Game1.activeClickableMenu).GetCurrentPage()).inventory;
            this.LuremasterCraftingRecipeBeingHovered.AlterCraftingRecipeForLuremaster((SObject)inventoryMenu.actualInventory[newValue]);
        }
    }

    internal bool IsLuremasterUsingCursorInput { get; set; }

    internal List<KeyValuePair<string, int>> OriginalRecipeList { get; set; } = [];

    internal int OriginalQuantityPerCraft { get; set; }

    internal Point CraftingMenuCursorLockPosition { get; set; }

    internal Queue<ISkill> SkillsToReset { get; } = [];

    internal MasteryWarningBox? WarningBox { get; set; }

    internal SiloMenuWrapper? MenuWrapper { get; set; }
}
