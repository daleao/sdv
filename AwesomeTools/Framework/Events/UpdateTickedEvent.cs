using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

namespace TheLion.Stardew.Tools.Framework.Events;

internal class UpdateTickedEvent : IEvent
{
    /// <inheritdoc />
    public void Hook()
    {
        ModEntry.ModHelper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
    }

    /// <inheritdoc />
    public void Unhook()
    {
        ModEntry.ModHelper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
    }

    /// <summary>Raised after the game state is updated.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        Farmer who = Game1.player;
        if (who.FarmerSprite.isOnToolAnimation()) return;

        if (ModEntry.Config.ShockwaveDelay > 0 && !e.IsMultipleOf(ModEntry.Config.ShockwaveDelay)) return;

        Tool tool = who.CurrentTool;
        Vector2 actionTile = new((int)(who.GetToolLocation().X / Game1.tileSize), (int)(who.GetToolLocation().Y / Game1.tileSize));
        bool doneShockwave = false;
        switch (tool)
        {
            case Axe:
                who.Stamina -= who.toolPower - who.ForagingLevel * 0.1f * (who.toolPower - 1) * ModEntry.Config.StaminaCostMultiplier;
                doneShockwave = ModEntry.AxeFx.DoShockwave(tool, actionTile, who);
                break;

            case Pickaxe:
                who.Stamina -= who.toolPower - who.MiningLevel * 0.1f * (who.toolPower - 1) * ModEntry.Config.StaminaCostMultiplier;
                doneShockwave = ModEntry.PickaxeFx.DoShockwave(tool, actionTile, who);
                break;
        }

        if (doneShockwave) Unhook();
    }
}