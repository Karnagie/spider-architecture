#pragma warning disable CS0067
using System;
using CompositeDirectorWithGeneratingComposites.CompositeDirector;
using CompositeDirectorWithGeneratingComposites.CompositeDirector.CompositeGeneration;

namespace Infrastructure.Disposables
{
    public interface IClearable : IPoolItem
    {
        Result Clear();
    }

    public interface IClearableWithOptions : IClearable
    {
        DisposeTime DisposeTime { get; }
    }
    

    public class Empty : IClearable
    {
        public void Dispose()
        {
            // TODO release managed resources here
        }

        public event Action Disposed;
        public DisposeTime DisposeTime { get; }
        public Result Clear()
        {
            return Result.Ok;
        }
    }

    public enum DisposeTime
    {
        SceneEnd,
        GameEnd,
    }
}