namespace DaLion.Stardew.Tweex.Framework.Patches;

[UsedImplicitly]
internal sealed class ObjectLoadDisplayName : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectLoadDisplayName()
    {
        Target = RequireMethod<SObject>("loadDisplayName");
    }

    #region harmony patches

    /// <summary>Add flower name to mead display name.</summary>
    private static void ObjectLoadDisplayNamePostfix(SObject __instance, ref string __result)
    {
        if (!__instance.name.Contains("Mead") || __instance.preservedParentSheetIndex.Value <= 0 ||
            !ModEntry.Config.KegsRememberHoneyFlower) return;

        var prefix = Game1.objectInformation[__instance.preservedParentSheetIndex.Value].Split('/')[4];
        __result = prefix + ' ' + __result;
    }

    #endregion harmony patches
}