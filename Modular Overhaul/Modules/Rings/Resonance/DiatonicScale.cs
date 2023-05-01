namespace DaLion.Overhaul.Modules.Rings.Resonance;

#region using directives

using DaLion.Shared.Extensions;

#endregion using directives

/// <summary>A heptatonic scale that includes five whole steps (whole tones) and two half steps (semitones) in each octave.</summary>
public readonly struct DiatonicScale
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DiatonicScale"/> struct with
    ///     <see cref="Gemstone.Ruby"/> as the root <see cref="Gemstone"/>.
    /// </summary>
    public DiatonicScale()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DiatonicScale"/> struct with the
    ///     specified root <see cref="Gemstone"/>.
    /// </summary>
    /// <param name="root">The <see cref="Gemstone"/> with which is the root of the <see cref="DiatonicScale"/>.</param>
    public DiatonicScale(Gemstone root)
    {
        this.Notes.ShiftLeft(Array.IndexOf(this.Notes, root));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DiatonicScale"/> struct.Constructs an instance, with the
    ///     <see cref="Gemstone"/> at the specified index in the <see cref="Gemstone.Ruby"/> scale as the root..
    /// </summary>
    /// <param name="indexInRubyScale">
    ///     The index of the root <see cref="Gemstone"/> in the canonical
    ///     <see cref="DiatonicScale"/> of <see cref="Gemstone.Ruby"/>.
    /// </param>
    public DiatonicScale(int indexInRubyScale)
    {
        this.Transpose(indexInRubyScale);
    }

    /// <summary>Gets the ordered set of <see cref="Gemstone"/>s that make up the notes in the <see cref="DiatonicScale"/>.</summary>
    public Gemstone[] Notes { get; } =
    {
        Gemstone.Ruby,
        Gemstone.Aquamarine,
        Gemstone.Amethyst,
        Gemstone.Garnet,
        Gemstone.Emerald,
        Gemstone.Jade,
        Gemstone.Topaz,
    };

    /// <summary>Gets the first <see cref="Gemstone"/> in the <see cref="DiatonicScale"/>.</summary>
    public Gemstone Root => this.Notes[0];

    /// <summary>Gets the <see cref="Gemstone"/> at the specified <paramref name="index"/>.</summary>
    /// <param name="index">A <see cref="int"/> index.</param>
    public Gemstone this[int index] => this.Notes[index % 7];

    /// <summary>Gets the index of the specified <see cref="Gemstone"/> in the current <see cref="DiatonicScale"/>.</summary>
    /// <param name="gemstone">Some <see cref="Gemstone"/>.</param>
    /// <returns>The <see cref="int"/> index of the <paramref name="gemstone"/> in the current <seealso cref="DiatonicScale"/>.</returns>
    public int IndexOf(Gemstone gemstone)
    {
        return Array.IndexOf(this.Notes, gemstone);
    }

    /// <summary>Shift all notes to the left by the specified <paramref name="count"/>.</summary>
    /// <param name="count">The number of shifts to perform.</param>
    public void Transpose(int count)
    {
        this.Notes.ShiftLeft(count);
    }

    /// <summary>Shift all notes to the left by the specified <paramref name="interval"/>.</summary>
    /// <param name="interval">The <see cref="IntervalNumber"/>.</param>
    public void Transpose(IntervalNumber interval)
    {
        this.Transpose((int)interval);
    }
}
