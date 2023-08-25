using System.Collections.Generic;
using CompositeDirectorWithGeneratingComposites.CompositeDirector;
using CompositeDirectorWithGeneratingComposites.CompositeDirector.CompositeGeneration;
using UnityEngine;

namespace Infrastructure.Disposables
{
    public static class DisposableGrouper
    {
        public static Result ThatWithTime(this Result result, DisposeTime disposeTime)
        {
            var last = CompositeHelper.Last;
            if (last == null)
                return Result.Error;
            
            last.Apply(new CommandThatWithTime(disposeTime));
            
            return default;
        }
    }
    
    public class CommandThatWithTime : ICommand
    {
        private DisposeTime _time;

        public CommandThatWithTime(DisposeTime time)
        {
            _time = time;
        }
        
        public T[] Do<T>(T[] items)
        {
            List<T> stash = new List<T>();
            foreach (T item in items)
            {
                if (item is IClearableWithOptions clearableWithOptions)
                {
                    if (clearableWithOptions.DisposeTime == _time)
                    {
                        stash.Add(item);
                    }
                }
            }
            return stash.ToArray();
        }
    }
}