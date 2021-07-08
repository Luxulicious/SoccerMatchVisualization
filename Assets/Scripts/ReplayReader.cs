using Assets.Scripts.Domain;
using Sirenix.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TheLuxGames.Visualizer.Domain;
using UnityEngine;

namespace Assets.Scripts
{

    //TODO Maybe make the delimiters just char arrays by default
    public abstract class ReplayReader<TReplay, TBall, TPlayer> : IFileReader<TReplay> 
        where TReplay : Replay, new() 
        where TBall : Ball, new()
        where TPlayer : Player, new()
    {
        private string frameCountDelimiter => ":";
        private string ballPropertyDelimiter => ",";

        private string frameCountPattern => $"^(\\d+)[^{frameCountDelimiter}]";
        private string ballPattern => "((?<=;:)(.+)(?=;:))";
        private string playersPattern => $"((?<=:)(.+)(?=(;:)({ballPattern});:))";
        private string playerDelimiter => ";";
        private string playerPropertyDelimiter => ",";

        public virtual TReplay ReadFromFile(string path)
        {
            TReplay replay = new TReplay();
            using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var frame = new Frame();
                    //Get frame count
                    string frameCountStr = ReadFrameCount(line);
                    frame.FrameIndex = int.Parse(frameCountStr);

                    //Get balls
                    string[] ballsStr = ReadBalls(line);
                    foreach (var ballStr in ballsStr)
                    {
                       TBall ball = ConstructBall(ballStr);
                       frame.Objects.Add(ball);
                    }

                    //Get players
                    string[] playersPerReplayStr= ReadPlayersPerReplay(line);
                    foreach (var playersPerFrameStr in playersPerReplayStr)
                    {
                        string[] playersStr = ReadPlayersPerFrame(playersPerFrameStr);
                        foreach (var playerStr in playersStr)
                        {
                            TPlayer player = ConstructPlayer(playerStr);
                            frame.Objects.Add(player);
                        }
                    }
                    replay.Frames.Add(frame);
                }
            }
            return replay;
        }

        private TPlayer ConstructPlayer(string line)
        {
            string[] propertiesStr = line.Split(playerPropertyDelimiter.ToCharArray());
            int id = int.Parse(propertiesStr[1]);
            Vector3 position = new Vector3(float.Parse(propertiesStr[3]), float.Parse(propertiesStr[4]), 0);
            float velocity = float.Parse(propertiesStr[5]);
            return new TPlayer()
            {
                Id = id,
                Position = position,
                Velocity = velocity
            };
        }

        private string[] ReadPlayersPerFrame(string line)
        {
            return line.Split(playerDelimiter.ToCharArray());
        }

        private string[] ReadPlayersPerReplay(string line)
        {
            return Regex.Matches(line, playersPattern, RegexOptions.Singleline).Cast<Match>()
                .Select(m => m.Value)
                .ToArray();
        }

        private TBall ConstructBall(string ballStr)
        {
            string[] ballPropertiesStr = ballStr.Split(ballPropertyDelimiter.ToCharArray());
            float velocity = float.Parse(ballPropertiesStr[3]);
            float xPos = float.Parse(ballPropertiesStr[0]);
            float yPos = float.Parse(ballPropertiesStr[1]);
            float zPos = float.Parse(ballPropertiesStr[2]);
            Vector3 position = new Vector3(xPos, yPos, zPos);
            var ball = new TBall()
            {
                Position = position,
                Velocity = velocity
            };
            return ball;
        }

        private string ReadFrameCount(string line)
        {
            var frameCount = Regex.Match(line, frameCountPattern, RegexOptions.Singleline).Value;
            frameCount = frameCount.Trim(frameCountDelimiter.ToCharArray());
            return frameCount;
        }

        private string[] ReadBalls(string line)
        {
            return Regex.Matches(line, ballPattern, RegexOptions.Singleline).Cast<Match>()
                .Select(m => m.Value)
                .ToArray(); 
        }
    }

    public class SoccerReplayReader : ReplayReader<SoccerReplay, SoccerBall, SoccerPlayer>
    {

    }
}
