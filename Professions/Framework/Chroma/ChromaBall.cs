namespace DaLion.Professions.Framework.Chroma;

using System;
using System.Collections.Generic;
using DaLion.Core.Framework;
using DaLion.Shared.Classes;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;

internal sealed class ChromaBall(SObject slimeBall, Vector2? tile = null) : SlimeBall(slimeBall, tile)
{
    public override Dictionary<string, int> GetDrops()
    {
        var drops = base.GetDrops();
        var r = new Random(Guid.NewGuid().GetHashCode());
        var closest = ChromaMapper.ItemsByColor.Keys.ArgMin(color => color.L1Distance(this.SlimeColor));
        var range = new ColorRange(
            [(byte)(closest.R - 10), (byte)(closest.R + 10)],
            [(byte)(closest.G - 10), (byte)(closest.G + 10)],
            [(byte)(closest.B - 10), (byte)(closest.B + 10)]);
        if (range.Contains(closest))
        {
            drops.Add(ChromaMapper.ItemsByColor[closest].Choose(r), 1);
        }

        return drops;
    }
}
