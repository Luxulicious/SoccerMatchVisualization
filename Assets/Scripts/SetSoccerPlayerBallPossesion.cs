using Assets.Scripts;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSoccerPlayerBallPossesion : MonoBehaviour
{
    [SerializeField, Required] private SoccerBallComponent _ballComponent;

    public void Set(SoccerPlayerComponent current, SoccerPlayerComponent previous)
    {
        if(current != null) current.BallInPossesion = _ballComponent;
        if(previous != null) previous.BallInPossesion = null;
    }
}
