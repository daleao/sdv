namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop.SaveLoaded;

#region using directives

using System.Collections.Generic;
using DaLion.Overhaul.Modules.Professions.Events.Display.RenderedHud;
using DaLion.Overhaul.Modules.Professions.Events.GameLoop.DayStarted;
using DaLion.Overhaul.Modules.Professions.Events.GameLoop.OneSecondUpdateTicked;
using DaLion.Overhaul.Modules.Professions.Events.GameLoop.TimeChanged;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Comparers;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.SMAPI;
using Netcode;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class ProfessionSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ProfessionSaveLoadedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ProfessionSaveLoadedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        var player = Game1.player;
        player.professions.OnArrayReplaced += this.OnArrayReplaced;
        player.professions.OnElementChanged += this.OnElementChanged;

        Skill.List.ForEach(s => s.Revalidate());
        if (ProfessionsModule.Config.EnableLimitBreaks)
        {
            player.RevalidateUltimate();
        }

        Game1.game1.RevalidateFishPondPopulations();

        if (Context.IsMainPlayer)
        {
            if (Game1.game1.DoesAnyPlayerHaveProfession(Profession.Luremaster, out _))
            {
                this.Manager.Enable<LuremasterTimeChangedEvent>();
            }
            else if (Context.IsMultiplayer)
            {
                this.Manager.Enable<MonitorLuremastersDayStartedEvent>();
            }
        }

        if (player.HasProfession(Profession.Prospector))
        {
            this.Manager.Enable<ProspectorRenderedHudEvent>();
        }

        if (player.HasProfession(Profession.Scavenger))
        {
            this.Manager.Enable<ScavengerRenderedHudEvent>();
        }

        this.Manager.Enable<PrestigeAchievementOneSecondUpdateTickedEvent>();
    }

    /// <summary>Invoked when the value list is replaced.</summary>
    /// <param name="list">The net field whose values changed.</param>
    /// <param name="oldValues">The previous list of values.</param>
    /// <param name="newValues">The new list of values.</param>
    private void OnArrayReplaced(NetList<int, NetInt> list, IList<int> oldValues, IList<int> newValues)
    {
        var oldSet = new HashSet<int>(oldValues, new EquatableComparer<int>());
        var changed = new HashSet<int>(newValues, new EquatableComparer<int>());

        foreach (var value in oldSet)
        {
            if (!changed.Contains(value))
            {
                this.OnProfessionRemoved(value);
            }
        }

        foreach (var value in changed)
        {
            if (!oldSet.Contains(value))
            {
                this.OnProfessionAdded(value);
            }
        }
    }

    /// <summary>Invoked when an entry is replaced.</summary>
    /// <param name="list">The net field whose values changed.</param>
    /// <param name="index">The list index which changed.</param>
    /// <param name="oldValue">The previous value.</param>
    /// <param name="newValue">The new value.</param>
    private void OnElementChanged(NetList<int, NetInt> list, int index, int oldValue, int newValue)
    {
        this.OnProfessionRemoved(oldValue);
        this.OnProfessionAdded(newValue);
    }

    /// <summary>Invoked when a profession is added to the local player.</summary>
    /// <param name="which">The index of the added profession.</param>
    private void OnProfessionAdded(int which)
    {
        if (which.IsIn(Profession.GetRange(true)))
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
        }
    }

    /// <summary>Invoked when a profession is removed from the local player.</summary>
    /// <param name="which">The index of the removed profession.</param>
    private void OnProfessionRemoved(int which)
    {
        if (which.IsIn(Profession.GetRange(true)))
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
        }
    }
}
