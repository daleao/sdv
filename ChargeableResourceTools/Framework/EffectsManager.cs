using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using TheLion.AwesomeTools.Framework.ToolEffects;

namespace TheLion.AwesomeTools.Framework
{
	public class EffectsManager
	{
		private readonly AxeEffect _axe;
		private readonly PickaxeEffect _pickaxe;
		private readonly int _multiplier;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The overal mod settings.</param>
		/// <param name="modRegistry"
		public EffectsManager(ModConfig config, IModRegistry modRegistry)
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
						who.stamina -= (who.toolPower - who.ForagingLevel * 0.1f) * (who.toolPower - 1) * _multiplier;
						_axe.SpreadToolEffect(tool, actionTile, _axe.Config.RadiusAtEachLevel, location, who);
					}
					break;
				case Pickaxe:
					if (_pickaxe.Config.EnablePickaxeCharging)
					{
						who.stamina -= (who.toolPower - who.MiningLevel * 0.1f) * (who.toolPower - 1) * _multiplier;
						_pickaxe.SpreadToolEffect(tool, actionTile, _pickaxe.Config.RadiusAtEachLevel, location, who);
					}
					break;
				default:
					break;
			}
		}
	}
}
