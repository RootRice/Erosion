using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventListener : MonoBehaviour
{
    protected List<EventAction> actions;
    public EventListener()
    {
        actions = new List<EventAction>();
    }
    public EventListener(List<EventAction> _actions)
    {
        actions = _actions;
    }

    public void AddEvent(EventAction e)
    {
        actions.Add(e);
    }

    public void RemoveEvent(EventAction e)
    {
        actions.Remove(e);
    }

    public void TriggerActions()
    {
        foreach (EventAction action in actions)
        {
            action.TriggerAction();
        }
    }
    public void ResetActions()
    {
        foreach (EventAction action in actions)
        {
            action.Reset();
        }
    }
}
