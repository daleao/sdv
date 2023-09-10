namespace DaLion.Overhaul.Modules.Combat.Patchers.Rings;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

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
        switch (value)
        {
            case 100:
                __instance.lightTexture = Textures.StrongerResonanceTx;
                break;
            case 101:
                __instance.lightTexture = Textures.PatternedResonanceTx;
                break;
        }
    }

    #endregion harmony patches
}
