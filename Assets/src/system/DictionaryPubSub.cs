﻿using System;
using System.Collections.Generic;
using Game.Utils;
using Game.System.Events;
using System.Collections.Concurrent;

namespace Game.System {
    public class DictionaryPubSub : IComponent, IPubSub {
        IDictionary<Type, IDictionary<long, Action<IEvent>>> actions;
        IDictionary<long, Type> subscribers;
        readonly ConcurrentQueue<IEvent> q;

        long subId = 0;

        public void Tick(GameSession _) {
            while (q.TryDequeue(out IEvent e)) PublishEvent(e);
        }

        public DictionaryPubSub() {
            actions = new Dictionary<Type, IDictionary<long, Action<IEvent>>>();
            subscribers = new Dictionary<long, Type>();
            q = new ConcurrentQueue<IEvent>();
        }

        public IPubSub Publish<T>(T e) where T : IEvent {
            q.Enqueue(e);
            return this;
        }

        public IPubSub PublishSync<T>(T e) where T : IEvent {
            PublishEvent(e);
            return this;
        }

        public long Subscribe<T>(Action<T> action) where T : IEvent {
            long id = ++subId;
            Type t = typeof(T);
            IDictionary<long, Action<IEvent>> dict = actions.ContainsKey(t)
                ? actions[t]
                : new Dictionary<long, Action<IEvent>>();
            subscribers[id] = t;
            dict[id] = e => action((T)e);
            actions[t] = dict;

            return id;
        }

        public IPubSub Unsubscribe(long subscription) {
            Type t = subscribers.ContainsKey(subscription) ? subscribers[subscription] : null;
            if (t != null) {
                IDictionary<long, Action<IEvent>> dict = actions[t];
                dict.Remove(subscription);
                subscribers.Remove(subscription);
            }
            return this;
        }

        void PublishEvent(IEvent e) {
            Sequences.ForEach(actions[e.GetType()], entry => entry.Value(e));
        }
    }
}
