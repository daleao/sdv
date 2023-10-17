namespace DaLion.Overhaul.Modules.Combat.Commands;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Combat.StatusEffects;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;
using StardewValley;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class ClearStatusCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="ClearStatusCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal ClearStatusCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "clear_status" };

    /// <inheritdoc />
    public override string Documentation => "Clears all cached status effects.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (args.Length <= 0)
        {
            Monster_Bleeding.Values.Clear();
            Monster_Burned.Values.Clear();
            Monster_Poisoned.Values.Clear();
            Monster_Slowed.Values.Clear();
            BleedAnimation.BleedAnimationByMonster.Clear();
            BurnAnimation.BurnAnimationsByMonster.Clear();
            PoisonAnimation.PoisonAnimationByMonster.Clear();
            SlowAnimation.SlowAnimationByMonster.Clear();
            StunAnimation.StunAnimationByMonster.Clear();
            return;
        }

        while (args.Length > 0)
        {
            switch (args[0].ToLowerInvariant())
            {
                case "bleed":
                    Monster_Bleeding.Values.Clear();
                    BleedAnimation.BleedAnimationByMonster.Clear();
                    break;
                case "burn":
                    Monster_Burned.Values.Clear();
                    BurnAnimation.BurnAnimationsByMonster.Clear();
                    break;
                case "chill":
                    Monster_Chilled.Values.Clear();
                    break;
                case "fear":
                    Monster_Feared.Values.Clear();
                    break;
                case "freeze":
                    Monster_Frozen.Values.Clear();
                    break;
                case "poison":
                    Monster_Poisoned.Values.Clear();
                    PoisonAnimation.PoisonAnimationByMonster.Clear();
                    break;
                case "slow":
                    Monster_Slowed.Values.Clear();
                    SlowAnimation.SlowAnimationByMonster.Clear();
                    break;
                case "stun":
                    StunAnimation.StunAnimationByMonster.Clear();
                    break;
            }

            args = args.Skip(1).ToArray();
        }
    }
}
