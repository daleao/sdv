namespace DaLion.Professions.Framework.Patchers;

#region using directives

using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class CharacterInitNetFieldsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CharacterInitNetFieldsPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal CharacterInitNetFieldsPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Character>("initNetFields");
    }

    #region harmony patches

    /// <summary>Patch to add custom net fields.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void CharacterInitNetFieldsPostfix(Character __instance)
    {
        if (__instance is Farmer { Name: not null } farmer)
        {
            __instance.NetFields
                .AddField(farmer.Get_LimitBreakId())
                .AddField(farmer.Get_IsLimitBreaking())
                .AddField(farmer.Get_IsHuntingTreasure());
        }
    }

    #endregion harmony patches
}
