using System.Collections.Generic;
using UnityEngine;

public static class Events
{
    public static NotificationEvent NotificationEvent = new();
}

public class NotificationEvent : GameEvent
{
    public string text;
}
