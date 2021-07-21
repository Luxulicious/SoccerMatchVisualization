using Sirenix.OdinInspector;
using UnityEngine;

namespace TheLuxGames.Visualizer.Behaviours.Soccer
{
    public class SetSoccerPlayerBallPossesion : MonoBehaviour
    {
        [SerializeField, Required] private SoccerBallComponent _ballComponent;

        public void Set(SoccerPlayerComponent current, SoccerPlayerComponent previous)
        {
            if (current != null) current.BallInPossesion = _ballComponent;
            if (previous != null) previous.BallInPossesion = null;
        }
    }
}