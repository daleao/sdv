namespace DaLion.Core.Framework.Events;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class JinxedUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="JinxedUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal JinxedUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.hasBuff(BuffIDs.Jinxed);

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        MeleeWeapon.clubCooldown = MeleeWeapon.clubCooldownTime;
        MeleeWeapon.daggerCooldown = MeleeWeapon.daggerCooldownTime;
        MeleeWeapon.defenseCooldown = MeleeWeapon.defenseCooldownTime;
    }
}
