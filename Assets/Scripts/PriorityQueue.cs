using System;
using System.Collections.Generic;

public class PriorityQueue<T>
{
    private List<(T item, float priority)> elements = new List<(T, float)>();

    public int Count => elements.Count;

    public void Enqueue(T item, float priority)
    {
        elements.Add((item, priority));
        elements.Sort((a, b) => a.priority.CompareTo(b.priority)); // Sort by priority
    }

    public T Dequeue()
    {
        if (elements.Count == 0) throw new InvalidOperationException("Queue is empty");
        var bestItem = elements[0].item;
        elements.RemoveAt(0);
        return bestItem;
    }

    public bool Contains(T item)
    {
        return elements.Exists(e => EqualityComparer<T>.Default.Equals(e.item, item));
    }
}

