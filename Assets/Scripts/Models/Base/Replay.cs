using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheLuxGames.Visualizer.Models
{
    [Serializable]
    public class Replay
    {
        [Range(1, 360)]
        [SerializeField] private int _frameRate = 25;

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

        [ShowInInspector, UsedImplicitly] private List<Frame> FramesAsList => _frames?.Values.ToList();

        public int FrameRate { get => _frameRate; set => _frameRate = value; }

        public int? FirstFrameIndex => Frames.Any() ? Frames.OrderBy(x => x.Key).First().Key : (int?)null;
    }
}