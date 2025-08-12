using System;
using System.Collections.Generic;
using UnityEngine;

public class RNGManager
{
    private System.Random rng;
    private Queue<double> buffer;

    public int Seed { get; private set; }
    public int BufferSize { get; private set; } = 100;

    public void SetUp(int? seed = null, int bufferSize = 100)
    {
        Seed = seed ?? UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        rng = new System.Random(Seed);

        BufferSize = bufferSize;
        buffer = new Queue<double>(BufferSize);

        FillBuffer();
    }

    private void FillBuffer()
    {
        while (buffer.Count < BufferSize)
        {
            buffer.Enqueue(rng.NextDouble());
        }
    }

    private double Consume()
    {
        double val = buffer.Dequeue();
        buffer.Enqueue(rng.NextDouble()); // refill
        return val;
    }

    public int NextInt(int min, int max)
    {
        return min + (int)(Consume() * (max - min));
    }

    public float NextFloat()
    {
        return (float)Consume();
    }

    public float NextFloat(float min, float max)
    {
        return min + (float)Consume() * (max - min);
    }

    // Peek ahead without consuming
    public int PeekNextInt(int min, int max, int offset = 0)
    {
        double val = Peek(offset);
        return min + (int)(val * (max - min));
    }

    public float PeekNextFloat(int offset = 0)
    {
        return (float)Peek(offset);
    }

    public float PeekNextFloat(float min, float max, int offset = 0)
    {
        double val = Peek(offset);
        return min + (float)val * (max - min);
    }

    private double Peek(int offset)
    {
        if (offset < 0 || offset >= buffer.Count)
            throw new ArgumentOutOfRangeException(nameof(offset), "Offset out of buffer range.");
        return GetElementAt(buffer, offset);
    }

    private T GetElementAt<T>(IEnumerable<T> collection, int index)
    {
        using (var enumerator = collection.GetEnumerator())
        {
            for (int i = 0; i <= index; i++)
                enumerator.MoveNext();
            return enumerator.Current;
        }
    }
}
