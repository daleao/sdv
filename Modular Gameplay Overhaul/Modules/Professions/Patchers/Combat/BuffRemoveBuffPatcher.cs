namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class BuffRemoveBuffPatcher : HarmonyPatcher
{
    private static readonly int PiperBuffId = (Manifest.UniqueID + Profession.Piper).GetHashCode();

    /// <summary>Initializes a new instance of the <see cref="BuffRemoveBuffPatcher"/> class.</summary>
    internal BuffRemoveBuffPatcher()
    {
        this.Target = this.RequireMethod<Buff>(nameof(Buff.removeBuff));
    }

    #region harmony patches

    [HarmonyPrefix]
    private static void BuffRemoveBuffPrefix(Buff __instance)
    {
        if (__instance.which == PiperBuffId && __instance.millisecondsDuration <= 0)
        {
            Array.Clear(Game1.player.Get_PiperBuffs(), 0, 12);
        }
    }

    #endregion harmony patches
}
