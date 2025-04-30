using System;
using System.Collections.Generic;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();

    private static UnityMainThreadDispatcher _instance;

    public static UnityMainThreadDispatcher Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("MainThreadDispatcher").AddComponent<UnityMainThreadDispatcher>();
            }
            return _instance;
        }
    }

    public void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke(); // Execute actions queued for the main thread
            }
        }
    }

    public void Enqueue(Action action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action); // Add actions to the queue
        }
    }
}