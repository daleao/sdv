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
using DaLion.Shared.Extensions.Collections;
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
            var player = Game1.player;
            if (value is null)
            {
                field = null;
                Data.Write(player, DataKeys.LimitBreakId, null);
                Events.DisableWithAttribute<LimitEventAttribute>();
                Log.D($"{player.Name}'s Limit Break was removed.");
                return;
            }

            field = value;
            Data.Write(player, DataKeys.LimitBreakId, value.Id.ToString());
            if (Config.Masteries.EnableLimitBreaks)
            {
                Events.Enable<LimitWarpedEvent>();
            }

            Log.D($"{player.Name}'s LimitBreak was set to {value}.");
        }
    }

    internal ProspectorHunt? ProspectorHunt
    {
        get;
        set
        {
            if (value is null && this.ScavengerHunt is null)
            {
                Events.Disable<TreasureHuntPoolTrackerTimeChangedEvent>();
            }
            else
            {
                Events.Enable<TreasureHuntPoolTrackerTimeChangedEvent>();
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
                Events.Disable<TreasureHuntPoolTrackerTimeChangedEvent>();
            }
            else
            {
                Events.Enable<TreasureHuntPoolTrackerTimeChangedEvent>();
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
                Events.Enable<AnglerWarpedEvent>();
            }
            else
            {
                Events.Disable<AnglerWarpedEvent>();
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
                Events.Enable<DesperadoQuickshotUpdateTickedEvent>();
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

            var index = valid.IndexOf(field);
            if (index < 0)
            {
                field = valid[0];
                return;
            }

            if (value > field)
            {
                index = Math.Min(index + 1, valid.Count - 1);
            }
            else if (value < field)
            {
                index = Math.Max(index - 1, 0);
            }

            var validValue = valid[index];
            if (field == validValue)
            {
                return;
            }

            this.TapperCraftingRecipeBeingHovered.ResetTapperCraftingRecipe();
            field = validValue;
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

            var index = valid.IndexOf(field);
            if (index < 0)
            {
                field = value > field ? valid[^1] : valid[0];
                return;
            }

            var validValue = field;
            if (value > field)
            {
                validValue = value - field >= 10
                    ? FindVerticalSelection(field, 1) // scroll down
                    : valid[Math.Min(index + 1, valid.Count - 1)]; // scroll right
            }
            else if (value < field)
            {
                validValue = field - value >= 10
                    ? FindVerticalSelection(field, -1) // scroll up
                    : valid[Math.Max(index - 1, 0)]; // scroll left
            }

            if (field == validValue)
            {
                return;
            }

            field = validValue;
            Game1.playSound("smallSelect");
            var inventoryMenu = BetterCraftingIntegration.Instance?.Menu is not null
                ? BetterCraftingIntegration.Instance.GetInventoryMenu()
                : ((CraftingPage)((GameMenu)Game1.activeClickableMenu).GetCurrentPage()).inventory;
            this.LuremasterCraftingRecipeBeingHovered.AlterCraftingRecipeForLuremaster((SObject)inventoryMenu.actualInventory[validValue]);
        }
    }

    internal bool IsLuremasterUsingCursorInput { get; set; }

    internal List<KeyValuePair<string, int>> OriginalRecipeList { get; set; } = [];

    internal int OriginalQuantityPerCraft { get; set; }

    internal Point CraftingMenuCursorLockPosition { get; set; }

    internal Queue<ISkill> SkillsToReset { get; } = [];

    internal MasteryWarningBox? WarningBox { get; set; }

    internal SiloMenuWrapper? MenuWrapper { get; set; }

    internal int GlobalBiodiversityFactor { get; set; }

    private static int FindVerticalSelection(int selected, int direction)
    {
        const int columns = 12;

        var row = selected / columns;
        var column = selected % columns;
        for (var targetRow = row + direction;
            targetRow >= 0 && targetRow < 3;
            targetRow += direction)
        {
            var rowStart = targetRow * columns;
            var rowEnd = rowStart + columns;

            var candidates = State.LuremasterValidIngredientsForSubstitution
                .Where(i => i >= rowStart && i < rowEnd);
            if (candidates.None())
            {
                continue;
            }

            return candidates
                .OrderBy(i => Math.Abs((i % columns) - column))
                .First();
        }

        return direction < 0
        ? State.LuremasterValidIngredientsForSubstitution.First()
        : State.LuremasterValidIngredientsForSubstitution.Last();
    }
}
