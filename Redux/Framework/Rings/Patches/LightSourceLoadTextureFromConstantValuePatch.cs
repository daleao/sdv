namespace DaLion.Redux.Framework.Rings.Patches;

#region using directives

using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class LightSourceLoadTextureFromConstantValuePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="LightSourceLoadTextureFromConstantValuePatch"/> class.</summary>
    internal LightSourceLoadTextureFromConstantValuePatch()
    {
        this.Target = this.RequireMethod<LightSource>("loadTextureFromConstantValue");
    }

    #region harmony patches

    /// <summary>Load custom phase light textures.</summary>
    [HarmonyPostfix]
    private static void LightSourceLoadTextureFromConstantValuePostfix(LightSource __instance, int value)
    {
        if (value == ModEntry.Manifest.UniqueID.GetHashCode())
        {
            __instance.lightTexture = Textures.ResonanceLightTx;
        }
    }

    #endregion harmony patches
}
