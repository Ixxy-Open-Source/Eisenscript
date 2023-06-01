using System;
using UnityEngine;

namespace Eisenscript
{
    public delegate void CameraInfoHandler(object sender, CameraInfoArgs args);
    public class CameraInfoArgs : EventArgs
    {
        public CameraInfo CamInfo { get; }

        public CameraInfoArgs(CameraInfo camInfo)
        {
            CamInfo = camInfo;
        }
    }
}