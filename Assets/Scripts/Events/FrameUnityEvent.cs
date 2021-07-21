using System;
using TheLuxGames.Visualizer.Models;
using UnityEngine.Events;

namespace TheLuxGames.Visualizer.Events
{
    [Serializable]
    public class FrameUnityEvent : UnityEvent<Frame> { }
}