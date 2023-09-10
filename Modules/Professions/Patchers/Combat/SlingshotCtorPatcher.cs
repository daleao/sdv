namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotCtorPatcher"/> class.</summary>
    internal SlingshotCtorPatcher()
    {
        this.Target = this.RequireConstructor<Slingshot>(Type.EmptyTypes);
    }

    /// <inheritdoc />
    protected override bool ApplyImpl(Harmony harmony)
    {
        if (!base.ApplyImpl(harmony))
        {
            return false;
        }

        this.Target = this.RequireConstructor<Slingshot>(typeof(int));
        return base.ApplyImpl(harmony);
    }

    /// <inheritdoc />
    protected override bool UnapplyImpl(Harmony harmony)
    {
        this.Target = this.RequireConstructor<Slingshot>(Type.EmptyTypes);
        if (!base.UnapplyImpl(harmony))
        {
            return false;
        }

        this.Target = this.RequireConstructor<Slingshot>(typeof(int));
        return base.UnapplyImpl(harmony);
    }

    #region harmony patches

    /// <summary>Add Rascal ammo slot.</summary>
    [HarmonyPostfix]
    private static void SlingshotCtorPostfix(Slingshot __instance)
    {
        if (!Game1.player.HasProfession(Profession.Rascal) ||
            (__instance.numAttachmentSlots.Value != 1 && __instance.attachments.Length != 1))
        {
            return;
        }

        __instance.numAttachmentSlots.Value = 2;
        __instance.attachments.SetCount(2);
    }

    #endregion harmony patches
}
