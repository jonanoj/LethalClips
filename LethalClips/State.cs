using System.Collections.Generic;
using UnityEngine;

namespace LethalClips;


public abstract class State<K, V> where V : State<K, V>, new() {
    private static readonly Dictionary<K, V> states = [];

    public K Instance { get; private set; }

    public static V Of(K obj) {
        if(!states.TryGetValue(obj, out V state)) {
            state = states[obj] = new();
            state.Instance = obj;
        }

        return state;
    }

    public static IEnumerable<(K, V)> All() {
        foreach(var pair in states) {
            yield return (pair.Key, pair.Value);
        }
    }
}


public static class StateExtensions {
    public static V GetState<K, V>(this GameObject obj) where K : Component where V : State<K, V>, new() {
        return State<K, V>.Of(obj.GetComponent<K>());
    }

    public static bool TryGetState<K, V>(this GameObject obj, out V state) where K : Component where V : State<K, V>, new() {
        if(obj.TryGetComponent(out K component)) {
            state = State<K, V>.Of(component);
            return true;
        } else {
            state = null;
            return false;
        }
    }

    public static V GetState<K, V>(this Component obj) where K : Component where V : State<K, V>, new() {
        return GetState<K, V>(obj.gameObject);
    }

    public static bool TryGetState<K, V>(this Component obj, out V state) where K : Component where V : State<K, V>, new() {
        return TryGetState<K, V>(obj.gameObject, out state);
    }
}
