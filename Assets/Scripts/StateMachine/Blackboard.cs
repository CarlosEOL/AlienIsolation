using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class Blackboard
    {
        // A dictionary for more dynamic/flexible data needs
        private Dictionary<string, object> data = new();

        public void SetData(string key, object value) => data[key] = value;
        public T GetData<T>(string key) => data.ContainsKey(key) ? (T)data[key] : default;
    }
}