using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TheLuxGames.Visualizer.Models;
using UnityEngine;
using Object = TheLuxGames.Visualizer.Models.Object;

namespace TheLuxGames.Visualizer.Readers
{
    public abstract class ReplayReader<TReplay, TBall, TPlayer> : IReplayReader<TReplay>
        where TReplay : Replay, new()
        where TBall : Ball, new()
        where TPlayer : Player, new()
    {
        public virtual string FrameIndexPattern => $"^(\\d+)[^{_frameIndexDelimiter}]";
        public virtual string BallPattern => "((?<=;:)(.+)(?=;:))";
        public virtual string PlayersPattern => $"((?<=:)(.+)(?=(;:)({BallPattern});:))";
        private string _frameIndexDelimiter => ":";
        private string _ballPropertyDelimiter => ",";
        private string _playerDelimiter => ";";
        private string _playerPropertyDelimiter => ",";

        public virtual float CoordinateRatio => 100;
        public virtual bool SwitchYZCoordinates => true;

        public virtual TReplay ReadFromFile(string filePath)
        {
            TReplay replay = new TReplay();
            using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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

        public virtual IEnumerator ReadFramesAsync(string filePath, Action<Frame> onFrameLoaded)
        {
            using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                string frame;
                while ((frame = sr.ReadLine()) != null)
                {
                    Frame f = GetFrame(frame);
                    onFrameLoaded.Invoke(f);
                    yield return null;
                }
            }
            yield return null;
        }

        protected virtual Frame GetFrame(string frame)
        {
            var f = new Frame();
            //Get frame count
            int frameIndex = GetFrameIndex(frame);
            f.FrameIndex = frameIndex;
            //Get objects
            Dictionary<int, Object> objects = GetObjects(frame);
            objects.ForEach(o => f.Objects.Add(o.Key, o.Value));
            return f;
        }

        protected virtual Dictionary<int, Object> GetObjects(string frame)
        {
            Dictionary<int, Object> objects = new Dictionary<int, Object>();

            //Get balls
            TBall[] balls = GetBalls(frame);
            balls.ForEach(b => objects.Add(b.Id, b));

            //Get players
            TPlayer[] players = GetPlayers(frame);
            players.ForEach(p => objects.Add(p.Id, p));

            if (CoordinateRatio != 1)
            {
                foreach (KeyValuePair<int, Object> o in objects)
                    o.Value.Position /= CoordinateRatio;
            }

            if (SwitchYZCoordinates)
            {
                foreach (KeyValuePair<int, Object> o in objects)
                    o.Value.Position = new Vector3(o.Value.Position.x, o.Value.Position.z, o.Value.Position.y);
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
            propertiesStr = player.Split(_playerPropertyDelimiter.ToCharArray());
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
            string playersStr = Regex.Match(frame, PlayersPattern, RegexOptions.Singleline).Value;
            return playersStr.Split(_playerDelimiter.ToCharArray());
        }

        private TBall ConstructBall(string ball)
        {
            string[] ballPropertiesStr = ball.Split(_ballPropertyDelimiter.ToCharArray());
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
            var frameCount = Regex.Match(frame, FrameIndexPattern, RegexOptions.Singleline).Value;
            frameCount = frameCount.Trim(_frameIndexDelimiter.ToCharArray());
            return frameCount;
        }

        private string[] ReadBalls(string frame)
        {
            return Regex.Matches(frame, BallPattern, RegexOptions.Singleline).Cast<Match>()
                .Select(m => m.Value)
                .ToArray();
        }
    }
}