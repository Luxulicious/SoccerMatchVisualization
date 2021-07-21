using System;

namespace TheLuxGames.Visualizer.Models
{
    [Serializable]
    public class Ball : MovableObject, IEquatable<Object>
    {
        public bool Equals(Object other) => other is Player && this.Id == other.Id;
    }
}