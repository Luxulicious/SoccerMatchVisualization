using Assets.Scripts;
using System;
using UnityEngine.Events;

[Serializable]
public class SPCSPCUnityEvent : UnityEvent<SoccerPlayerComponent, SoccerPlayerComponent> { }

public class FindNearestSoccerPlayer : FindNearestObjectComponent<SoccerPlayerComponent, SPCSPCUnityEvent>
{
}