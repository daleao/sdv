namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class LightSourceLoadTextureFromConstantValuePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="LightSourceLoadTextureFromConstantValuePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal LightSourceLoadTextureFromConstantValuePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<LightSource>("loadTextureFromConstantValue");
    }

    #region harmony patches

    /// <summary>Load custom phase light textures.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void LightSourceLoadTextureFromConstantValuePostfix(LightSource __instance, int value)
    {
        __instance.lightTexture = value switch
        {
            100 => Textures.StrongerResonanceTx,
            101 => Textures.PatternedResonanceTx,
            _ => __instance.lightTexture,
        };
    }

    #endregion harmony patches
}
