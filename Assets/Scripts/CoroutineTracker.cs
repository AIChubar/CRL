using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineTracker
{
    private MonoBehaviour coroutineRunner;
    private HashSet<Coroutine> activeCoroutines = new HashSet<Coroutine>();

    private System.Action onAllCoroutinesFinished;

    public CoroutineTracker(MonoBehaviour runner, System.Action onFinished)
    {
        this.coroutineRunner = runner;
        this.onAllCoroutinesFinished = onFinished;
    }

    public Coroutine StartTrackedCoroutine(IEnumerator coroutine)
    {
        Coroutine startedCoroutine = coroutineRunner.StartCoroutine(TrackCoroutine(coroutine));
        activeCoroutines.Add(startedCoroutine);
        return startedCoroutine;
    }   


    private IEnumerator TrackCoroutine(IEnumerator coroutine)
    {
        Coroutine runningCoroutine = coroutineRunner.StartCoroutine(coroutine);
        yield return runningCoroutine;  // Wait for the coroutine to finish
        activeCoroutines.Remove(runningCoroutine);
        if (activeCoroutines.Count == 0)
        {
            onAllCoroutinesFinished?.Invoke();
        }
    }


    public bool IsRunning => activeCoroutines.Count  > 0;
}
