namespace DaLion.Shared.Integrations.GMCM;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Menus;

#endregion using directives

/// <summary>Draws the hover text over a <see cref="DynamicKeyValuePairListOption"/>.</summary>
[UsedImplicitly]
public sealed class DynamicKeyValuePairListOptionRenderedActiveMenuEvent : RenderedActiveMenuEvent
{
    /// <summary>Initializes a new instance of the <see cref="DynamicKeyValuePairListOptionRenderedActiveMenuEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    public DynamicKeyValuePairListOptionRenderedActiveMenuEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => DynamicKeyValuePairListOption.Tooltip is not null;

    /// <inheritdoc />
    protected override void OnRenderedActiveMenuImpl(object? sender, RenderedActiveMenuEventArgs e)
    {
        IClickableMenu.drawHoverText(e.SpriteBatch, DynamicKeyValuePairListOption.Tooltip, Game1.smallFont);
    }
}
