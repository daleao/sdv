namespace DaLion.Redux.Framework.Tweex.Patches;

#region using directives

using DaLion.Shared.Harmony;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectLoadDisplayNamePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ObjectLoadDisplayNamePatch"/> class.</summary>
    internal ObjectLoadDisplayNamePatch()
    {
        this.Target = this.RequireMethod<SObject>("loadDisplayName");
    }

    #region harmony patches

    /// <summary>Add flower name to mead display name.</summary>
    private static void ObjectLoadDisplayNamePostfix(SObject __instance, ref string __result)
    {
        if (!__instance.name.Contains("Mead") || __instance.preservedParentSheetIndex.Value <= 0 ||
            !ModEntry.Config.Tweex.KegsRememberHoneyFlower)
        {
            return;
        }

        var prefix = Game1.objectInformation[__instance.preservedParentSheetIndex.Value].Split('/')[4];
        __result = prefix + ' ' + __result;
    }

    #endregion harmony patches
}
