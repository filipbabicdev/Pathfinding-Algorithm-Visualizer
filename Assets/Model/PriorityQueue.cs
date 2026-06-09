using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<TElement>
{
    // Min-heap u nizu: heap[0] je uvek najmanji prioritet.
    List<Node> heap = new List<Node>();
    // element -> indeks u heap-u, za O(1) Contains i O(log n) SetPriority
    Dictionary<TElement, int> indexOf = new Dictionary<TElement, int>();

    public int Count()
    {
        return heap.Count;
    }

    public void Enqueue(TElement e, int p = 0)
    {
        // Ako element već postoji, tretiraj kao promenu prioriteta (čuva konzistentnost mape).
        if (indexOf.ContainsKey(e))
        {
            SetPriority(e, p);
            return;
        }
        heap.Add(new Node(e, p));
        int i = heap.Count - 1;
        indexOf[e] = i;
        SiftUp(i);
    }

    public TElement Dequeue()
    {
        TElement min = heap[0].element;
        int last = heap.Count - 1;
        Swap(0, last);
        heap.RemoveAt(last);
        indexOf.Remove(min);
        if (heap.Count > 0)
            SiftDown(0);
        return min;
    }

    public bool Contains(TElement e)
    {
        return indexOf.ContainsKey(e);
    }

    public void SetPriority(TElement e, int p)
    {
        if (!indexOf.TryGetValue(e, out int i))
            return;
        int old = heap[i].priority;
        heap[i].priority = p;
        if (p < old) SiftUp(i);        // prioritet smanjen (decrease-key, kao kod relaksacije)
        else if (p > old) SiftDown(i);
    }

    void SiftUp(int i)
    {
        while (i > 0)
        {
            int parent = (i - 1) / 2;
            if (heap[i].priority < heap[parent].priority)
            {
                Swap(i, parent);
                i = parent;
            }
            else break;
        }
    }

    void SiftDown(int i)
    {
        int n = heap.Count;
        while (true)
        {
            int left = 2 * i + 1;
            int right = 2 * i + 2;
            int smallest = i;
            if (left < n && heap[left].priority < heap[smallest].priority) smallest = left;
            if (right < n && heap[right].priority < heap[smallest].priority) smallest = right;
            if (smallest == i) break;
            Swap(i, smallest);
            i = smallest;
        }
    }

    void Swap(int a, int b)
    {
        Node tmp = heap[a];
        heap[a] = heap[b];
        heap[b] = tmp;
        indexOf[heap[a].element] = a;
        indexOf[heap[b].element] = b;
    }

    class Node
    {
        public TElement element;
        public int priority;
        public Node(TElement e, int p) { element = e; priority = p; }
    }
}
