namespace DaLion.Shared.Classes;

/// <summary>Counts down from a value in milliseconds.</summary>
public sealed class Timer
{
    private readonly Func<int> _getCurrent;
    private readonly Action<int> _setCurrent;
    private readonly Action? _callback;

    /// <summary>Initializes a new instance of the <see cref="Timer"/> class.</summary>
    /// <param name="getCurrent">A <see cref="Func{TResult}"/> that returns the value that should be counted down.</param>
    /// <param name="setCurrent">A <see cref="Action"/> that sets the value that should be counted down.</param>
    /// <param name="callback">An optional <see cref="Action"/> that will the triggered when the timer hits zero.</param>
    public Timer(Func<int> getCurrent, Action<int> setCurrent, Action? callback = null)
    {
        this._getCurrent = getCurrent;
        this._setCurrent = setCurrent;
        this._callback = callback;
    }

    /// <summary>Gets the current value.</summary>
    public int Current
    {
        get => this._getCurrent();
        private set => this._setCurrent(value);
    }

    /// <summary>Decrements the timer by the elapsed number of milliseconds.</summary>
    /// <returns><see langword="true"/> if the timer reached zero, otherwise <see langword="false"/>.</returns>
    public bool Decrement()
    {
        this.Current -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
        if (this.Current > 0)
        {
            return false;
        }

        this._callback?.Invoke();
        return true;
    }
}
