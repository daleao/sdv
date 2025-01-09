namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class GreenSlimeOverlapsFarmerForDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GreenSlimeOverlapsFarmerForDamagePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal GreenSlimeOverlapsFarmerForDamagePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Monster>(nameof(Monster.OverlapsFarmerForDamage));
    }

    #region harmony patches

    /// <summary>Patch to prevent damage from Piped Slimes.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool GreenSlimeOverlapsFarmerForDamagePrefix(Monster __instance)
    {
        return __instance is not GreenSlime slime || slime.Get_Piped() is null;
    }

    #endregion harmony patches
}
