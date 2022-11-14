namespace DaLion.Ligo.Modules.Arsenal.Weapons.Patches;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Weapons.VirtualProperties;
using HarmonyLib;
using StardewValley.Monsters;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterTakeDamagePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="MonsterTakeDamagePatch"/> class.</summary>
    internal MonsterTakeDamagePatch()
    {
        this.Target = this.RequireMethod<Monster>(
            nameof(Monster.takeDamage),
            new[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(string) });
    }

    #region harmony patches

    [HarmonyPrefix]
    private static void MonsterTakeDamagePrefix(Monster __instance, int damage)
    {
        __instance.Set_Overkill(Math.Max(damage - __instance.Health, 0));
    }

    #endregion harmony patches
}
