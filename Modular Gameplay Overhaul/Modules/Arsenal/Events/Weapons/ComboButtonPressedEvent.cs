namespace DaLion.Overhaul.Modules.Arsenal.Events;

#region using directives

using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Shared.Events;
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

        var hitStep = ArsenalModule.State.ComboHitStep;
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

        var type = weapon.type.Value;
        if (type == MeleeWeapon.club && hitStep == finalHitStep - 1)
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
