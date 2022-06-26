namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using Common.Events;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ArsenalUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArsenalUpdateTickedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (!ModEntry.Config.WeaponsCostStamina || Game1.player.CurrentTool is not Slingshot ||
            !Game1.player.usingSlingshot) return;

        if (e.IsMultipleOf(30)) Game1.player.Stamina -= (1 - Game1.player.CombatLevel * 0.05f);
    }
}