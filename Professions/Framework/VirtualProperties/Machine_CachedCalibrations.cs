namespace DaLion.Professions.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Machine_CachedCalibrations
{
    internal static ConditionalWeakTable<SObject, Dictionary<string, int>> Values { get; } = [];

    internal static Dictionary<string, int> Get_Calibrations(this SObject machine)
    {
        return Values.TryGetValue(machine, out var cached) ? cached : [];
    }

    internal static void Set_Calibrations(this SObject machine, Dictionary<string, int> calibrations)
    {
        Values.Remove(machine);
        Values.Add(machine, calibrations);
    }
}
