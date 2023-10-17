namespace DaLion.Overhaul.Modules.Combat.Patchers;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Overhaul.Modules.Combat.StatusEffects;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
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
            Farmer? killer = null;
            if (__instance.IsBleeding())
            {
                __instance.Get_BleedTimer().Value -= time.ElapsedGameTime.Milliseconds;
                if (__instance.Get_BleedTimer() <= 0)
                {
                    __instance.Unbleed();
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
                            killer = __instance.Get_Bleeder();
                        }
                    }

                    __instance.startGlowing(Color.Maroon, true, 0.05f);
                }
            }

            if (__instance.IsBurning())
            {
                __instance.startGlowing(Color.Yellow, true, 0.05f);
                __instance.Get_BurnTimer().Value -= time.ElapsedGameTime.Milliseconds;
                if (__instance.Get_BurnTimer() <= 0)
                {
                    __instance.Unburn();
                }
                else
                {
                    if (ticks % 180 == 0)
                    {
                        var burn = (int)(1d / 16d * __instance.MaxHealth);
                        if (__instance is Bug or Fly)
                        {
                            burn *= 4;
                        }

                        __instance.Health -= burn;
                        Log.D($"{__instance.Name} suffered {burn} burn damage. HP Left: {__instance.Health}");
                        if (__instance.Health <= 0)
                        {
                            killer = __instance.Get_Burner();
                        }
                    }

                    __instance.startGlowing(Color.Yellow, true, 0.05f);
                }
            }

            if (__instance.IsPoisoned())
            {
                __instance.Get_PoisonTimer().Value -= time.ElapsedGameTime.Milliseconds;
                if (__instance.Get_PoisonTimer() <= 0)
                {
                    __instance.Detox();
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
                            killer = __instance.Get_Poisoner();
                        }
                    }

                    __instance.startGlowing(Color.LimeGreen, true, 0.05f);
                }
            }

            if (__instance.Health <= 0)
            {
                __instance.Die(killer ?? Game1.player);
                return false; // run original logic
            }

            if (!__instance.IsSlowed())
            {
                return true; // run original logic
            }

            __instance.Get_SlowTimer().Value -= time.ElapsedGameTime.Milliseconds;
            if (__instance.Get_SlowTimer() <= 0)
            {
                if (__instance.IsChilled())
                {
                    __instance.Unchill();
                }
                else
                {
                    __instance.Unslow();
                }

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
                __instance.Unslow();
                return true; // run original logic
            }

            if (slowIntensity < 1d && ticks % (int)(1d / slowIntensity) == 0)
            {
                return true; // run original logic
            }

            if (Reflector.GetUnboundFieldGetter<Monster, int>("invincibleCountdown")
                    .Invoke(__instance) is not (var invincibility and > 0))
            {
                return false; // don't run original logic
            }

            invincibility -= time.ElapsedGameTime.Milliseconds;
            Reflector.GetUnboundFieldSetter<Monster, int>("invincibleCountdown")
                .Invoke(__instance, invincibility);
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
