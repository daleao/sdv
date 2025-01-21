namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotGetAmmoDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotGetAmmoDamagePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal SlingshotGetAmmoDamagePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.GetAmmoDamage));
    }

    #region harmony patches

    /// <summary>Patch to set Rascal Slime ammo damage.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void SlingshotGetAmmoDamagePostfix(Slingshot __instance, ref int __result, SObject ammunition)
    {
        if (ammunition.QualifiedItemId != QIDs.Slime)
        {
            return;
        }

        var user = __instance.lastUser;
        if (!user.HasProfession(Profession.Piper))
        {
            __result = 1;
            return;
        }

        if (!user.HasProfession(Profession.Piper, true))
        {
            __result = 20;
            return;
        }

        __result = 40;
    }

    #endregion harmony patches
}
