using System;
using UnityEngine;

public class RNGManager
{

    private System.Random rng;

    public int Seed { get; private set; }

    public void SetUp(int? seed = null)
    {
        Seed = seed ?? UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        rng = new System.Random(Seed);
    }

    public int NextInt(int min, int max)
    {
        return rng.Next(min, max);
    }

    public float NextFloat()
    {
        return (float)rng.NextDouble();
    }
}