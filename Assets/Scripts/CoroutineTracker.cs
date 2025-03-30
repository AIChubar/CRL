using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineTracker
{
    private MonoBehaviour coroutineRunner;
    private HashSet<IEnumerator> activeCoroutines = new HashSet<IEnumerator>();

    private System.Action onAllCoroutinesFinished;

    public CoroutineTracker(MonoBehaviour runner, System.Action onFinished)
    {
        this.coroutineRunner = runner;
        this.onAllCoroutinesFinished = onFinished;
    }

    public Coroutine StartTrackedCoroutine(IEnumerator coroutine)
    {
        Coroutine startedCoroutine = coroutineRunner.StartCoroutine(TrackCoroutine(coroutine));
        activeCoroutines.Add(coroutine);
        return startedCoroutine;
    }   

    private IEnumerator TrackCoroutine(IEnumerator coroutine)
    {
        yield return coroutineRunner.StartCoroutine(coroutine); 
        activeCoroutines.Remove(coroutine);
        
        if (activeCoroutines.Count == 0)
        {
            onAllCoroutinesFinished?.Invoke();
        }
    }

    public bool IsRunning => activeCoroutines.Count > 0;
}