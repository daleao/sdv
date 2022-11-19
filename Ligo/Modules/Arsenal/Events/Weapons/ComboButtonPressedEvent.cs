namespace DaLion.Ligo.Modules.Arsenal.Events;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Extensions;
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
    public override bool IsEnabled => ModEntry.Config.Arsenal.Weapons.AllowComboHits;

    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        var player = Game1.player;
        if (!Context.IsWorldReady || Game1.activeClickableMenu is not null || !e.Button.IsUseToolButton() ||
            player.CurrentTool is not MeleeWeapon weapon || weapon.isScythe())
        {
            return;
        }

        var hitStep = ModEntry.State.Arsenal.ComboHitStep;
        if (hitStep == ComboHitStep.Idle)
        {
            return;
        }

        if (hitStep >= weapon.GetFinalHitStep())
        {
            ModEntry.ModHelper.Input.Suppress(e.Button);
            return;
        }

        if (!ModEntry.State.Arsenal.IsFarmerAnimating)
        {
            return;
        }

        ModEntry.ModHelper.Input.Suppress(e.Button);

        var type = weapon.type.Value;
        switch (type)
        {
            case MeleeWeapon.club:
            {
                if ((int)ModEntry.State.Arsenal.ComboHitStep % 2 == 0)
                {
                    player.QueueForwardSwipe(weapon);
                }
                else
                {
                    player.QueueOverheadSwipe(weapon);
                }

                break;
            }

            case MeleeWeapon.stabbingSword or MeleeWeapon.defenseSword:
                if ((int)ModEntry.State.Arsenal.ComboHitStep % 2 == 0)
                {
                    player.QueueForwardSwipe(weapon);
                }
                else
                {
                    player.QueueBackwardSwipe(weapon);
                }

                break;
        }
    }
}
