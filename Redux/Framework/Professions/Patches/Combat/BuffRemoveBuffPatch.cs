namespace DaLion.Redux.Framework.Professions.Patches.Combat;

#region using directives

using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class BuffRemoveBuffPatch : HarmonyPatch
{
    private static readonly int _piperBuffId = (ModEntry.Manifest.UniqueID + Profession.Piper).GetHashCode();

    /// <summary>Initializes a new instance of the <see cref="BuffRemoveBuffPatch"/> class.</summary>
    internal BuffRemoveBuffPatch()
    {
        this.Target = this.RequireMethod<Buff>(nameof(Buff.removeBuff));
    }

    #region harmony patches

    [HarmonyPrefix]
    private static void BuffRemoveBuffPrefix(Buff __instance)
    {
        if (__instance.which == _piperBuffId && __instance.millisecondsDuration <= 0)
        {
            Array.Clear(ModEntry.State.Professions.AppliedPiperBuffs, 0, 12);
        }
    }

    #endregion harmony patches
}
