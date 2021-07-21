using System;
using UnityEngine;

namespace TheLuxGames.Visualizer.Domain
{
    [Serializable]
    public abstract class Object
    {
        [SerializeField] private int _id = -1;

        //TODO Maybe not depend on Unity vector3
        [SerializeField] private Vector3 _position;

        public Vector3 Position { get => _position; set => _position = value; }
        public int Id { get => _id; set => _id = value; }
    }
}