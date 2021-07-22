using System;
using UnityEngine;

namespace TheLuxGames.Visualizer.Models
{
    [Serializable]
    public abstract class Object
    {
        [SerializeField] private int _id = -1;

        [SerializeField] private Vector3 _position;

        public Vector3 Position { get => _position; set => _position = value; }
        public int Id { get => _id; set => _id = value; }
    }
}