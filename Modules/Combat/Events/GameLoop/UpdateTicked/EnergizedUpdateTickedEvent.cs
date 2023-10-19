namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class EnergizedUpdateTickedEvent : UpdateTickedEvent
{
    private WeakReference<BaseEnchantment>? _instance;

    /// <summary>Initializes a new instance of the <see cref="EnergizedUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal EnergizedUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        switch (Game1.player.CurrentTool)
        {
            case MeleeWeapon weapon when weapon.GetEnchantmentOfType<EnergizedEnchantment>() is { } energized:
                this._instance = new WeakReference<BaseEnchantment>(energized, false);
                break;
            case Slingshot slingshot when slingshot.GetEnchantmentOfType<RangedEnergizedEnchantment>() is { } energized:
                this._instance = new WeakReference<BaseEnchantment>(energized, false);
                break;
            default:
                this.Disable();
                break;
        }
    }

    /// <inheritdoc />
    protected override void OnDisabled()
    {
        this._instance = null;
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (this._instance is null || !this._instance.TryGetTarget(out var instance))
        {
            this.Disable();
            return;
        }

        switch (instance)
        {
            case EnergizedEnchantment energized:
                energized.Update(e.Ticks);
                break;
            case RangedEnergizedEnchantment energized:
                energized.Update(e.Ticks);
                break;
        }
    }
}
