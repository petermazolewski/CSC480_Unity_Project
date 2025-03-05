using System;
using System.Collections.Generic;

public class Queue<T>
{
    private List<T> elements = new List<T>();

    public int Count => elements.Count;

    public void Enqueue(T item)
    {
        elements.Add(item);
    }

    public T Dequeue()
    {
        if (elements.Count == 0) throw new InvalidOperationException("Queue is empty");
        var bestItem = elements[0];
        elements.RemoveAt(0);
        return bestItem;
    }

    public bool Contains(T item)
    {
        return elements.Contains(item);
    }
}

