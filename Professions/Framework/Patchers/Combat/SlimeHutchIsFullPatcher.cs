namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class SlimeHutchIsFullPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlimeHutchIsFullPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal SlimeHutchIsFullPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<SlimeHutch>(nameof(SlimeHutch.isFull));
    }

    #region harmony patches

    /// <summary>Patch to increase Prestiged Piper Hutch capacity.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static void GreenSlimeUpdatePrefix(SlimeHutch __instance, ref int ____slimeCapacity)
    {
        var building = __instance.ParentBuilding;
        if (building?.GetOwner().HasProfessionOrLax(Profession.Piper, true) != true)
        {
            return;
        }

        if (____slimeCapacity < 0)
        {
            ____slimeCapacity = (int)(building.GetData()?.MaxOccupants * 1.5f ?? 30);
        }
    }

    #endregion harmony patches
}
