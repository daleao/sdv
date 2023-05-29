namespace DaLion.Overhaul.Modules.Professions.Patchers.Integration;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Projectiles;

#endregion using directives

[UsedImplicitly]
[RequiresMod("PeacefulEnd.Archery", "Archery", "1.2.0")]
internal sealed class ArrowProjectileBehaviorOnCollisionWithOtherPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ArrowProjectileBehaviorOnCollisionWithOtherPatcher"/> class.</summary>
    internal ArrowProjectileBehaviorOnCollisionWithOtherPatcher()
    {
        this.Target = "Archery.Framework.Objects.Projectiles.ArrowProjectile"
            .ToType()
            .RequireMethod("behaviorOnCollisionWithOther");
        this.Prefix!.priority = Priority.High;
    }

    #region harmony patches

    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    private static void ArrowProjectileBehaviorOnCollisionWithOtherPrefix(BasicProjectile __instance, ref float ____breakChance, Farmer ____owner)
    {
        __instance.Set_DidPierce(false);
        if (!____owner.HasProfession(Profession.Rascal))
        {
            return;
        }

        ____breakChance *= ____owner.HasProfession(Profession.Rascal, true) ? 1.7f : 1.35f;
        if (!____owner.HasProfession(Profession.Desperado))
        {
            return;
        }

        var overcharge = __instance.Get_Overcharge();
        ____breakChance *= 2f - overcharge;
    }

    #endregion harmony patches
}
