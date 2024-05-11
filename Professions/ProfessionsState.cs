namespace DaLion.Professions;

#region using directives

using System.Collections.Generic;
using DaLion.Professions.Framework.Events.Display.RenderedHud;
using DaLion.Professions.Framework.Events.GameLoop.OneSecondUpdateTicked;
using DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;
using DaLion.Professions.Framework.Events.Input.ButtonsChanged;
using DaLion.Professions.Framework.Events.Player.Warped;
using DaLion.Professions.Framework.Limits;
using DaLion.Professions.Framework.TreasureHunts;
using DaLion.Shared.Extensions;
using StardewValley;
using StardewValley.Monsters;

#endregion using directives

internal sealed class ProfessionsState
{
    private int _rageCounter;
    private List<int>? _orderedProfessions;
    private Monster? _lastDesperadoTarget;
    private LimitBreak? _limitBreak;
    private ProspectorHunt? _prospectorHunt;
    private ScavengerHunt? _scavengerHunt;
    private Dictionary<string, int>? _prestigedEcologistBuffsLookup;

    internal int SpelunkerLadderStreak { get; set; }

    internal List<string> SpelunkerUncollectedItems { get; set; } = [];

    internal int DemolitionistAdrenaline { get; set; }

    internal List<ChainedExplosion> ChainedExplosions { get; } = [];

    internal int BruteRageCounter
    {
        get => this._rageCounter;
        set
        {
            this._rageCounter = value switch
            {
                >= 100 => 100,
                <= 0 => 0,
                _ => value,
            };
        }
    }

    internal Monster? LastDesperadoTarget
    {
        get => this._lastDesperadoTarget;
        set
        {
            this._lastDesperadoTarget = value;
            if (value is not null)
            {
                EventManager.Enable<DesperadoQuickshotUpdateTickedEvent>();
            }
        }
    }

    internal List<PipedSlime> SummonedSlimes { get; } = [];

    internal Queue<ISkill> SkillsToReset { get; } = [];

    internal bool UsedStatueToday { get; set; }

    internal List<int> OrderedProfessions
    {
        get
        {
            if (this._orderedProfessions is not null)
            {
                return this._orderedProfessions;
            }

            var player = Game1.player;
            var storedProfessions = Data.Read(player, DataKeys.OrderedProfessions);
            if (string.IsNullOrEmpty(storedProfessions))
            {
                Data.Write(player, DataKeys.OrderedProfessions, string.Join(',', player.professions));
                this._orderedProfessions = [.. player.professions];
            }
            else
            {
                var professionsList = storedProfessions.ParseList<int>();
                if (professionsList.Count != player.professions.Count || !professionsList.All(player.professions.Contains))
                {
                    Log.W(
                        $"Player {player.Name}'s professions does not match the stored list of professions. The stored professions will be reset.");
                    Data.Write(player, DataKeys.OrderedProfessions, string.Join(',', player.professions));
                    this._orderedProfessions = [.. player.professions];
                }
                else
                {
                    this._orderedProfessions = professionsList;
                }
            }

            return this._orderedProfessions;
        }
    }

    internal LimitBreak? LimitBreak
    {
        get => this._limitBreak;
        set
        {
            if (value is null)
            {
                this._limitBreak = null;
                Data.Write(Game1.player, DataKeys.LimitBreakId, null);
                EventManager.DisableWithAttribute<LimitEventAttribute>();
                Log.I($"{Game1.player.Name}'s Limit Break was removed.");
                return;
            }

            this._limitBreak = value;
            Data.Write(Game1.player, DataKeys.LimitBreakId, value.Id.ToString());
            if (Config.Masteries.UnlockLimitBreaks)
            {
                EventManager.Enable<LimitWarpedEvent>();
            }

            Log.I($"{Game1.player.Name}'s LimitBreak was set to {value}.");
        }
    }

    internal ProspectorHunt? ProspectorHunt
    {
        get => this._prospectorHunt;
        set
        {
            if (value is null)
            {
                EventManager.Disable(
                    typeof(ProspectorHuntOneSecondUpdateTickedEvent),
                    typeof(ProspectorRenderedHudEvent));
                if (!Game1.player.HasProfession(Profession.Scavenger))
                {
                    EventManager.Disable<TrackerButtonsChangedEvent>();
                }
            }
            else
            {
                EventManager.Enable(
                    typeof(ProspectorHuntOneSecondUpdateTickedEvent),
                    typeof(ProspectorRenderedHudEvent),
                    typeof(TrackerButtonsChangedEvent));
            }

            this._prospectorHunt = value;
        }
    }

    internal ScavengerHunt? ScavengerHunt
    {
        get => this._scavengerHunt;
        set
        {
            if (value is null)
            {
                EventManager.Disable(
                    typeof(ScavengerHuntOneSecondUpdateTickedEvent),
                    typeof(ScavengerRenderedHudEvent));
                if (!Game1.player.HasProfession(Profession.Prospector))
                {
                    EventManager.Disable<TrackerButtonsChangedEvent>();
                }
            }
            else
            {
                EventManager.Enable(
                    typeof(ScavengerHuntOneSecondUpdateTickedEvent),
                    typeof(ScavengerRenderedHudEvent),
                    typeof(TrackerButtonsChangedEvent));
            }

            this._scavengerHunt = value;
        }
    }

    internal Dictionary<string, int> PrestigedEcologistBuffsLookup
    {
        get
        {
            this._prestigedEcologistBuffsLookup ??= Data
                    .Read(Game1.player, DataKeys.PrestigedEcologistBuffLookup)
                    .ParseDictionary<string, int>();
            return this._prestigedEcologistBuffsLookup;
        }
    }
}
