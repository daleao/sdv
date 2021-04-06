using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;

namespace TheLion.AwesomeTools
{
	/// <summary>Manages control between each tool.</summary>
	public class EffectsManager
	{
		private readonly AxeEffect _axe;
		private readonly PickaxeEffect _pickaxe;
		private readonly float _multiplier;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The overal mod settings.</param>
		/// <param name="modRegistry">Metadata about loaded mods.</param>
		public EffectsManager(ToolConfig config, IModRegistry modRegistry)
		{
			_axe = new AxeEffect(config.AxeConfig, modRegistry);
			_pickaxe = new PickaxeEffect(config.PickaxeConfig, modRegistry);
			_multiplier = config.StaminaCostMultiplier;
		}

		/// <summary>Do awesome shit with your tools.</summary>
		public void DoShockwave(Vector2 actionTile, Tool tool, GameLocation location, Farmer who)
		{
			switch (tool)
			{
				case Axe:
					if (_axe.Config.EnableAxeCharging)
					{
						_axe.SpreadToolEffect(tool, actionTile, _axe.Config.RadiusAtEachPowerLevel, _multiplier, location, who);
					}
					break;

				case Pickaxe:
					if (_pickaxe.Config.EnablePickaxeCharging)
					{
						_pickaxe.SpreadToolEffect(tool, actionTile, _pickaxe.Config.RadiusAtEachPowerLevel, _multiplier, location, who);
					}
					break;
			}
		}
	}
}