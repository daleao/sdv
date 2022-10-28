namespace DaLion.Redux.Professions.Patches.Combat;

#region using directives

using DaLion.Redux.Professions.Extensions;
using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotCtorPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotCtorPatch"/> class.</summary>
    internal SlingshotCtorPatch()
    {
        this.Target = this.RequireConstructor<Slingshot>(Type.EmptyTypes);
    }

    /// <inheritdoc />
    protected override void ApplyImpl(Harmony harmony)
    {
        base.ApplyImpl(harmony);
        this.Target = this.RequireConstructor<Slingshot>(typeof(int));
        base.ApplyImpl(harmony);
    }

    #region harmony patches

    /// <summary>Add Rascal ammo slot.</summary>
    [HarmonyPostfix]
    private static void SlingshotCtorPostfix(Slingshot __instance)
    {
        if (!Game1.player.HasProfession(Profession.Rascal))
        {
            return;
        }

        __instance.numAttachmentSlots.Value = 2;
        __instance.attachments.SetCount(2);
    }

    #endregion harmony patches
}
