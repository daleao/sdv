// ReSharper disable CompareOfFloatsByEqualityOperator
namespace DaLion.Overhaul.Modules.Slingshots.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Overhaul.Modules.Enchantments.Gemstone;
using DaLion.Overhaul.Modules.Rings.VirtualProperties;
using DaLion.Overhaul.Modules.Slingshots.Extensions;
using DaLion.Overhaul.Modules.Slingshots.Integrations;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Integrations.Archery;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Slingshot_Stats
{
    internal static ConditionalWeakTable<Slingshot, Holder> Values { get; } = new();

    internal static float Get_EffectiveDamageModifier(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).GetEffectiveDamageModifier();
    }

    internal static float Get_DisplayedDamageModifier(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).GetDisplayedDamage();
    }

    internal static float Get_EffectiveKnockback(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).GetEffectiveKnockbackBonus();
    }

    internal static float Get_DisplayedKnockback(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).GetDisplayedKnockbackBonus();
    }

    internal static float Get_EffectiveCritChance(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).GetEffectiveCritChanceBonus();
    }

    internal static float Get_DisplayedCritChance(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).GetDisplayedCritChanceBonus();
    }

    internal static float Get_EffectiveCritPower(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).GetEffectiveCritPowerBonus();
    }

    internal static float Get_DisplayedCritPower(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).GetDisplayedCritPowerBonus();
    }

    internal static float Get_EffectiveFireSpeed(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).GetEffectiveFireSpeedModifier();
    }

    internal static float Get_DisplayedFireSpeed(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).GetDisplayedFireSpeedModifer();
    }

    internal static float Get_EffectiveCooldownReduction(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).GetEffectiveCooldownModifier();
    }

    internal static float Get_DisplayedCooldownModifier(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).GetDisplayedCooldownModifer();
    }

    internal static float Get_EffectiveResilience(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).GetEffectiveResilienceModifier();
    }

    internal static float Get_DisplayedResilience(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).GetDisplayedResilienceModifier();
    }

    internal static int Get_RowsInTooltip(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).GetStatRowsInTooltip();
    }

    internal static int Get_SpaceBeforeAmmoSlots(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).GetTooltipSpaceBeforeAmmoSlots();
    }

    internal static int Get_ExtraTooltipSpace(this Slingshot slingshot)
    {
        return Values.GetValue(slingshot, Create).GetBottomExtraTooltipSpace();
    }

    internal static void Invalidate(this Slingshot slingshot)
    {
        Values.Remove(slingshot);
    }

    private static Holder Create(Slingshot slingshot)
    {
        var bowData = ArcheryIntegration.Instance?.ModApi?.GetWeaponData(Manifest, slingshot);
        return bowData is null ? CreateAsSlingshot(slingshot) : CreateAsBow(slingshot, bowData);
    }

    private static SlingshotStats CreateAsSlingshot(Slingshot slingshot)
    {
        var holder = new SlingshotStats();

        if (SlingshotsModule.Config.EnableRebalance)
        {
            holder.DamageMod = slingshot.InitialParentTileIndex switch
            {
                ItemIDs.MasterSlingshot => 0.5f,
                ItemIDs.GalaxySlingshot => 1f,
                ItemIDs.InfinitySlingshot => 1.5f,
                _ => 0f,
            };

            holder.KnockbackBonus = slingshot.InitialParentTileIndex switch
            {
                ItemIDs.MasterSlingshot => 0.1f,
                ItemIDs.GalaxySlingshot => 0.2f,
                ItemIDs.InfinitySlingshot => 0.25f,
                _ => 0f,
            };
        }
        else
        {
            holder.DamageMod = slingshot.InitialParentTileIndex switch
            {
                ItemIDs.MasterSlingshot => 2f,
                ItemIDs.GalaxySlingshot => 4f,
                ItemIDs.InfinitySlingshot => 5f,
                _ => 0f,
            };
        }

        holder.AmmoDamage = slingshot.GetAmmoDamage();

        for (var i = 0; i < slingshot.attachments.Length; i++)
        {
            var ammo = slingshot.attachments[i];
            if (ammo is null)
            {
                continue;
            }

            switch (ammo.ParentSheetIndex)
            {
                case ItemIDs.Ruby:
                    holder.RubyBonus += 0.1f;
                    if (slingshot.Get_ResonatingChord<RubyEnchantment>() is { } rubyChord)
                    {
                        holder.RubyBonus += (float)(rubyChord.Amplitude * 0.1f);
                        holder.AmmoDamage += (int)(holder.AmmoDamage * rubyChord.Amplitude);
                    }

                    break;

                case ItemIDs.Aquamarine:
                    holder.AquamarineBonus += 0.1f;
                    if (slingshot.Get_ResonatingChord<AquamarineEnchantment>() is { } aquamarineChord)
                    {
                        holder.AquamarineBonus += (float)(aquamarineChord.Amplitude * 0.1f);
                        holder.AmmoDamage += (int)(holder.AmmoDamage * aquamarineChord.Amplitude);
                    }

                    break;

                case ItemIDs.Amethyst:
                    holder.AmethystBonus += 0.1f;
                    if (slingshot.Get_ResonatingChord<AmethystEnchantment>() is { } amethystChord)
                    {
                        holder.AmethystBonus += (float)(amethystChord.Amplitude * 0.1f);
                        holder.AmmoDamage += (int)(holder.AmmoDamage * amethystChord.Amplitude);
                    }

                    break;

                case ItemIDs.Emerald:
                    holder.EmeraldBonus += 1f;
                    if (slingshot.Get_ResonatingChord<EmeraldEnchantment>() is { } emeraldChord)
                    {
                        holder.EmeraldBonus += (float)emeraldChord.Amplitude;
                        holder.AmmoDamage += (int)(holder.AmmoDamage * emeraldChord.Amplitude);
                    }

                    break;

                case ItemIDs.Jade:
                    holder.JadeBonus += EnchantmentsModule.ShouldEnable && EnchantmentsModule.Config.RebalancedForges
                        ? 0.5f
                        : 0.1f;
                    if (slingshot.Get_ResonatingChord<JadeEnchantment>() is { } jadeChord)
                    {
                        holder.JadeBonus += (float)(jadeChord.Amplitude *
                                                    (EnchantmentsModule.ShouldEnable &&
                                                     EnchantmentsModule.Config.RebalancedForges
                                                        ? 0.5f
                                                        : 0.1f));
                        holder.AmmoDamage += (int)(holder.AmmoDamage * jadeChord.Amplitude);
                    }

                    break;

                case ItemIDs.Topaz:
                    holder.TopazBonus += 1f;
                    if (slingshot.Get_ResonatingChord<TopazEnchantment>() is { } topazChord)
                    {
                        holder.TopazBonus += (float)topazChord.Amplitude;
                        holder.AmmoDamage += (int)(holder.AmmoDamage * topazChord.Amplitude);
                    }

                    break;

                case ItemIDs.Diamond:
                    for (var j = 0; j < 2; j++)
                    {
                        switch (Game1.random.Next(7))
                        {
                            case 0:
                                holder.RubyBonus += 0.1f;
                                break;
                            case 1:
                                holder.AquamarineBonus += 0.1f;
                                break;
                            case 2:
                                holder.AmethystBonus += 0.1f;
                                break;
                            case 3:
                                holder.EmeraldBonus += 1f;
                                break;
                            case 4:
                                holder.JadeBonus += EnchantmentsModule.ShouldEnable &&
                                                   EnchantmentsModule.Config.RebalancedForges
                                    ? 0.5f
                                    : 0.1f;
                                break;
                            case 5:
                                holder.TopazBonus += 1f;
                                break;
                            case 6:
                                holder.GarnetBonus += 1f;
                                break;
                        }
                    }

                    break;

                case SObject.prismaticShardIndex:
                    holder.RubyBonus += 0.1f;
                    holder.AquamarineBonus += 0.1f;
                    holder.AmethystBonus += 0.1f;
                    holder.EmeraldBonus += 1f;
                    holder.JadeBonus += EnchantmentsModule.ShouldEnable &&
                                       EnchantmentsModule.Config.RebalancedForges
                        ? 0.5f
                        : 0.1f;
                    holder.TopazBonus += 1f;
                    holder.GarnetBonus += 1f;
                    break;

                default:
                    if (Globals.GarnetIndex.HasValue && ammo.ParentSheetIndex == Globals.GarnetIndex.Value)
                    {
                        holder.GarnetBonus += 1f;
                        if (slingshot.Get_ResonatingChord<GarnetEnchantment>() is { } garnetChord)
                        {
                            holder.GarnetBonus += (float)garnetChord.Amplitude;
                            holder.AmmoDamage += (int)(holder.AmmoDamage * garnetChord.Amplitude);
                        }
                    }

                    break;
            }
        }

        #region deprecated

        //if (slingshot.hasEnchantmentOfType<RubyEnchantment>())
        //{
        //    holder.RubyBonus = slingshot.GetEnchantmentLevel<RubyEnchantment>() * 0.1f;
        //    if (slingshot.Get_ResonatingChord<RubyEnchantment>() is { } rubyChord)
        //    {
        //        holder.RubyBonus +=
        //            (float)(slingshot.GetEnchantmentLevel<RubyEnchantment>() * rubyChord.Amplitude * 0.1f);
        //    }
        //}

        //if (slingshot.hasEnchantmentOfType<AmethystEnchantment>())
        //{
        //    holder.AmethystBonus = slingshot.GetEnchantmentLevel<AmethystEnchantment>() * 0.1f;
        //    if (slingshot.Get_ResonatingChord<AmethystEnchantment>() is { } amethystChord)
        //    {
        //        holder.AmethystBonus += (float)(slingshot.GetEnchantmentLevel<AmethystEnchantment>() *
        //                                        amethystChord.Amplitude * 0.1f);
        //    }
        //}

        //if (slingshot.hasEnchantmentOfType<AquamarineEnchantment>())
        //{
        //    holder.AquamarineBonus = slingshot.GetEnchantmentLevel<AquamarineEnchantment>() * 0.046f;
        //    if (slingshot.Get_ResonatingChord<AquamarineEnchantment>() is { } aquamarineChord)
        //    {
        //        holder.AquamarineBonus += (float)(slingshot.GetEnchantmentLevel<AquamarineEnchantment>() *
        //                                          aquamarineChord.Amplitude * 0.046f);
        //    }
        //}

        //if (slingshot.hasEnchantmentOfType<JadeEnchantment>())
        //{
        //    holder.JadeBonus = slingshot.GetEnchantmentLevel<JadeEnchantment>() *
        //                       (EnchantmentsModule.ShouldEnable && EnchantmentsModule.Config.RebalancedForges
        //                           ? 0.5f
        //                           : 0.1f);
        //    if (slingshot.Get_ResonatingChord<JadeEnchantment>() is { } jadeChord)
        //    {
        //        holder.JadeBonus += (float)(slingshot.GetEnchantmentLevel<JadeEnchantment>() * jadeChord.Amplitude *
        //                                    (EnchantmentsModule.ShouldEnable &&
        //                                     EnchantmentsModule.Config.RebalancedForges
        //                                        ? 0.5f
        //                                        : 0.1f));
        //    }
        //}

        //if (slingshot.hasEnchantmentOfType<EmeraldEnchantment>())
        //{
        //    holder.EmeraldBonus = slingshot.GetEnchantmentLevel<EmeraldEnchantment>();
        //    if (slingshot.Get_ResonatingChord<EmeraldEnchantment>() is { } emeraldChord)
        //    {
        //        holder.EmeraldBonus +=
        //            (float)(slingshot.GetEnchantmentLevel<EmeraldEnchantment>() * emeraldChord.Amplitude);
        //    }
        //}

        //if (slingshot.hasEnchantmentOfType<GarnetEnchantment>())
        //{
        //    holder.GarnetBonus = slingshot.GetEnchantmentLevel<GarnetEnchantment>();
        //    if (slingshot.Get_ResonatingChord<GarnetEnchantment>() is { } garnetChord)
        //    {
        //        holder.GarnetBonus +=
        //            (float)(slingshot.GetEnchantmentLevel<GarnetEnchantment>() * garnetChord.Amplitude);
        //    }
        //}

        //if (slingshot.hasEnchantmentOfType<TopazEnchantment>())
        //{
        //    holder.TopazBonus = slingshot.GetEnchantmentLevel<TopazEnchantment>();
        //    if (slingshot.Get_ResonatingChord<TopazEnchantment>() is { } topazChord)
        //    {
        //        holder.TopazBonus +=
        //            (float)(slingshot.GetEnchantmentLevel<TopazEnchantment>() * topazChord.Amplitude);
        //    }
        //}

        #endregion deprecated

        if (holder.RubyBonus > 0f || slingshot.InitialParentTileIndex != ItemIDs.BasicSlingshot)
        {
            holder.RowsInTooltip++;
        }

        if (holder.AmethystBonus > 0f || slingshot.InitialParentTileIndex != ItemIDs.BasicSlingshot)
        {
            holder.RowsInTooltip++;
        }

        if (holder.AquamarineBonus > 0f)
        {
            holder.RowsInTooltip++;
        }

        if (holder.JadeBonus > 0f)
        {
            holder.RowsInTooltip++;
        }

        if (holder.EmeraldBonus > 0f)
        {
            holder.RowsInTooltip++;
        }

        if (holder.GarnetBonus > 0f)
        {
            holder.RowsInTooltip++;
        }

        if (holder.TopazBonus > 0f)
        {
            holder.RowsInTooltip++;
        }

        return holder;
    }

    private static BowStats CreateAsBow(Slingshot bow, IWeaponData bowData)
    {
        var holder = new BowStats();

        var weaponModel = ArcheryIntegration.Instance!.GetWeaponModel.Value.Invoke(null, new object?[] { bow });
        var ammoModel = bow.attachments?[0] is not null
            ? ArcheryIntegration.Instance.GetAmmoModel.Value.Invoke(null, new object?[] { bow.attachments[0] })
            : null;
        if (weaponModel is null)
        {
            ThrowHelper.ThrowInvalidOperationException($"Failed to retrieve weapon model for bow item {bow.Name}");
        }

        holder.MinDamage = bowData.DamageRange.Min;
        holder.MaxDamage = bowData.DamageRange.Max;
        if (bow.attachments?[0] is not null)
        {
            holder.HasAmmo = true;
            if (ammoModel is not null)
            {
                holder.AmmoDamage = Reflector.GetUnboundPropertyGetter<object, int>(ammoModel, "Damage").Invoke(ammoModel);
            }
        }
        else
        {
            holder.UsesInternalAmmo =
                Reflector.GetUnboundMethodDelegate<Func<object, bool>>(weaponModel, "UsesInternalAmmo")
                    .Invoke(weaponModel);
        }

        holder.BaseKnockback = Reflector.GetUnboundPropertyGetter<object, float>(weaponModel, "Knockback").Invoke(weaponModel);
        holder.BaseCritChance = Reflector.GetUnboundPropertyGetter<object, float>(weaponModel, "CriticalChance").Invoke(weaponModel);
        holder.BaseCritPower = Reflector.GetUnboundPropertyGetter<object, float>(weaponModel, "CriticalDamageMultiplier").Invoke(weaponModel);
        if (ammoModel is not null)
        {
            holder.BaseCritChance += Reflector.GetUnboundPropertyGetter<object, float>(ammoModel, "CriticalChance").Invoke(ammoModel);
            holder.BaseCritPower += Reflector.GetUnboundPropertyGetter<object, float>(ammoModel, "CriticalDamageMultiplier").Invoke(ammoModel);
        }

        holder.RowsInTooltip++;
        if (holder.BaseKnockback != 1f)
        {
            holder.RowsInTooltip++;
        }

        if (holder.BaseCritChance != 0f)
        {
            holder.RowsInTooltip++;
        }

        if (holder.BaseCritPower != (holder.HasAmmo ? 2f : 1f))
        {
            holder.RowsInTooltip++;
        }

        holder.HasSpecialAttack = Reflector.GetUnboundPropertyGetter<object, object?>(weaponModel, "SpecialAttack").Invoke(weaponModel) is not null;

        if (bow.attachments is null)
        {
            return holder;
        }

        for (var i = 0; i < bow.attachments.Length; i++)
        {
            var ammo = bow.attachments[i];
            if (ammo is null)
            {
                continue;
            }

            var ammoId = Reflector
                .GetStaticMethodDelegate<Func<Item, string>>(
                    "Archery.Framework.Objects.InstancedObject".ToType(),
                    "GetInternalId").Invoke(ammo);
            if (string.IsNullOrEmpty(ammoId))
            {
                continue;
            }

            if (ammoId.Contains("Ruby"))
            {
                holder.RubyBonus += 0.1f;
                if (bow.Get_ResonatingChord<RubyEnchantment>() is { } rubyChord)
                {
                    holder.RubyBonus += (float)(rubyChord.Amplitude * 0.1f);
                    holder.AmmoDamage += (int)(holder.AmmoDamage * rubyChord.Amplitude);
                }
            }
            else if (ammoId.Contains("Aquamarine"))
            {
                holder.AquamarineBonus += 0.1f;
                if (bow.Get_ResonatingChord<AquamarineEnchantment>() is { } aquamarineChord)
                {
                    holder.AquamarineBonus += (float)(aquamarineChord.Amplitude * 0.1f);
                    holder.AmmoDamage += (int)(holder.AmmoDamage * aquamarineChord.Amplitude);
                }
            }
            else if (ammoId.Contains("Amethyst"))
            {
                holder.AmethystBonus += 0.1f;
                if (bow.Get_ResonatingChord<AmethystEnchantment>() is { } amethystChord)
                {
                    holder.AmethystBonus += (float)(amethystChord.Amplitude * 0.1f);
                    holder.AmmoDamage += (int)(holder.AmmoDamage * amethystChord.Amplitude);
                }
            }
            else if (ammoId.Contains("Emerald"))
            {
                holder.EmeraldBonus += 1f;
                if (bow.Get_ResonatingChord<EmeraldEnchantment>() is { } emeraldChord)
                {
                    holder.EmeraldBonus += (float)emeraldChord.Amplitude;
                    holder.AmmoDamage += (int)(holder.AmmoDamage * emeraldChord.Amplitude);
                }
            }
            else if (ammoId.Contains("Jade"))
            {
                holder.JadeBonus += EnchantmentsModule.ShouldEnable && EnchantmentsModule.Config.RebalancedForges
                    ? 0.5f
                    : 0.1f;
                if (bow.Get_ResonatingChord<JadeEnchantment>() is { } jadeChord)
                {
                    holder.JadeBonus += (float)(jadeChord.Amplitude *
                                                (EnchantmentsModule.ShouldEnable &&
                                                 EnchantmentsModule.Config.RebalancedForges
                                                    ? 0.5f
                                                    : 0.1f));
                    holder.AmmoDamage += (int)(holder.AmmoDamage * jadeChord.Amplitude);
                }
            }
            else if (ammoId.Contains("Topaz"))
            {
                holder.TopazBonus += 1f;
                if (bow.Get_ResonatingChord<TopazEnchantment>() is { } topazChord)
                {
                    holder.TopazBonus += (float)topazChord.Amplitude;
                    holder.AmmoDamage += (int)(holder.AmmoDamage * topazChord.Amplitude);
                }
            }
            else if (ammoId.Contains("Garnet"))
            {
                holder.GarnetBonus += 1f;
                if (bow.Get_ResonatingChord<GarnetEnchantment>() is { } garnetChord)
                {
                    holder.GarnetBonus += (float)garnetChord.Amplitude;
                    holder.AmmoDamage += (int)(holder.AmmoDamage * garnetChord.Amplitude);
                }
            }
            else if (ammoId.Contains("Diamond"))
            {
                for (var j = 0; j < 2; j++)
                {
                    switch (Game1.random.Next(7))
                    {
                        case 0:
                            holder.RubyBonus += 0.1f;
                            break;
                        case 1:
                            holder.AquamarineBonus += 0.1f;
                            break;
                        case 2:
                            holder.AmethystBonus += 0.1f;
                            break;
                        case 3:
                            holder.EmeraldBonus += 1f;
                            break;
                        case 4:
                            holder.JadeBonus += EnchantmentsModule.ShouldEnable &&
                                               EnchantmentsModule.Config.RebalancedForges
                                ? 0.5f
                                : 0.1f;
                            break;
                        case 5:
                            holder.TopazBonus += 1f;
                            break;
                        case 6:
                            holder.GarnetBonus += 1f;
                            break;
                    }
                }
            }
        }

        if (holder.BaseKnockback == 1f && holder.AmethystBonus > 0f)
        {
            holder.RowsInTooltip++;
        }

        if (holder.BaseCritChance == 0f && holder.AquamarineBonus > 0f)
        {
            holder.RowsInTooltip++;
        }

        if (holder.BaseCritPower == (holder.HasAmmo ? 2f : 1f) && holder.JadeBonus > 0f)
        {
            holder.RowsInTooltip++;
        }

        if (holder.EmeraldBonus > 0f)
        {
            holder.RowsInTooltip++;
        }

        if (holder.GarnetBonus > 0f)
        {
            holder.RowsInTooltip++;
        }

        if (holder.TopazBonus > 0f)
        {
            holder.RowsInTooltip++;
        }

        if (holder.UsesInternalAmmo)
        {
            holder.RowsInTooltip--; // not sure why but these bows already add extra space somewhere else
        }

        return holder;
    }

    internal abstract class Holder
    {
        internal abstract float GetEffectiveDamageModifier();

        internal abstract float GetDisplayedDamage();

        internal abstract float GetEffectiveKnockbackBonus();

        internal abstract float GetDisplayedKnockbackBonus();

        internal abstract float GetEffectiveCritChanceBonus();

        internal abstract float GetDisplayedCritChanceBonus();

        internal abstract float GetEffectiveCritPowerBonus();

        internal abstract float GetDisplayedCritPowerBonus();

        internal abstract float GetEffectiveFireSpeedModifier();

        internal abstract float GetDisplayedFireSpeedModifer();

        internal abstract float GetEffectiveCooldownModifier();

        internal abstract float GetDisplayedCooldownModifer();

        internal abstract float GetEffectiveResilienceModifier();

        internal abstract float GetDisplayedResilienceModifier();

        internal abstract int GetTooltipSpaceBeforeAmmoSlots();

        internal abstract int GetStatRowsInTooltip();

        internal abstract int GetBottomExtraTooltipSpace();
    }

    internal class SlingshotStats : Holder
    {
        public float DamageMod { get; internal set; }

        public int AmmoDamage { get; internal set; }

        public float RubyBonus { get; internal set; }

        public float KnockbackBonus { get; internal set; }

        public float AmethystBonus { get; internal set; }

        public float AquamarineBonus { get; internal set; }

        public float JadeBonus { get; internal set; }

        public float EmeraldBonus { get; internal set; }

        public float GarnetBonus { get; internal set; }

        public float TopazBonus { get; internal set; }

        public int RowsInTooltip { get; internal set; }

        internal override float GetEffectiveDamageModifier()
        {
            return this.RubyBonus + 1;
        }

        internal override float GetDisplayedDamage()
        {
            if (this.AmmoDamage <= 0)
            {
                return this.DamageMod;
            }

            var minDamage = (int)(this.AmmoDamage / 2 * (this.DamageMod + 1f) * (this.RubyBonus + 1));
            var maxDamage = (int)((this.AmmoDamage + 2) * (this.DamageMod + 1f) * (this.RubyBonus + 1));
            return ((uint)maxDamage << 16) | (uint)minDamage;
        }

        internal override float GetEffectiveKnockbackBonus()
        {
            return this.AmethystBonus;
        }

        internal override float GetDisplayedKnockbackBonus()
        {
            return this.KnockbackBonus + this.AmethystBonus;
        }

        internal override float GetEffectiveCritChanceBonus()
        {
            return this.AquamarineBonus;
        }

        internal override float GetDisplayedCritChanceBonus()
        {
            return Utility.Clamp(this.AquamarineBonus, 0f, 1f);
        }

        internal override float GetEffectiveCritPowerBonus()
        {
            return this.JadeBonus;
        }

        internal override float GetDisplayedCritPowerBonus()
        {
            return this.JadeBonus;
        }

        internal override float GetEffectiveFireSpeedModifier()
        {
            return 10f / (10f + this.EmeraldBonus);
        }

        internal override float GetDisplayedFireSpeedModifer()
        {
            return this.EmeraldBonus * 0.1f;
        }

        internal override float GetEffectiveCooldownModifier()
        {
            return 1f - (this.GarnetBonus * 0.1f);
        }

        internal override float GetDisplayedCooldownModifer()
        {
            return this.GarnetBonus * 0.1f;
        }

        internal override float GetEffectiveResilienceModifier()
        {
            return 10f / (10f + this.TopazBonus);
        }

        internal override float GetDisplayedResilienceModifier()
        {
            return this.TopazBonus * 0.1f;
        }

        internal override int GetTooltipSpaceBeforeAmmoSlots()
        {
            return 12;
        }

        internal override int GetStatRowsInTooltip()
        {
            return this.RowsInTooltip;
        }

        internal override int GetBottomExtraTooltipSpace()
        {
            return 0;
        }
    }

    internal class BowStats : Holder
    {
        public int MinDamage { get; internal set; }

        public int MaxDamage { get; internal set; }

        public int AmmoDamage { get; internal set; }

        public float RubyBonus { get; internal set; }

        public float BaseKnockback { get; internal set; } = 1f;

        public float AmethystBonus { get; internal set; }

        public float BaseCritChance { get; internal set; }

        public float AquamarineBonus { get; internal set; }

        public float BaseCritPower { get; internal set; } = 1f;

        public float JadeBonus { get; internal set; }

        public float EmeraldBonus { get; internal set; }

        public float GarnetBonus { get; internal set; }

        public float TopazBonus { get; internal set; }

        public int RowsInTooltip { get; internal set; }

        public bool UsesInternalAmmo { get; internal set; }

        public bool HasAmmo { get; internal set; }

        public bool HasSpecialAttack { get; internal set; }

        internal override float GetEffectiveDamageModifier()
        {
            return this.RubyBonus + 1f;
        }

        internal override float GetDisplayedDamage()
        {
            var minDamage = (int)((this.MinDamage + this.AmmoDamage) * (this.RubyBonus + 1));
            var maxDamage = (int)((this.MaxDamage + this.AmmoDamage) * (this.RubyBonus + 1));
            return ((uint)maxDamage << 16) | (uint)minDamage;
        }

        internal override float GetEffectiveKnockbackBonus()
        {
            return this.AmethystBonus;
        }

        internal override float GetDisplayedKnockbackBonus()
        {
            return this.BaseKnockback + this.AmethystBonus - 0.25f;
        }

        internal override float GetEffectiveCritChanceBonus()
        {
            return this.AquamarineBonus;
        }

        internal override float GetDisplayedCritChanceBonus()
        {
            return Utility.Clamp(this.BaseCritChance + this.AquamarineBonus, 0f, 1f);
        }

        internal override float GetEffectiveCritPowerBonus()
        {
            return this.JadeBonus + 1f;
        }

        internal override float GetDisplayedCritPowerBonus()
        {
            return Utility.Clamp(this.BaseCritPower + this.JadeBonus, 1f, float.MaxValue) - (this.HasAmmo ? 2f : 1f);
        }

        internal override float GetEffectiveFireSpeedModifier()
        {
            return 10f / (10f + this.EmeraldBonus);
        }

        internal override float GetDisplayedFireSpeedModifer()
        {
            return this.EmeraldBonus * 0.1f;
        }

        internal override float GetEffectiveCooldownModifier()
        {
            return 1f - (this.GarnetBonus * 0.1f);
        }

        internal override float GetDisplayedCooldownModifer()
        {
            return this.GarnetBonus * 0.1f;
        }

        internal override float GetEffectiveResilienceModifier()
        {
            return 10f / (10f + this.TopazBonus);
        }

        internal override float GetDisplayedResilienceModifier()
        {
            return this.TopazBonus * 0.1f;
        }

        internal override int GetTooltipSpaceBeforeAmmoSlots()
        {
            return this.UsesInternalAmmo ? 0 : 12;
        }

        internal override int GetStatRowsInTooltip()
        {
            return this.RowsInTooltip;
        }

        internal override int GetBottomExtraTooltipSpace()
        {
            return 12;
        }
    }
}
