using Sirenix.OdinInspector;
using TheLuxGames.Visualizer.Events;
using TheLuxGames.Visualizer.Models.Soccer;
using UnityEngine;
using UnityEngine.Events;

namespace TheLuxGames.Visualizer.Behaviours.Soccer
{
    public class SoccerPlayerComponent : ObjectComponent<SoccerPlayer>
    {
        [SerializeField, FoldoutGroup("Events")] private IntUnityEvent _onTeamIdChanged = new IntUnityEvent();
        [SerializeField, ReadOnly] private SoccerBallComponent _ballInPossesion;

        [SerializeField, FoldoutGroup("Events")] private UnityEvent _onBallPossesionLost = new UnityEvent();
        [SerializeField, FoldoutGroup("Events")] private UnityEvent _onBallPossesionGained = new UnityEvent();

        public override SoccerPlayer Value
        {
            get => base.Value; set
            {
                if (value != null && (value.Id != Value.Id || Value == null))
                    _onTeamIdChanged.Invoke(value.TeamId);
                base.Value = value;
            }
        }

        public SoccerBallComponent BallInPossesion
        {
            get => _ballInPossesion; set
            {
                if (_ballInPossesion == null && value != null)
                    _onBallPossesionGained.Invoke();
                else if (_ballInPossesion != null && value == null)
                    _onBallPossesionLost.Invoke();
                _ballInPossesion = value;
            }
        }
    }
}