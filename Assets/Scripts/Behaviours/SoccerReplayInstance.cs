using System;
using TheLuxGames.Visualizer.Models.Soccer;
using UnityEngine;
using Object = TheLuxGames.Visualizer.Models.Object;

namespace TheLuxGames.Visualizer.Behaviours.Soccer
{
    public class SoccerReplayInstance : ReplayInstance
    {
        protected override GameObject InstantiateObject(Object o, Type t, Transform parentTransform)
        {
            var gameObject = base.InstantiateObject(o, t, parentTransform);
            if (t == typeof(SoccerBall)) AddObjectComponent<SoccerBall, SoccerBallComponent>(gameObject, o);
            else if (t == typeof(SoccerPlayer))
            {
                AddObjectComponent<SoccerPlayer, SoccerPlayerComponent>(gameObject, o);
            }
            return gameObject;
        }
    }
}