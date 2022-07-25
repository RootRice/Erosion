using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    protected IDictionary<TKey, TValue> wrappedDictionary;

    public VirtualDictionary()
    {
        wrappedDictionary = new Dictionary<TKey, TValue>();
    }

    public VirtualDictionary(int capacity)
    {
        wrappedDictionary = new Dictionary<TKey, TValue>(capacity);
    }

    public VirtualDictionary(IEqualityComparer<TKey> comparer)
    {
        wrappedDictionary = new Dictionary<TKey, TValue>(comparer);
    }

    public VirtualDictionary(int capacity, IEqualityComparer<TKey> comparer)
    {
        wrappedDictionary = new Dictionary<TKey, TValue>(capacity, comparer);
    }

    public VirtualDictionary(IDictionary<TKey, TValue> dictionary)
    {
        wrappedDictionary = new Dictionary<TKey, TValue>(dictionary);
    }

    public VirtualDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
    {
        wrappedDictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
    }

    public virtual void Add(TKey key, TValue value)
    {
        wrappedDictionary.Add(key, value);
    }

    public virtual bool ContainsKey(TKey key)
    {
        return wrappedDictionary.ContainsKey(key);
    }

    public virtual ICollection<TKey> Keys
    {
        get
        {
            return wrappedDictionary.Keys;
        }
    }

    public virtual bool Remove(TKey key)
    {
        return wrappedDictionary.Remove(key);
    }

    public virtual bool TryGetValue(TKey key, out TValue value)
    {
        return wrappedDictionary.TryGetValue(key, out value);
    }

    public virtual ICollection<TValue> Values
    {
        get
        {
            return wrappedDictionary.Values;
        }
    }

    public virtual TValue this[TKey key]
    {
        get
        {
            return wrappedDictionary[key];
        }
        set
        {
            wrappedDictionary[key] = value;
        }
    }

    public virtual void Add(KeyValuePair<TKey, TValue> item)
    {
        wrappedDictionary.Add(item);
    }

    public virtual void Clear()
    {
        wrappedDictionary.Clear();
    }

    public virtual bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return wrappedDictionary.Contains(item);
    }

    public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        wrappedDictionary.CopyTo(array, arrayIndex);
    }

    public virtual int Count
    {
        get
        {
            return wrappedDictionary.Count;
        }
    }

    public virtual bool IsReadOnly
    {
        get { return wrappedDictionary.IsReadOnly; }
    }

    public virtual bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return wrappedDictionary.Remove(item);
    }

    public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return wrappedDictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class ForceTracker : VirtualDictionary<string, Vector3>
{
    public Vector3 TotalForce;

    public void Update()
    {
        TotalForce = Vector3.zero;
        foreach (Vector3 force in wrappedDictionary.Values)
        {
            TotalForce += force;
        }
    }

    public override void Add(KeyValuePair<string, Vector3> item)
    {
        base.Add(item);
        Update();
    }

    public override Vector3 this[string key]
    {
        get { return base[key]; }
        set { base[key] = value; Update(); }
    }
}