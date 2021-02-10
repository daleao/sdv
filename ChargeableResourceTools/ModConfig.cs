using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using TheLion.AwesomeTools.Framework.Configs;

namespace TheLion.AwesomeTools
{
	public class ModConfig
	{
		/// <summary>The Axe features to enable.</summary>
		public AxeConfig AxeConfig { get; set; } = new();
		/// <summary>The Pickaxe features to enable.</summary>
		public PickaxeConfig PickaxeConfig { get; set; } = new();

		/// <summary>Whether the mod requires an additional hotkey to activate.</summary>
		public bool RequireHotkey { get; set; } = true;
		public KeybindList Hotkey { get; set; } = KeybindList.ForSingle(SButton.LeftShift);

		/// <summary>How much stamina the shockwave should consume.</summary>
		public int StaminaCostMultiplier { get; set; } = 1;
	}
}
