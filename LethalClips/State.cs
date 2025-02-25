using System.Collections.Generic;

namespace LethalClips;


internal static class State<T> where T : new()
{
    private static readonly Dictionary<object, T> states = [];

    internal static T Of(object obj) {
        if (!states.TryGetValue(obj, out T state)) {
            state = states[obj] = new();
        }
        
        return state;
    }
}
