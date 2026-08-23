namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class SlimeHutchCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlimeHutchCtorPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal SlimeHutchCtorPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireConstructor<SlimeHutch>(typeof(string), typeof(string));
    }

    #region harmony patches

    /// <summary>Patch to color Slime Balls.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void SlimeHutchCtorPostfix(SlimeHutch __instance, ref int ____slimeCapacity)
    {
        var areThereAnyPipers = false;
        foreach (var farmer in Game1.getAllFarmers())
        {
            if (farmer.HasProfession(Profession.Piper))
            {
                areThereAnyPipers = true;
            }
        }

        if (!areThereAnyPipers)
        {
            return;
        }

        ____slimeCapacity = 30;
        __instance.waterSpots.SetCount(6);
    }

    #endregion harmony patches
}
