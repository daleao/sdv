namespace DaLion.Professions.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Shared.Extensions;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Machine_CachedCalibrations
{
    internal static ConditionalWeakTable<SObject, Dictionary<string, float>> Values { get; } = [];

    internal static Dictionary<string, float> Get_Calibrations(this SObject machine)
    {
        return Values.GetValue(machine, m => Data.Read(m, DataKeys.CalibrationPerItem).ParseDictionary<string, float>());
    }

    internal static void Set_Calibrations(this SObject machine, Dictionary<string, float> calibrations)
    {
        Values.Remove(machine);
        Values.Add(machine, calibrations);
    }
}
