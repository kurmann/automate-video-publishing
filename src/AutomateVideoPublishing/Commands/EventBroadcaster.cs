public class EventBroadcaster
{
    private Dictionary<Type, object> eventHandlers = new();

    public void Publish<T>(T eventInstance)
    {
        if (eventHandlers.TryGetValue(typeof(T), out var handler))
        {
            ((Action<T>)handler)(eventInstance);
        }
    }

    public void Subscribe<T>(Action<T> eventHandler)
    {
        eventHandlers[typeof(T)] = eventHandler;
    }
}
