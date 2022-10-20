#pragma warning disable CS1591
namespace DaLion.Common.Integrations.JsonAssets;

#region using directives

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>The API provided by Json Assets.</summary>
public interface IJsonAssetsApi
{
    event EventHandler ItemsRegistered;

    event EventHandler IdsAssigned;

    event EventHandler AddedItemsToShop;

    event EventHandler IdsFixed;

    void LoadAssets(string path);

    void LoadAssets(string path, ITranslationHelper translations);

    int GetObjectId(string name);

    int GetCropId(string name);

    int GetFruitTreeId(string name);

    int GetBigCraftableId(string name);

    int GetHatId(string name);

    int GetWeaponId(string name);

    int GetClothingId(string name);

    IDictionary<string, int> GetAllObjectIds();

    IDictionary<string, int> GetAllCropIds();

    IDictionary<string, int> GetAllFruitTreeIds();

    IDictionary<string, int> GetAllBigCraftableIds();

    IDictionary<string, int> GetAllHatIds();

    IDictionary<string, int> GetAllWeaponIds();

    IDictionary<string, int> GetAllClothingIds();

    List<string> GetAllObjectsFromContentPack(string cp);

    List<string> GetAllCropsFromContentPack(string cp);

    List<string> GetAllFruitTreesFromContentPack(string cp);

    List<string> GetAllBigCraftablesFromContentPack(string cp);

    List<string> GetAllHatsFromContentPack(string cp);

    List<string> GetAllWeaponsFromContentPack(string cp);

    List<string> GetAllClothingFromContentPack(string cp);

    List<string> GetAllBootsFromContentPack(string cp);

    bool FixIdsInItem(Item item);

    void FixIdsInItemList(List<Item> items);

    void FixIdsInLocation(GameLocation location);

    bool TryGetCustomSprite(object entity, out Texture2D texture, out Rectangle sourceRect);

    bool TryGetCustomSpriteSheet(object entity, out Texture2D texture, out Rectangle sourceRect);
}
