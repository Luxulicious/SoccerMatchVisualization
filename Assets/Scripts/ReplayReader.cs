using Assets.Scripts.Domain;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TheLuxGames.Visualizer.Domain;
using UnityEngine;
using Object = TheLuxGames.Visualizer.Domain.Object;

namespace Assets.Scripts
{

    //TODO Maybe make the delimiters just char arrays by default
    //TODO Maybe make the pattern detection properties and methods also virtual... 
    public abstract class ReplayReader<TReplay, TBall, TPlayer> : IReplayReader<TReplay> 
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

        public virtual float CoordinateRatio => 100;
        public virtual bool SwitchYZCoordinates => true;

        public virtual TReplay ReadFromFile(string path)
        {
            TReplay replay = new TReplay();
            using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                string frame;
                while ((frame = sr.ReadLine()) != null)
                {
                    Frame f = GetFrame(frame);
                    replay.Frames.Add(f.FrameIndex, f);
                }
            }
            return replay;
        }

        protected virtual Frame GetFrame(string frame)
        {
            var f = new Frame();
            //Get frame count
            int frameIndex = GetFrameIndex(frame);
            f.FrameIndex = frameIndex;
            //Get objects
            //TODO Maybe change this to something more insert effecient 
            List<Object> objects = GetObjects(frame);
            f.Objects.AddRange(objects);
            return f;
        }

        protected virtual List<Object> GetObjects(string frame)
        {
            List<Object> objects = new List<Object>();

            //Get balls
            TBall[] balls = GetBalls(frame);
            objects.AddRange(balls);

            //Get players
            TPlayer[] players = GetPlayers(frame);
            objects.AddRange(players);

            //TODO This is probably ineffecient
            if (CoordinateRatio != 1)
            {
                foreach (var o in objects)
                    o.Position /= CoordinateRatio;
            }

            //TODO This is probably ineffecient as well, recreating the entire vector3 everytime now...
            if (SwitchYZCoordinates)
            {
                foreach (var o in objects)
                    o.Position = new Vector3(o.Position.x, o.Position.z, o.Position.y);
            }

            return objects;
        }

        protected virtual TPlayer[] GetPlayers(string frame)
        {
            TPlayer[] players;
            string[] playersStr = ReadPlayers(frame);
            players = new TPlayer[playersStr.Length];
            if (typeof(TPlayer) == typeof(TeamPlayer) || typeof(TPlayer).IsSubclassOf(typeof(TeamPlayer)))
            {
                for (int i = 0; i < playersStr.Length; i++)
                {
                    players[i] = ConstructTeamPlayer(playersStr[i]);
                }
            }
            else
            {
                for (int i = 0; i < playersStr.Length; i++)
                {
                    players[i] = ConstructPlayer(playersStr[i]);
                }
            }

            return players;
        }

        private TPlayer ConstructTeamPlayer(string player)
        {
            string[] properties;
            TeamPlayer tp = ConstructPlayer(player, out properties) as TeamPlayer;
            tp.PlayerNumber = int.Parse(properties[2]);
            tp.TeamId = int.Parse(properties[0]);
            return tp as TPlayer;
        }

        protected virtual TBall[] GetBalls(string frame)
        {
            TBall[] balls;
            string[] ballsStr = ReadBalls(frame);
            balls = new TBall[ballsStr.Length];
            for (int i = 0; i < ballsStr.Length; i++)
            {
                string ballStr = (string)ballsStr[i];
                TBall ball = ConstructBall(ballStr);
                balls[i] = (ball);
            }

            return balls;
        }

        private int GetFrameIndex(string frame)
        {
            string frameCountStr = ReadFrameIndex(frame);
            return int.Parse(frameCountStr);
        }

        private TPlayer ConstructPlayer(string player)
        {
            return ConstructPlayer(player, out string[] _);
        }

        private TPlayer ConstructPlayer(string player, out string[] propertiesStr)
        {
            propertiesStr = player.Split(playerPropertyDelimiter.ToCharArray());
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

        private string[] ReadPlayers(string frame)
        {
            string playersStr = Regex.Match(frame, playersPattern, RegexOptions.Singleline).Value;
            return playersStr.Split(playerDelimiter.ToCharArray());
        }

        private TBall ConstructBall(string ball)
        {
            string[] ballPropertiesStr = ball.Split(ballPropertyDelimiter.ToCharArray());
            float velocity = float.Parse(ballPropertiesStr[3]);
            float xPos = float.Parse(ballPropertiesStr[0]);
            float yPos = float.Parse(ballPropertiesStr[1]);
            float zPos = float.Parse(ballPropertiesStr[2]);
            Vector3 position = new Vector3(xPos, yPos, zPos);
            var b = new TBall()
            {
                Position = position,
                Velocity = velocity
            };
            return b;
        }

        private string ReadFrameIndex(string frame)
        {
            var frameCount = Regex.Match(frame, frameCountPattern, RegexOptions.Singleline).Value;
            frameCount = frameCount.Trim(frameCountDelimiter.ToCharArray());
            return frameCount;
        }

        private string[] ReadBalls(string frame)
        {
            return Regex.Matches(frame, ballPattern, RegexOptions.Singleline).Cast<Match>()
                .Select(m => m.Value)
                .ToArray(); 
        }
    }

    public class SoccerReplayReader : ReplayReader<SoccerReplay, SoccerBall, SoccerPlayer>
    {

    }
}
