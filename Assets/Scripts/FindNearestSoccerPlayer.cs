using Assets.Scripts;
using Assets.Scripts.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

[Serializable]
public class DoubleSoccerPlayerComponentUnityEvent : UnityEvent<SoccerPlayerComponent, SoccerPlayerComponent> { }

public class FindNearestSoccerPlayer : FindNearestObjectComponent<SoccerPlayerComponent, DoubleSoccerPlayerComponentUnityEvent>
{

}

