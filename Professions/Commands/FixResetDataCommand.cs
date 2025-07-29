namespace DaLion.Professions.Commands;

#region using directives

using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Collections;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="FixSlingshotsCommand"/> class.</summary>
/// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
[UsedImplicitly]
internal sealed class FixResetDataCommand(CommandHandler handler)
    : ConsoleCommand(handler)
{
    /// <inheritdoc />
    public override string[] Triggers { get; } = ["fix_reset_data"];

    /// <inheritdoc />
    public override string Documentation =>
        "Fixes mismatch between acquired professions and number of times the corresponding skill has been reset.";

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        Dictionary<string, int> resetCountBySkill = [];
        foreach (ISkill vanilla in VanillaSkill.List)
        {
            if (vanilla.AcquiredProfessions.Length == 0 ||
                (vanilla.AcquiredProfessions.Length == 1 && vanilla.CurrentLevel >= 10))
            {
                continue;
            }

            var count = vanilla.AcquiredProfessions.Length - 1;
            if (vanilla.CurrentLevel < 10)
            {
                count++;
            }

            resetCountBySkill[vanilla.StringId] = count;
        }

        foreach (ISkill custom in CustomSkill.Loaded.Values)
        {
            if (custom.AcquiredProfessions.Length == 0 ||
                (custom.AcquiredProfessions.Length == 1 && custom.CurrentLevel >= 10))
            {
                continue;
            }

            var count = custom.AcquiredProfessions.Length - 1;
            if (custom.CurrentLevel < 10)
            {
                count++;
            }

            resetCountBySkill[custom.StringId] = count;
        }

        Data.Write(Game1.player, DataKeys.ResetCountBySkill, resetCountBySkill.Stringify());
        return true;
    }
}
