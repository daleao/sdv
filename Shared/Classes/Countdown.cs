namespace DaLion.Shared.Classes;

/// <summary>Counts down from a baseline value.</summary>
internal sealed class Countdown
{
    /// <summary>Initializes a new instance of the <see cref="Countdown"/> class.Construct an instance.</summary>
    /// <param name="initial">The initial value from which to count down.</param>
    public Countdown(int initial)
    {
        this.Initial = initial;
        this.Current = initial;
    }

    /// <summary>Gets the initial value from which to count down.</summary>
    public int Initial { get; }

    /// <summary>Gets the current value.</summary>
    public int Current { get; private set; }

    /// <summary>Reduces the current value by one.</summary>
    /// <returns>Returns whether the value was decremented (i.e. wasn't already zero).</returns>
    public bool Decrement()
    {
        if (this.Current <= 0)
        {
            return false;
        }

        --this.Current;
        return true;
    }

    /// <summary>Restarts the countdown.</summary>
    public void Reset()
    {
        this.Current = this.Initial;
    }
}
