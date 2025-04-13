using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineTracker
{
    private MonoBehaviour coroutineRunner;
    private HashSet<IEnumerator> activeCoroutines = new HashSet<IEnumerator>();
    private AnimationType animationType;

    public CoroutineTracker(MonoBehaviour runner, AnimationType animationType)
    {
        this.coroutineRunner = runner;
        this.animationType = animationType;
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
            GameManager.instance.eventManager.OnCoroutineEnd?.InvokeEvent(animationType);
        }
    }

    public bool IsRunning => activeCoroutines.Count > 0;
}

public enum AnimationType
{
    Spin, Result
}