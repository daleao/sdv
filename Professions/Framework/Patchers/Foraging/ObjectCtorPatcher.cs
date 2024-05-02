namespace DaLion.Professions.Framework.Patchers.Foraging;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectCtorPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ObjectCtorPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireConstructor<SObject>(
            typeof(string), typeof(int), typeof(bool), typeof(int), typeof(int));
    }

    #region harmony patches

    /// <summary>Patch for Ecologist wild berry recovery.</summary>
    [HarmonyPostfix]
    private static void ObjectCtorPostfix(SObject __instance)
    {
        if (!__instance.isForage() || __instance.Edibility <= 0)
        {
            return;
        }

        var owner = __instance.GetOwner();
        if (owner.HasProfession(Profession.Ecologist))
        {
            __instance.Edibility = (int)(__instance.Edibility * 1.5f);
        }
    }

    #endregion harmony patches
}
