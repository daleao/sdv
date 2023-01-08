namespace DaLion.Overhaul.Modules.Arsenal.Events.Weapons;

#region using directives

using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ComboButtonPressedEvent : ButtonPressedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ComboButtonPressedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ComboButtonPressedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Context.IsWorldReady && Game1.activeClickableMenu is null && ArsenalModule.Config.Weapons.EnableComboHits;

    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        var player = Game1.player;
        if (!e.Button.IsUseToolButton() || player.CurrentTool is not MeleeWeapon weapon || weapon.isScythe())
        {
            return;
        }

        ArsenalModule.State.HoldingWeaponSwing = true;
        this.Manager.Enable<ComboButtonReleasedEvent>();

        var hitStep = ArsenalModule.State.ComboHitQueued;
        if (hitStep == ComboHitStep.Idle)
        {
            return;
        }

        var finalHitStep = weapon.GetFinalHitStep();
        if (hitStep >= finalHitStep)
        {
            ModHelper.Input.Suppress(e.Button);
            return;
        }

        if (!ArsenalModule.State.FarmerAnimating)
        {
            return;
        }

        ModHelper.Input.Suppress(e.Button);

        if (weapon.IsClub() && hitStep == finalHitStep - 1)
        {
            player.QueueSmash(weapon);
        }
        else if ((int)hitStep % 2 == 0)
        {
            player.QueueForwardSwipe(weapon);
        }
        else
        {
            player.QueueReverseSwipe(weapon);
        }
    }
}
