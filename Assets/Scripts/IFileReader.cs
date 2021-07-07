using System;

namespace Assets.Scripts
{
    public interface IReader
    { 
        
    }

    public interface IFileReader<TOutput> : IReader
    {
        TOutput ReadFromFile(string path);
    }

}
