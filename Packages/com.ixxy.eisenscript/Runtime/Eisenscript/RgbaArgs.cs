using System;
using UnityEngine;

namespace Eisenscript
{
    public delegate void RgbaEventHandler(object sender, RgbaArgs args);

    public class RgbaArgs : EventArgs
    {
        public Color Rgba { get; }

        public RgbaArgs(Color rgba)
        {
            Rgba = rgba;
        }
    }
}