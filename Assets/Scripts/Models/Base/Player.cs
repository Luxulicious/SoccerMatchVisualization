using System;

namespace TheLuxGames.Visualizer.Models
{
    [Serializable]
    public class Player : MovableObject, IEquatable<Object>
    {
        public bool Equals(Object other)
        {
            return other is Player && this.Id == other.Id;
        }
    }
}