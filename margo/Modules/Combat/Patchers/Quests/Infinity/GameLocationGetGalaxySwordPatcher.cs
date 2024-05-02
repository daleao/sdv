namespace DaLion.Overhaul.Modules.Combat.Patchers.Quests.Infinity;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using DaLion.Shared.Constants;
using DaLion.Shared.Enums;
using DaLion.Shared.Exceptions;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationGetGalaxySwordPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationGetGalaxySwordPatcher"/> class.</summary>
    internal GameLocationGetGalaxySwordPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>("getGalaxySword");
    }

    #region harmony patches

    /// <summary>Convert cursed -> blessed enchantment + galaxysoul -> infinity enchatnment.</summary>
    [HarmonyPrefix]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Preference for inner functions.")]
    private static bool GameLocationGetGalaxySwordPrefix()
    {
        if (!CombatModule.Config.Quests.EnableHeroQuest)
        {
            return true; // run original logic
        }

        try
        {
            var player = Game1.player;
            var obtained = player.Read(DataKeys.GalaxyArsenalObtained).ParseList<int>();
            if (obtained.Count == 4)
            {
                Log.W("Player was already gifted a full-set of Galaxy weapons. How did they get here?");
                return false; // don't run original logic
            }

            int? chosen = null;
            for (var i = 0; i < player.Items.Count; i++)
            {
                var item = player.Items[i];
                WeaponType type;
                switch (item)
                {
                    case MeleeWeapon weapon when !weapon.isScythe():
                        type = (WeaponType)weapon.type.Value;
                        break;
                    case Slingshot:
                        type = WeaponType.Slingshot;
                        break;
                    default:
                        continue;
                }

                var galaxy = galaxyFromWeaponType(type);
                if (obtained.Contains(galaxy))
                {
                    continue;
                }

                chosen = galaxy;
                break;
            }

            chosen ??= new[]
            {
                WeaponIds.GalaxySword,
                WeaponIds.GalaxyHammer,
                WeaponIds.GalaxyDagger,
                WeaponIds.GalaxySlingshot,
            }.Except(obtained).First();

            Item chosenAsItem = chosen.Value == WeaponIds.GalaxySlingshot
                ? new Slingshot(chosen.Value)
                : new MeleeWeapon(chosen.Value);

            Game1.flashAlpha = 1f;

            player.completelyStopAnimatingOrDoingAction();
            DelayedAction.playSoundAfterDelay("getNewSpecialItem", 750);
            player.freezePause = 4000;
            player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[]
            {
                new(57, 0), new(57, 2500, secondaryArm: false, flip: false, farmer =>
                {
                    farmer.mostRecentlyGrabbedItem = chosenAsItem;
                    Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(
                        "TileSheets\\weapons",
                        Game1.getSquareSourceRectForNonStandardTileSheet(
                            Tool.weaponsTexture,
                            16,
                            16,
                            ((Tool)chosenAsItem).IndexOfMenuItemView),
                        2500f,
                        1,
                        0,
                        farmer.Position + new Vector2(0f, -140f),
                        flicker: false,
                        flipped: false,
                        1f,
                        0f,
                        Color.White,
                        4f,
                        0f,
                        0f,
                        0f) { motion = new Vector2(0f, +0.1f) });
                }),
                new(
                    (short)player.FarmerSprite.CurrentFrame,
                    500,
                    secondaryArm: false,
                    flip: false,
                    farmer =>
                    {
                        if (!farmer.addItemToInventoryBool(chosenAsItem))
                        {
                            Game1.createItemDebris(chosenAsItem, farmer.getStandingPosition(), -1);
                        }

                        Farmer.showReceiveNewItemMessage(farmer);
                    },
                    behaviorAtEndOfFrame: true),
            });

            player.reduceActiveItemByOne();
            for (var i = 0; i < obtained.Count; i++)
            {
                player.reduceActiveItemByOne();
            }

            if (CombatModule.Config.Quests.IridiumBarsPerGalaxyWeapon > 0)
            {
                player.Items.First(i => i?.ParentSheetIndex == ObjectIds.IridiumBar).Stack -=
                    CombatModule.Config.Quests.IridiumBarsPerGalaxyWeapon;
            }

            player.Append(DataKeys.GalaxyArsenalObtained, chosen.Value.ToString());
            obtained = player.Read(DataKeys.GalaxyArsenalObtained).ParseList<int>();
            if (obtained.Count == 4)
            {
                Game1.createItemDebris(new Boots(BootsIds.SpaceBoots), player.getStandingPosition(), -1);
            }

            if (!player.mailReceived.Contains("galaxySword"))
            {
                player.mailReceived.Add("galaxySword");
            }

            player.jitterStrength = 0f;
            Game1.screenGlowHold = false;
            Reflector.GetStaticFieldGetter<Multiplayer>(typeof(Game1), "multiplayer").Invoke()
                .globalChatInfoMessage("GalaxySword", Game1.player.Name);
            return false; // don't run original logic

            int galaxyFromWeaponType(WeaponType type)
            {
                return type switch
                {
                    WeaponType.StabbingSword or WeaponType.DefenseSword => WeaponIds.GalaxySword,
                    WeaponType.Dagger => WeaponIds.GalaxyDagger,
                    WeaponType.Club => WeaponIds.GalaxyHammer,
                    WeaponType.Slingshot => WeaponIds.GalaxySlingshot,
                    _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<WeaponType, int>(type),
                };
            }
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
