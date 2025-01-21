namespace DaLion.Professions.Framework.Patchers;

#region using directives

using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterInitNetFieldsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterInitNetFieldsPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal MonsterInitNetFieldsPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Monster>("initNetFields");
    }

    #region harmony patches

    /// <summary>Patch to add custom net debuffs.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void MonsterInitNetFieldsPostix(Monster __instance)
    {
        __instance.NetFields.AddField(__instance.Get_Poached());
    }

    #endregion harmony patches
}
