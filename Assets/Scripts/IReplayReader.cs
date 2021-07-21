using System;
using System.Collections;
using TheLuxGames.Visualizer.Domain;

namespace Assets.Scripts
{
    public interface IFileReader
    {
    }

    public interface IReplayReader<TReplay> : IFileReader where TReplay : Replay
    {
        TReplay ReadFromFile(string path);

        IEnumerator ReadFramesAsync(string filePath, Action<Frame> onFrameAdded);
    }
}