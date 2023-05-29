namespace DaLion.Overhaul.Modules.Slingshots.Patchers.Integration;

#region using directives

using DaLion.Overhaul.Modules.Slingshots.Integrations;
using DaLion.Overhaul.Modules.Slingshots.VirtualProperties;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[RequiresMod("PeacefulEnd.Archery", "Archery", "1.2.0")]
internal sealed class BowPerformFirePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BowPerformFirePatcher"/> class.</summary>
    internal BowPerformFirePatcher()
    {
        this.Target = "Archery.Framework.Objects.Weapons.Bow"
            .ToType()
            .RequireMethod(
                "PerformFire",
                new[]
                {
                    typeof(BasicProjectile),
                    typeof(string),
                    typeof(Slingshot),
                    typeof(GameLocation),
                    typeof(Farmer),
                    typeof(bool),
                });
    }

    #region harmony patches

    /// <summary>Cache projectile source.</summary>
    [HarmonyPrefix]
    private static void BowPerformFirePrefix(BasicProjectile projectile, Slingshot slingshot)
    {
        var data = ArcheryIntegration.Instance!.ModApi!.GetProjectileData(Manifest, projectile);
        if (data.Key)
        {
            ArrowProjectile_Properties.Create(projectile, slingshot);
        }
    }

    #endregion harmony patches
}
