using AutoBattler.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AutoBattler.Event
{
    public class EventBusManager : GenericMonoSingleton<EventBusManager>
    {
        private readonly Dictionary<string, UnityEvent<object[]>> _eventDictionary = new Dictionary<string, UnityEvent<object[]>>();

        public void Subscribe(EventNameEnum eventName, UnityAction<object[]> listener)
        {
            string eventKey = eventName.ToString().ToUpper();

            if (!_eventDictionary.TryGetValue(eventKey, out var thisEvent))
            {
                thisEvent = new UnityEvent<object[]>();
                _eventDictionary.Add(eventKey, thisEvent);
            }

            thisEvent.AddListener(listener);
        }

        public void Unsubscribe(EventNameEnum eventName, UnityAction<object[]> listener)
        {
            string eventKey = eventName.ToString().ToUpper();

            if (_eventDictionary.TryGetValue(eventKey, out var thisEvent))
            {
                thisEvent.RemoveListener(listener);
            }
        }

        public void Raise(EventNameEnum eventName, params object[] parameters)
        {
            string eventKey = eventName.ToString().ToUpper();

            if (_eventDictionary.TryGetValue(eventKey, out var thisEvent))
            {
                thisEvent.Invoke(parameters ?? new object[0]);
            }
            else
            {
                Debug.LogWarning($"Event '{eventKey}' has no listeners.");
            }
        }

        public void RaiseNoParams(EventNameEnum eventName)
        {
            string eventKey = eventName.ToString().ToUpper();

            if (_eventDictionary.TryGetValue(eventKey, out var thisEvent))
            {
                thisEvent.Invoke(new object[0]);
            }
            else
            {
                Debug.LogWarning($"Event '{eventKey}' has no listeners.");
            }
        }

        public void ClearEvent(EventNameEnum eventName)
        {
            string eventKey = eventName.ToString().ToUpper();

            if (_eventDictionary.ContainsKey(eventKey))
            {
                _eventDictionary[eventKey].RemoveAllListeners();
            }
        }

        public bool HasListeners(EventNameEnum eventName)
        {
            string eventKey = eventName.ToString().ToUpper();
            return _eventDictionary.ContainsKey(eventKey) && _eventDictionary[eventKey].GetPersistentEventCount() > 0;
        }
    }

}