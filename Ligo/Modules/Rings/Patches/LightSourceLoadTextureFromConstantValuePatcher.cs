namespace DaLion.Ligo.Modules.Rings.Patches;

#region using directives

using HarmonyLib;
using Shared.Harmony;

#endregion using directives

[UsedImplicitly]
internal sealed class LightSourceLoadTextureFromConstantValuePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="LightSourceLoadTextureFromConstantValuePatcher"/> class.</summary>
    internal LightSourceLoadTextureFromConstantValuePatcher()
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
