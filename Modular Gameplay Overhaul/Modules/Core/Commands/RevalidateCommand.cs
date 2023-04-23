namespace DaLion.Overhaul.Modules.Core.Commands;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Collections;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class RevalidateCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="RevalidateCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal RevalidateCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "revalidate" };

    /// <inheritdoc />
    public override string Documentation => "Force a full revalidation of all modules.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        EnumerateModules().ForEach(module => module.Revalidate());
        Log.I("Revalidation completed.");
    }
}
