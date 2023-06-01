using System;
using UnityEngine;

namespace Eisenscript
{
    public delegate void DrawEventHandler(object sender, DrawArgs args);
    public class DrawArgs : EventArgs
    {
        public Matrix4x4 Matrix { get; }
        public Color Rgba { get; }
        public TokenType Type { get; }

        public DrawArgs(TokenType type, Matrix4x4 mtx, Color rgba)
        {
            Type = type;
            Matrix = mtx;
            Rgba = rgba;
        }
    }
}
