using System;
[AttributeUsage(AttributeTargets.Method)]
public class EventHandlerAttribute : Attribute {
    public string EventName { get; }
    public EventHandlerAttribute(string name) => EventName = name;
}