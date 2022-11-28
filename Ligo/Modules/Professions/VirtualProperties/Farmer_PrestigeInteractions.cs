namespace DaLion.Ligo.Modules.Professions.VirtualProperties;

#region using directives

using System.Collections.Generic;
using System.Runtime.CompilerServices;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Farmer_PrestigeInteractions
{
    internal static ConditionalWeakTable<Farmer, Holder> Values { get; } = new();

    internal static Queue<ISkill> Get_SkillsToReset(this Farmer farmer)
    {
        return Values.GetOrCreateValue(farmer).SkillsToReset;
    }

    internal static bool Get_HasSkillsToReset(this Farmer farmer)
    {
        return Values.TryGetValue(farmer, out var holder) && holder.SkillsToReset.Count > 0;
    }

    internal static bool Get_UsedStatueToday(this Farmer farmer)
    {
        return Values.GetOrCreateValue(farmer).UsedStatueToday;
    }

    internal static void Set_UsedStatueToday(this Farmer farmer, bool value)
    {
        Values.GetOrCreateValue(farmer).UsedStatueToday = value;
    }

    internal class Holder
    {
        public Queue<ISkill> SkillsToReset { get; } = new();

        public bool UsedStatueToday { get; internal set; }
    }
}
