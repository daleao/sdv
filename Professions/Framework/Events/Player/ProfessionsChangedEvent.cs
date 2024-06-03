namespace DaLion.Professions.Framework.Events.Player;

#region using directives

using System;
using DaLion.Professions;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.SMAPI;

#endregion using directives

/// <summary>Monitors changes to <see cref="Farmer.professions"/>.</summary>
internal sealed class ProfessionsChangedEvent : ManagedEvent
{
    private readonly Farmer _who;

    /// <summary>Initializes a new instance of the <see cref="ProfessionsChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    /// <param name="who">The <see cref="Farmer"/> to monitor.</param>
    internal ProfessionsChangedEvent(EventManager manager, Farmer who)
        : base(manager)
    {
        who.professions.OnValueAdded += this.OnProfessionAdded;
        who.professions.OnValueRemoved += this.OnProfessionRemoved;
        this._who = who;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this._who.professions.OnValueAdded -= this.OnProfessionAdded;
        this._who.professions.OnValueRemoved -= this.OnProfessionRemoved;
        GC.SuppressFinalize(this);
    }

    /// <summary>Invoked when a profession is added to the local player.</summary>
    /// <param name="added">The index of the added profession.</param>
    private void OnProfessionAdded(int added)
    {
        if (Profession.TryFromValue(added, out var profession))
        {
            profession.OnAdded(Game1.player);
        }
        else if (Profession.TryFromValue(added - 100, out profession))
        {
            profession.OnAdded(Game1.player, true);
        }

        if (added.IsIn(Profession.GetRange(true)))
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
        }
    }

    /// <summary>Invoked when a profession is removed from the local player.</summary>
    /// <param name="removed">The index of the removed profession.</param>
    private void OnProfessionRemoved(int removed)
    {
        if (Profession.TryFromValue(removed, out var profession))
        {
            profession.OnRemoved(Game1.player);
        }
        else if (Profession.TryFromValue(removed - 100, out profession))
        {
            profession.OnRemoved(Game1.player, true);
        }

        if (removed.IsIn(Profession.GetRange(true)))
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
        }
    }
}
