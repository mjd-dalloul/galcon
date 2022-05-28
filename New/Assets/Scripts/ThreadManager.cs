using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ThreadManager : MonoBehaviour {
    private static Queue<Action> functionToExecuteOnMainThread;

    void Start() {
        functionToExecuteOnMainThread = new Queue<Action>();

    }

    void Update() {
        if (functionToExecuteOnMainThread.Count > 0) {
            Action function = functionToExecuteOnMainThread.Dequeue();
            function();
        }
    }

    public static void startThreadFunction(Action function) {
        new Thread(new ThreadStart(function)).Start();
    }

    public static void queueToMainThread(Action function) {
        functionToExecuteOnMainThread.Enqueue(function);
    }
}