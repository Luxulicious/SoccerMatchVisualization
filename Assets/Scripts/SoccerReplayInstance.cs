using Assets.Scripts;
using Assets.Scripts.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = TheLuxGames.Visualizer.Domain.Object;

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
