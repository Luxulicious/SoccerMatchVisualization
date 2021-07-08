using System;
using TheLuxGames.Visualizer.Domain;

namespace Assets.Scripts
{
    public interface IFileReader
    { 
        
    }

    public interface IReplayReader<TReplay> : IFileReader where TReplay : Replay
    {
        TReplay ReadFromFile(string path);
    }

}
