using System;
using UnityEngine;

namespace TheLuxGames.Visualizer.Models
{
    [Serializable]
    public abstract class MovableObject : Object
    {
        [SerializeField] private float _velocity;
        public float Velocity { get => _velocity; set => _velocity = value; }
    }
}