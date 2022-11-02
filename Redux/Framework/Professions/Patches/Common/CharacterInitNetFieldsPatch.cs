namespace DaLion.Redux.Framework.Professions.Patches.Common;

#region using directives

using DaLion.Redux.Framework.Professions.VirtualProperties;
using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class CharacterInitNetFieldsPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="CharacterInitNetFieldsPatch"/> class.</summary>
    internal CharacterInitNetFieldsPatch()
    {
        this.Target = this.RequireMethod<Character>("initNetFields");
    }

    #region harmony patches

    /// <summary>Patch to add custom net fields.</summary>
    [HarmonyPostfix]
    private static void CharacterInitNetFieldsPostfix(Character __instance)
    {
        if (__instance is not Farmer farmer || farmer.Name is null)
        {
            return;
        }

        __instance.NetFields.AddFields(farmer.Get_UltimateIndex());
        __instance.NetFields.AddFields(farmer.Get_IsFake());
    }

    #endregion harmony patches
}
