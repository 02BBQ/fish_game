using System.Collections.Generic;
using UnityEngine;

public static class Events
{
    public static NotificationEvent NotificationEvent = new();
    public static CamShakeEvent CamShakeEvent = new();
    public static ItemAddedEvent ItemAddedEvent = new();
    public static ItemDeletedEvent ItemDeletedEvent = new();
}

public class NotificationEvent : GameEvent
{
    public string text;
}
public class CamShakeEvent : GameEvent
{
    public float amplitude;
    public float frequency;
    public float duration;
}

public class ItemAddedEvent : GameEvent
{
    public Item newItem;
}


public class ItemDeletedEvent : GameEvent
{
    public Item newItem;
}
