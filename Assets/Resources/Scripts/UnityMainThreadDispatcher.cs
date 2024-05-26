using System;
using System.Collections.Generic;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> executionQueue = new Queue<Action>();

    public void Update()
    {
        while (executionQueue.Count > 0)
        {
            executionQueue.Dequeue().Invoke();
        }
    }

    public static void ExecuteOnMainThread(Action action)
    {
        lock (executionQueue)
        {
            executionQueue.Enqueue(action);
        }
    }
}
