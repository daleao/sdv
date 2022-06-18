namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

#endregion using directives


[UsedImplicitly]
internal sealed class BuffRemoveBuffPatch : BasePatch
{
    private static readonly int _piperBuffId = (ModEntry.Manifest.UniqueID + Profession.Piper).GetHashCode();

    /// <summary>Construct an instance.</summary>
    internal BuffRemoveBuffPatch()
    {
        Original = RequireMethod<Buff>(nameof(Buff.removeBuff));
    }

    #region harmony patches

    [HarmonyPrefix]
    private static void BuffUpdatePrefix(Buff __instance)
    {
        if (__instance.which == _piperBuffId && __instance.millisecondsDuration <= 0)
            Array.Clear(ModEntry.PlayerState.AppliedPiperBuffs, 0, 12);
    }

    #endregion harmony patches
}