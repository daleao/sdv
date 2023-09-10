namespace DaLion.Overhaul.Modules.Tools.Events;

#region using directives

using System.Linq;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ToolSaveLoadedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ToolSaveLoadedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => ToolsModule.Config.EnableAutoSelection;

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        var player = Game1.player;
        var indices = player.Read(DataKeys.SelectableTools).ParseList<int>();
        if (indices.Count == 0)
        {
            return;
        }

        var leftover = indices.ToList();
        for (var i = 0; i < indices.Count; i++)
        {
            var index = indices[i];
            if (index < 0 || index >= player.Items.Count)
            {
                leftover.Remove(index);
                continue;
            }

            var item = player.Items[index];
            if (item is not (Tool tool and (Axe or Hoe or Pickaxe or WateringCan or FishingRod or MilkPail or Shears
                or MeleeWeapon)))
            {
                continue;
            }

            if (tool is MeleeWeapon weapon && !weapon.isScythe())
            {
                continue;
            }

            ToolsModule.State.SelectableToolByType[tool.GetType()] = new SelectableTool(tool, index);
            leftover.Remove(index);
        }

        player.Write(DataKeys.SelectableTools, string.Join(',', leftover));
    }
}
