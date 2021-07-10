using Assets.Scripts.Domain;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class IntUnityEvent : UnityEvent<int>
{ 
    
}

namespace Assets.Scripts
{
    public class SoccerPlayerComponent : ObjectComponent<SoccerPlayer>
    {
        [SerializeField] private IntUnityEvent _onTeamIdChanged = new IntUnityEvent();

        public override SoccerPlayer Value
        {
            get => base.Value; set
            {
                if (value != null && (value.Id != Value.Id || Value == null))
                    _onTeamIdChanged.Invoke(value.TeamId);
                base.Value = value;
            }
        }
    }
}