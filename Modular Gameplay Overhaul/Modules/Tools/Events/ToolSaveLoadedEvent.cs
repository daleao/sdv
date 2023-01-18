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
        var slots = Game1.player.Read(DataFields.SelectableSlots).ParseList<int>();
        if (slots.Count == 0)
        {
            return;
        }

        var leftover = slots.ToList();
        for (var i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            if (slot < 0)
            {
                leftover.Remove(slot);
                continue;
            }

            var item = Game1.player.Items[slot];
            if (item is not (Tool tool and (Axe or Hoe or Pickaxe or WateringCan or FishingRod or MilkPail or Shears
                or MeleeWeapon)))
            {
                continue;
            }

            if (tool is MeleeWeapon weapon && !weapon.isScythe())
            {
                continue;
            }

            ToolsModule.State.SelectableTools.Add(tool);
            leftover.Remove(slot);
        }

        Game1.player.Write(DataFields.SelectableSlots, string.Join(',', leftover));
    }
}
