using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ThreadManager : MonoBehaviour
{
    private static Queue<Action> functionToExecuteOnMainThread;
    private static object _object = new object();
    void Start()
    {
        functionToExecuteOnMainThread = new Queue<Action>();
    }

    void Update()
    {
        while (functionToExecuteOnMainThread.Count > 0)
        {
            Action function = functionToExecuteOnMainThread.Dequeue();
            function?.Invoke();
        }


    }

    public static Thread startThreadFunction(Action function)
    {
        Thread newThread = new Thread(new ThreadStart(function));
        newThread.Start();
        return newThread;
    }

    public static void queueToMainThread(Action function)
    {
        lock (_object)
            functionToExecuteOnMainThread.Enqueue(function);


    }
}