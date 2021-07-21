using TheLuxGames.Visualizer.Models.Soccer;
using TheLuxGames.Visualizer.Readers.Soccer;

namespace TheLuxGames.Visualizer.Behaviours.Soccer
{
    public class SoccerReplayPlayer : ReplayPlayer<SoccerReplay, SoccerBall, SoccerPlayer>
    {
        protected override void Awake()
        {
            base.Awake();
            if (Reader == null) Reader = new SoccerReplayReader();
        }
    }
}