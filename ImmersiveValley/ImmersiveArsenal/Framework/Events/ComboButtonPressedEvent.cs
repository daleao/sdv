namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using DaLion.Common;
using DaLion.Common.Enums;
using DaLion.Common.Events;
using DaLion.Common.Extensions.Stardew;
using DaLion.Stardew.Arsenal.Extensions;
using StardewModdingAPI.Events;
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
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        var player = Game1.player;
        if (!e.Button.IsUseToolButton() || player.CurrentTool is not MeleeWeapon weapon || !player.UsingTool ||
            ModEntry.State.ComboHitStep >= weapon.GetFinalComboHitStep())
        {
            return;
        }

        ++ModEntry.State.ComboHitStep;
        ModEntry.ModHelper.Input.Suppress(e.Button);
    }
}
