namespace DaLion.Overhaul.Modules.Combat.Patchers;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Overhaul.Modules.Combat.StatusEffects;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterUpdatePatcher"/> class.</summary>
    internal MonsterUpdatePatcher()
    {
        this.Target =
            this.RequireMethod<Monster>(nameof(Monster.update), new[] { typeof(GameTime), typeof(GameLocation) });
        this.Prefix!.priority = Priority.First;
    }

    #region harmony patches

    /// <summary>Slow and damage-over-time effects.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    private static bool MonsterUpdatePrefix(Monster __instance, GameTime time)
    {
        try
        {
            var ticks = time.TotalGameTime.Ticks;
            if (__instance.IsBleeding())
            {
                __instance.Get_BleedTimer().Value -= time.ElapsedGameTime.Milliseconds;
                if (__instance.Get_BleedTimer() <= 0)
                {
                    __instance.StopBleeding();
                }
                else
                {
                    if (ticks % 60 == 0)
                    {
                        var bleed = (int)Math.Pow(2.5, __instance.Get_BleedStacks());
                        __instance.Health -= bleed;
                        Log.D($"{__instance.Name} suffered {bleed} bleed damage. HP Left: {__instance.Health}");
                        if (__instance.Health <= 0)
                        {
                            __instance.Die(__instance.Get_Bleeder() ?? Game1.player);
                            return true; // run original logic
                        }
                    }

                    //__instance.startGlowing(Color.Maroon, true, 0.05f);
                }
            }

            if (__instance.IsBurning())
            {
                __instance.Get_BurnTimer().Value -= time.ElapsedGameTime.Milliseconds;
                if (__instance.Get_BurnTimer() <= 0)
                {
                    __instance.CureBurn();
                }
                else
                {
                    if (ticks % 180 == 0)
                    {
                        var burn = (int)(1d / 16d * __instance.MaxHealth);
                        __instance.Health -= burn;
                        Log.D($"{__instance.Name} suffered {burn} burn damage. HP Left: {__instance.Health}");
                        if (__instance.Health <= 0)
                        {
                            __instance.Die(__instance.Get_Burner() ?? Game1.player);
                            return true; // run original logic
                        }
                    }

                    //__instance.startGlowing(Color.OrangeRed, true, 0.05f);
                }
            }

            if (__instance.IsPoisoned())
            {
                __instance.Get_PoisonTimer().Value -= time.ElapsedGameTime.Milliseconds;
                if (__instance.Get_PoisonTimer() <= 0)
                {
                    __instance.CurePoison();
                }
                else
                {
                    if (ticks % 180 == 0)
                    {
                        var poison = (int)(__instance.Get_PoisonStacks() * __instance.MaxHealth / 16d);
                        __instance.Health -= poison;
                        Log.D($"{__instance.Name} suffered {poison} poison damage. HP Left: {__instance.Health}");
                        if (__instance.Health <= 0)
                        {
                            __instance.Die(__instance.Get_Poisoner() ?? Game1.player);
                            return true; // run original logic
                        }
                    }

                    //__instance.startGlowing(Color.LimeGreen, true, 0.05f);
                }
            }

            if (!__instance.IsSlowed())
            {
                return true; // run original logic
            }

            __instance.Get_SlowTimer().Value -= time.ElapsedGameTime.Milliseconds;
            if (__instance.Get_SlowTimer() <= 0)
            {
                __instance.RemoveSlow();
                return true; // run original logic
            }

            if (__instance.IsChilled())
            {
                __instance.startGlowing(Color.PowderBlue, true, 0.05f);
                if (__instance.IsFrozen())
                {
                    __instance.glowingTransparency = 1f;
                }
            }

            var slowIntensity = __instance.Get_SlowIntensity();
            if (slowIntensity <= 0d)
            {
                __instance.RemoveSlow();
                return true; // run original logic
            }

            if (slowIntensity >= 1d)
            {
                return false; // don't run original logic
            }

            var framesToSkip = (int)(1d / slowIntensity);
            return ticks % framesToSkip == 0; // conditionally run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
