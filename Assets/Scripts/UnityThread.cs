#nullable enable
#pragma warning disable CS8618
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UnityThread : MonoBehaviour
{
    private ConcurrentQueue<Action> _actions = new();
    
    private void Update()
    {
        while(_actions.TryDequeue(out Action action))
        {
            action.Invoke();
        }
    }

    public void Enqueue(Action action)
    {
        _actions.Enqueue(action);
    }
}
#pragma warning restore CS8618
#nullable disable
