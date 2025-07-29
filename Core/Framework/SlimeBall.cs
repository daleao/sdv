namespace DaLion.Core.Framework;

using System;
using System.Collections.Generic;
using DaLion.Shared.Classes;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions.Collections;
using Microsoft.Xna.Framework;
using StardewValley.Extensions;

public class SlimeBall
{
    public SlimeBall(SObject slimeBall, Vector2? tile = null)
    {
        var split = slimeBall.orderData.Value.Split('/');
        this.SlimeColor = new Color(uint.Parse(split[0]));
        this.IsTiger = bool.Parse(split[1]);
        this.IsFirstGeneration = bool.Parse(split[2]);
        this.SpecialNumber = int.Parse(split[3]);
        this.Tile = tile ?? slimeBall.TileLocation;
    }

    public Color SlimeColor { get; }

    public bool IsTiger { get; }

    public bool IsFirstGeneration { get; }

    public int SpecialNumber { get; }

    public Vector2 Tile { get; }

    public virtual Dictionary<string, int> GetDrops()
    {
        Dictionary<string, int> drops = [];
        var r = new Random(Guid.NewGuid().GetHashCode());
        if (this.IsTiger)
        {
            if (r.NextBool(0.1))
            {
                drops.Add(QIDs.TaroTuber, 1);
                while (r.NextBool())
                {
                    drops[QIDs.TaroTuber]++;
                }
            }
            else if (r.NextBool(0.1))
            {
                drops.Add(QIDs.Ginger, 1);
            }
            else if (r.NextBool(0.02))
            {
                drops.Add(QIDs.PineappleSeeds, 1);
                while (r.NextBool())
                {
                    drops[QIDs.PineappleSeeds]++;
                }
            }
            else if (r.NextBool(0.006))
            {
                drops.Add(QIDs.MangoSapling, 1);
            }

            drops.Add(QIDs.Sap, r.Next(15, 26));
            if (!this.IsFirstGeneration)
            {
                return drops;
            }

            drops.Add(QIDs.Jade, 1);
            return drops;
        }

        var purple = new ColorRange([151, 255], [0, 49], [181, 255]);
        if (purple.Contains(this.SlimeColor) && this.SpecialNumber % (this.IsFirstGeneration ? 4 : 2) == 0)
        {
            drops.Add(QIDs.IridiumOre, r.Next(5, 11));
            if (this.IsFirstGeneration)
            {
                while (r.NextBool(0.072))
                {
                    drops.AddOrUpdate(QIDs.IridiumBar, 1, (a, b) => a + b);
                }
            }
        }

        var brown = new ColorRange([50, 100], [25, 50], [0, 25]);
        if (brown.Contains(this.SlimeColor))
        {
            drops.Add(QIDs.Wood, r.Next(30, 61));
            while (r.NextBool(0.1))
            {
                drops.AddOrUpdate(QIDs.Hardwood, r.Next(2, 5), (a, b) => a + b);
            }
        }

        var black = new ColorRange([0, 79], [0, 79], [0, 79]);
        if (black.Contains(this.SlimeColor))
        {
            drops.Add(QIDs.Coal, r.Next(5, 11));
            while (r.NextBool(0.05))
            {
                drops.AddOrUpdate(QIDs.Neptunite, 1, (a, b) => a + b);
            }

            while (r.NextBool(0.05))
            {
                drops.AddOrUpdate(QIDs.Bixite, 1, (a, b) => a + b);
            }
        }

        var yellow = new ColorRange([201, 255], [181, 255], [0, 49]);
        if (yellow.Contains(this.SlimeColor))
        {
            drops.Add(QIDs.GoldOre, r.Next(10, 21));
            while (r.NextBool(0.05))
            {
                drops.AddOrUpdate(QIDs.GoldBar, r.Next(1, 3), (a, b) => a + b);
            }
        }

        var red = new ColorRange([221, 255], [91, 149], [0, 49]);
        if (red.Contains(this.SlimeColor))
        {
            drops.Add(QIDs.CopperOre, r.Next(10, 21));
            while (r.NextBool(0.05))
            {
                drops.AddOrUpdate(QIDs.CopperBar, r.Next(1, 3), (a, b) => a + b);
            }
        }

        var grey = new ColorRange([151, 255], [151, 255], [151, 255]);
        if (!grey.Contains(this.SlimeColor))
        {
            return drops;
        }

        var white = new ColorRange([231, 255], [231, 255], [231, 255]);
        if (white.Contains(this.SlimeColor))
        {
            if (this.SlimeColor.R % 2 == 1)
            {
                drops.Add(QIDs.RefineQuartz, r.Next(2, 5));
                if (this.SlimeColor.G % 2 == 1)
                {
                    drops[QIDs.RefineQuartz] += r.Next(2, 5);
                }
            }
            else
            {
                drops.Add(QIDs.IronOre, r.Next(10, 21));
                while (r.NextBool(0.05))
                {
                    drops.AddOrUpdate(QIDs.IronBar, r.Next(1, 3), (a, b) => a + b);
                }
            }

            if ((this.SlimeColor.R % 2 == 0 && this.SlimeColor.G % 2 == 0 && this.SlimeColor.B % 2 == 0) ||
                this.SlimeColor == Color.White)
            {
                drops.Add(QIDs.Diamond, r.Next(1, 3));
            }
        }
        else
        {
            drops.Add(QIDs.Stone, r.Next(20, 41));
        }

        return drops;
    }
}
