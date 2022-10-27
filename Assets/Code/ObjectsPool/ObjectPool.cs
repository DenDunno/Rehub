﻿using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : IUpdate where T : MonoBehaviour, IPoolableObject
{
    private readonly T _prefab;
    private readonly Stack<T> _pool = new Stack<T>();
    private readonly List<T> _activeObjects = new List<T>();

    protected ObjectPool(T prefab)
    {
        _prefab = prefab;
    }

    public T Create()
    {
        T poolableObject = SpawnOrPop();
        poolableObject.gameObject.SetActive(true);
        _activeObjects.Add(poolableObject);

        return poolableObject;
    }

    private T SpawnOrPop()
    {
        if (_pool.IsEmpty())
        {
            return Object.Instantiate(_prefab);
        }
        
        T poolableObject = _pool.Pop();
        return poolableObject;
    }

    public void Update()
    {
        for (int i = 0 ; i < _activeObjects.Count; ++i)
        {
            if (_activeObjects[i].IsActive == false)
            {
                _pool.Push(_activeObjects[i]);
                _activeObjects[i].gameObject.SetActive(false);
                _activeObjects.Remove(_activeObjects[i]);
            }
        }
    }
}