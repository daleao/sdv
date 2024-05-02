namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class SlimeHutchCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlimeHutchCtorPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal SlimeHutchCtorPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireConstructor<SlimeHutch>(Type.EmptyTypes);
    }

    /// <inheritdoc />
    protected override bool ApplyImpl(Harmony harmony)
    {
        if (!base.ApplyImpl(harmony))
        {
            return false;
        }

        this.Target = this.RequireConstructor<SlimeHutch>([typeof(string), typeof(string)]);
        return base.ApplyImpl(harmony);
    }

    /// <inheritdoc />
    protected override bool UnapplyImpl(Harmony harmony)
    {
        if (!base.UnapplyImpl(harmony))
        {
            return false;
        }

        this.Target = this.RequireConstructor<SlimeHutch>([typeof(string), typeof(string)]);
        return base.UnapplyImpl(harmony);
    }

    #region harmony patches

    /// <summary>Patch to increase Prestiged Piper Hutch capacity.</summary>
    [HarmonyPrefix]
    private static void GreenSlimeUpdatePrefix(SlimeHutch __instance)
    {
        if (__instance.GetContainingBuilding()?.GetOwner().HasProfessionOrLax(Profession.Piper, true) == true)
        {
            __instance.waterSpots.SetCount(6);
        }
    }

    #endregion harmony patches
}
