using System;
using TheLuxGames.Visualizer.Behaviours.Soccer;
using UnityEngine.Events;

namespace TheLuxGames.Visualizer.Events
{
    [Serializable]
    public class SPCSPCUnityEvent : UnityEvent<SoccerPlayerComponent, SoccerPlayerComponent> { }
}