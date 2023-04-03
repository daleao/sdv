#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented

namespace DaLion.Overhaul;

#region using directives

using System.Collections.Generic;
using Newtonsoft.Json;

#endregion using directives

/// <summary>Holds global data, independent from save files.</summary>
public sealed class ModData
{
    [JsonProperty]
    public bool InitialSetupComplete { get; internal set; }

    [JsonProperty] public Dictionary<string, bool> WeaponRevalidationState { get; internal set; } = new();
}
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
