using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EventBusType
{
    Start,
    GameOver,
    GoMain
}
public static class EventBus
{
    private static readonly IDictionary<EventBusType, UnityEvent> Events = new Dictionary<EventBusType, UnityEvent>();
    public static void Subscribe(EventBusType eventType, UnityAction listener)
    {
        UnityEvent thisEvent;
        if (Events.TryGetValue(eventType, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent(); thisEvent.AddListener(listener); Events.Add(eventType, thisEvent);
        }
    }
    public static void Unsubscribe(EventBusType type, UnityAction listener)
    {
        UnityEvent thisEvent;
        if (Events.TryGetValue(type, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }
    public static void Publish(EventBusType type)
    {
        UnityEvent thisEvent;
        if (Events.TryGetValue(type, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }
}