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
using StardewValley;
using StardewValley.Monsters;

#endregion using directives

internal sealed class ProfessionsState
{
    private int _rageCounter;
    private Monster? _lastDesperadoTarget;
    private LimitBreak? _limitBreak;
    private ProspectorHunt? _prospectorHunt;
    private ScavengerHunt? _scavengerHunt;

    internal int SpelunkerLadderStreak { get; set; }

    internal List<Item> SpelunkerUncollectedItems { get; set; } = [];

    internal int DemolitionistExcitedness { get; set; }

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

    internal HashSet<GreenSlime> AllySlimes { get; } = [];

    internal Queue<ISkill> SkillsToReset { get; } = [];

    internal bool UsedStatueToday { get; set; }

    internal List<int> OrderedProfessions { get; set; } = [];

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
            Data.Write(Game1.player, DataKeys.LimitBreakId, value.Name);
            if (Config.Masteries.EnableLimitBreaks)
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

    internal Dictionary<string, int> PrestigedEcologistBuffsLookup { get; set; } = [];
}
