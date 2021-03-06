using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheLuxGames.Visualizer.Models
{
    [Serializable]
    public class Frame
    {
        [SerializeField] private int _frameIndex;

        [SerializeField] private Dictionary<int, Object> _objects;

        public Dictionary<int, Object> Objects
        {
            get
            {
                if (_objects == null) _objects = new Dictionary<int, Object>();
                return _objects;
            }
            set
            {
                if (_objects == null) _objects = new Dictionary<int, Object>();
                _objects = value;
            }
        }

        public int FrameIndex { get => _frameIndex; set => _frameIndex = value; }

        [ShowInInspector]
        public List<Ball> Balls => Objects.OfType<Ball>().ToList();

        [ShowInInspector]
        public List<TeamPlayer> TeamPlayers => Objects.OfType<TeamPlayer>().ToList();
    }
}