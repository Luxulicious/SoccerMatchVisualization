using System;
using System.Threading.Tasks;
using TheLuxGames.Visualizer.Domain;

namespace Assets.Scripts
{
    public interface IFileReader
    { 
        
    }

    public interface IReplayReader<TReplay> : IFileReader where TReplay : Replay
    {
        TReplay ReadFromFile(string path);
        Task ReadFramesAsync(string filePath, Action<Frame> onFrameAdded);
    }

}
