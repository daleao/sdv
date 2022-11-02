namespace DaLion.Redux.Framework.Professions.Patches.Common;

#region using directives

using DaLion.Redux.Framework.Professions.VirtualProperties;
using HarmonyLib;
using StardewValley.Monsters;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterInitNetFieldsPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="MonsterInitNetFieldsPatch"/> class.</summary>
    internal MonsterInitNetFieldsPatch()
    {
        this.Target = this.RequireMethod<Monster>("initNetFields");
    }

    #region harmony patches

    /// <summary>Patch to add custom net debuffs.</summary>
    [HarmonyPostfix]
    private static void MonsterInitNetFieldsPostix(Monster __instance)
    {
        __instance.NetFields.AddFields(
            __instance.Get_SlowIntensity(),
            __instance.Get_SlowTimer(),
            __instance.Get_FearIntensity(),
            __instance.Get_FearTimer());
    }

    #endregion harmony patches
}
