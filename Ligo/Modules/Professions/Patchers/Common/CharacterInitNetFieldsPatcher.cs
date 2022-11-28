namespace DaLion.Ligo.Modules.Professions.Patchers.Common;

#region using directives

using DaLion.Ligo.Modules.Professions.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class CharacterInitNetFieldsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CharacterInitNetFieldsPatcher"/> class.</summary>
    internal CharacterInitNetFieldsPatcher()
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
