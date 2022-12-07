namespace DaLion.Ligo.Modules.Arsenal.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCanUnderstandDwarvesSetterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerCanUnderstandDwarvesSetterPatcher"/> class.</summary>
    internal FarmerCanUnderstandDwarvesSetterPatcher()
    {
        this.Target = this.RequirePropertySetter<Farmer>(nameof(Farmer.canUnderstandDwarves));
    }

    #region harmony patches

    /// <summary>Register mail flag for understanding dwarves.</summary>
    [HarmonyPostfix]
    private static void FarmerCanUnderstandDwarvesSetterPostfix(Farmer __instance, bool value)
    {
        if (value && !__instance.hasOrWillReceiveMail("canUnderstandDwarves"))
        {
            __instance.mailReceived.Add("canUnderstandDwarves");
        }
    }

    #endregion harmony patches
}
