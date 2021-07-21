using System;
using TheLuxGames.Visualizer.Domain;

namespace Assets.Scripts.Domain
{
    [Serializable]
    public class SoccerReplay : Replay { }

    [Serializable]
    public class SoccerPlayer : TeamPlayer { }

    [Serializable]
    public class SoccerBall : Ball { }
}