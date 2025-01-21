namespace DaLion.Professions.Framework.Patchers;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerEatObjectPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerEatObjectPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FarmerEatObjectPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.eatObject));
    }

    #region harmony patches

    /// <summary>Patch to prevent eating during Limit Break.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool FarmerEatObjectPrefix()
    {
        if (State.LimitBreak?.IsActive != true)
        {
            return true; // run original logic
        }

        Game1.playSound("cancel");
        Game1.showRedMessage(I18n.Ulti_Canteat());
        return false; // don't run original logic
    }

    /// <summary>Patch to grant Prestiged Ecologist buffs.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void FarmerEatObjectPostfix(Farmer __instance, SObject o)
    {
        if (!o.IsForage() || !__instance.HasProfession(Profession.Ecologist, true))
        {
            return;
        }

        if (State.EcologistBuffsLookup.TryGetValue(o.ItemId, out var buffIndex))
        {
            __instance.ApplyPrestigedEcologistBuff(buffIndex);
            return;
        }

        buffIndex = Game1.random.Next(10);
        __instance.ApplyPrestigedEcologistBuff(buffIndex);
        State.EcologistBuffsLookup[o.ItemId] = buffIndex;
    }

    #endregion harmony patches
}
