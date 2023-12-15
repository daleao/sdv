namespace DaLion.Overhaul.Modules.Core.Patchers;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Core.Extensions;
using DaLion.Overhaul.Modules.Core.StatusEffects;
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

            if (ticks % 30 == 0)
            {
                var bleedTimer = __instance.Get_BleedTimer().Value;
                if (bleedTimer > 0)
                {
                    bleedTimer -= time.ElapsedGameTime.Milliseconds;
                    if (bleedTimer <= 0)
                    {
                        __instance.Unbleed();
                    }
                    else
                    {
                        __instance.Get_BleedTimer().Value = bleedTimer;
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

                var burnTimer = __instance.Get_BurnTimer().Value;
                if (burnTimer > 0)
                {
                    burnTimer -= time.ElapsedGameTime.Milliseconds;
                    if (burnTimer <= 0)
                    {
                        __instance.Unburn();
                    }
                    else
                    {
                        __instance.Get_BurnTimer().Value = burnTimer;
                        if ((__instance is Bug or Fly && ticks % 30 == 0) || ticks % 180 == 0)
                        {
                            var burn = (int)(1d / 16d * __instance.MaxHealth);
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

                var poisonTimer = __instance.Get_PoisonTimer().Value;
                if (poisonTimer > 0)
                {
                    poisonTimer -= time.ElapsedGameTime.Milliseconds;
                    if (poisonTimer <= 0)
                    {
                        __instance.Detox();
                    }
                    else
                    {
                        __instance.Get_PoisonTimer().Value = poisonTimer;
                        var pow = (int)Math.Pow(2, __instance.Get_PoisonStacks() - 1);
                        if (ticks % (180 / pow) == 0)
                        {
                            var poison = (int)(pow * __instance.MaxHealth / 16d);
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
            }

            var slowTimer = __instance.Get_SlowTimer().Value;
            if (slowTimer <= 0)
            {
                return true; // run original logic
            }

            slowTimer -= time.ElapsedGameTime.Milliseconds;
            if (slowTimer <= 0)
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

            __instance.Get_SlowTimer().Value = slowTimer;
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

            if (slowIntensity < 1f && ticks % (int)(1f / slowIntensity) == 0f)
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
