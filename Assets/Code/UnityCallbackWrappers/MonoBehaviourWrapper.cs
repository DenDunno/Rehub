using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class MonoBehaviourWrapper : MonoBehaviour
{
    private IEnumerable<ISubscriber> _subscribers = new ISubscriber[]{};
    private IEnumerable<IFixedUpdate> _fixedUpdates = new IFixedUpdate[]{};    
    private IEnumerable<IUpdate> _updates = new IUpdate[]{};
    
    protected void SetDependencies(object[] dependencies)
    {
        foreach (object dependency in dependencies)
        {
            if (dependency == null)
            {
                throw new ArgumentNullException("In " + gameObject.name);
            }
        }
        
        dependencies.OfType<IInitializable>().ForEach(initializable => initializable.Init());
        _subscribers = dependencies.OfType<ISubscriber>();
        _fixedUpdates = dependencies.OfType<IFixedUpdate>();
        _updates = dependencies.OfType<IUpdate>();
    }

    private void OnEnable()
    {
        _subscribers.SubscribeForEach();
    }

    private void OnDisable()
    {
        _subscribers.UnsubscribeForEach();
    }
    
    private void FixedUpdate()
    {
        _fixedUpdates.FixedUpdateForEach();
    }
    
    private void Update()
    {
        _updates.UpdateForEach();
    }
}