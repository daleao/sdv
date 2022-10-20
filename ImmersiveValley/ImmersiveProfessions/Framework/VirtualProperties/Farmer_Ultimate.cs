namespace DaLion.Stardew.Professions.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Common.Extensions.Stardew;
using DaLion.Stardew.Professions.Extensions;
using DaLion.Stardew.Professions.Framework.Events.Display;
using DaLion.Stardew.Professions.Framework.Events.Player;
using DaLion.Stardew.Professions.Framework.Ultimates;
using Netcode;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Farmer_Ultimate
{
    internal static ConditionalWeakTable<Farmer, Holder> Values { get; } = new();

    internal static Ultimate? Get_Ultimate(this Farmer farmer)
    {
        return Values.GetValue(farmer, Create).Ultimate;
    }

    internal static void Set_Ultimate(this Farmer farmer, Ultimate? value)
    {
        farmer.Write(DataFields.UltimateIndex, value?.Index.ToString() ?? string.Empty);
        Values.AddOrUpdate(farmer, Create(farmer));
        Log.W($"{farmer.Name}'s Ultimate was set to {value}.");

        if (value is not null)
        {
            ModEntry.Events.Enable<UltimateWarpedEvent>();
            if (Game1.currentLocation.IsDungeon())
            {
                ModEntry.Events.Enable<UltimateMeterRenderingHudEvent>();
            }
        }
        else
        {
            ModEntry.Events.DisableWithAttribute<UltimateEventAttribute>();
        }
    }

    internal static NetInt Get_UltimateIndex(this Farmer farmer)
    {
        return Values.GetValue(farmer, Create).NetIndex;
    }

    // Net types are readonly
    internal static void Set_UltimateIndex(this Farmer farmer, NetInt value)
    {
    }

    private static Holder Create(Farmer farmer)
    {
        var holder = new Holder();
        var index = farmer.Read(DataFields.UltimateIndex, -1);
        holder.Ultimate = index < 0 ? null : Ultimate.FromValue(index);
        holder.NetIndex.Value = index;
        return holder;
    }

    internal class Holder
    {
        public NetInt NetIndex { get; } = new(-1);

        public Ultimate? Ultimate { get; internal set; }
    }
}
