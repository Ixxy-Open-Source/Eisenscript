using System.Collections.Generic;
using UnityEngine;

namespace Eisenscript
{
    public class ColorPool
    {
        public TokenType Scheme { get; }
        public List<Color>? ColorList { get; }

        public ColorPool() : this(TokenType.RandomRgb) {}
        public ColorPool(TokenType scheme, List<Color>? colorList = null)
        {
            ColorList = colorList;
            Scheme = scheme;
        }

        public Color ChooseColor(System.Random rnd)
        {
            Color GreyScale(byte val)
            {
                return new Color(val, val, val);
            }
#pragma warning disable CS8509
            return Scheme switch
            {
                TokenType.RandomHue =>
                    Color.HSVToRGB((float)rnd.NextDouble() * 360, 1.0f, 1.0f),

                TokenType.RandomRgb =>
                    new Color(
                        (byte)(rnd.NextDouble() * 256),
                        (byte)(rnd.NextDouble() * 256),
                        (byte)(rnd.NextDouble() * 256)),

                TokenType.GreyScale =>
                    GreyScale((byte)(rnd.NextDouble() * 256)),

                TokenType.List =>
                    ColorList![rnd.Next(ColorList.Count)],
            };
#pragma warning restore CS8509
        }

        internal static ColorPool FromScan(Scan scan)
        {
            if (scan.Peek().Type != TokenType.List)
            {
                var line = scan.Peek().Line;
                var type = scan.Next().Type;
                if (type < TokenType.RandomHue || type > TokenType.GreyScale)
                {
                    throw new ParserException("Unrecognized Colorpool type", line);
                }
                return new ColorPool(type);
            }

            // Color list
            scan.Advance();
            List<Color> list = new();
            while (true)
            {
                list.Add(scan.NextRgba());
                if (scan.Peek().Type != TokenType.Comma)
                {
                    break;
                }

                scan.Advance();
            }

            return new ColorPool(TokenType.List, list);
        }
    }
}
