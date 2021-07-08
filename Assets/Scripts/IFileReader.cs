using System;

namespace Assets.Scripts
{
    public interface IFileReader
    { 
        
    }

    public interface IFileReader<TOutput> : IFileReader
    {
        TOutput ReadFromFile(string path);
    }

}
