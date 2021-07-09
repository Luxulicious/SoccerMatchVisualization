using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheLuxGames.Visualizer.Domain
{
    [Serializable]
    public class Replay
    {
        [Range(0, 360)]
        [SerializeField] private int _fps;

        //TODO Change list to something else since it's very likely ineffecient
        [SerializeField] private Dictionary<int, Frame> _frames;

        public Dictionary<int, Frame> Frames
        {
            get
            {
                if (_frames == null) _frames = new Dictionary<int, Frame>();
                return _frames;
            }

            set
            {
                if (_frames == null) _frames = new Dictionary<int, Frame>();
                _frames = value;
            }
        }

        public int Fps { get => _fps; set => _fps = value; }

        public int? FirstFrameIndex
        {
            get
            {
                if (Frames.Any())
                    //return Frames.OrderBy(x => x.FrameIndex).FirstOrDefault().FrameIndex;
                    return Frames.OrderBy(x => x.Key).First().Key;
                else return null;
            }
        }
    }

    [Serializable]
    public class Frame 
    {
        [SerializeField] private int _frameIndex;
        //TODO This should obviously not be a simple list in the long term due to performance reasons
        [SerializeField] private List<Object> _objects;

        public List<Object> Objects
        {
            get 
            {
                if (_objects == null) _objects = new List<Object>();
                return _objects; 
            }
            set
            {
                if (_objects == null) _objects = new List<Object>();
                _objects = value;
            }
        }
        public int FrameIndex { get => _frameIndex; set => _frameIndex = value; }

        [ShowInInspector]
        public List<Ball> Balls => Objects.OfType<Ball>().ToList();

        [ShowInInspector]
        public List<TeamPlayer> TeamPlayers => Objects.OfType<TeamPlayer>().ToList();
    }

    [Serializable]
    public abstract class Object
    {
        [SerializeField] private int _id = -1;
        //TODO Maybe not depend on Unity vector3
        [SerializeField] private Vector3 _position;

        public Vector3 Position { get => _position; set => _position = value; }
        public int Id { get => _id; set => _id = value; }
    }

    [Serializable]
    public abstract class MovableObject : Object
    {
        [SerializeField] private float _velocity;
        public float Velocity { get => _velocity; set => _velocity = value; }
    }

    [Serializable]
    public class Player : MovableObject
    {
    }

    [Serializable]
    public class Ball : MovableObject
    {
    }

    public class TeamPlayer : Player
    {
        [SerializeField] private int _teamId;
        [SerializeField] private int _playerNumber;

        public int TeamId { get => _teamId; set => _teamId = value; }
        public int PlayerNumber { get => _playerNumber; set => _playerNumber = value; }
    }
}