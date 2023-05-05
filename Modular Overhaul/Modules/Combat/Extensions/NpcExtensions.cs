namespace DaLion.Overhaul.Modules.Combat.Extensions;

#region using direectives

using System.Linq;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Extensions for the <see cref="NPC"/> class.</summary>
internal static class NpcExtensions
{
    internal static void OnRemoved(this NPC npc)
    {
        if (npc is not Monster { Health: > 0 } alive)
        {
            return;
        }

        if (Game1.getAllFarmers().Any(farmer => farmer.currentLocation == alive.currentLocation))
        {
            return;
        }

        Log.D($"{alive.Name} was removed before it was dead. Re-adding...");
        alive.currentLocation.characters.Add(alive);
    }
}
