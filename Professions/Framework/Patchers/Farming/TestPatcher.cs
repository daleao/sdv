namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class TestPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="TestPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal TestPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = typeof(AnimalHouse).RequireMethod(nameof(AnimalHouse.isFull));
    }

    #region harmony patches

    [HarmonyPostfix]
    [UsedImplicitly]
    private static void Test(AnimalHouse __instance, bool __result)
    {
        Log.D(__result.ToString());
    }

    #endregion harmony patches
}
