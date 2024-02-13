using System.Collections.Generic;
using UnityEngine;

public abstract class Command : ICommand
{
    public delegate void OnCompleteCallback();
    private List<OnCompleteCallback> callbacksList = new();
    public virtual void Execute()
    {
        foreach (OnCompleteCallback callback in callbacksList)
        {
            callback?.Invoke();
        }
        callbacksList.Clear();
    }
    internal void OnComplete(OnCompleteCallback callback)
    {
        callbacksList.Add(callback);
    }
    
}
