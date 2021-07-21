using System;

namespace TheLuxGames.Visualizer.Domain
{
    [Serializable]
    public class Ball : MovableObject, IEquatable<Object>
    {
        public  bool Equals(Object other)
        {
            return other is Player && this.Id == other.Id;
        }
    }
}