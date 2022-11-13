﻿using System;
using System.Collections.Generic;

namespace Game.Utils
{
    public static class Fns
    {
        public static Predicate<T> Compliment<T>(Predicate<T> pred)
        {
            return t => !pred(t);
        }

        public static Func<T, V> Comp<T, U, V>(Func<U, V> fn1, Func<T, U> fn2)
        {
            return t => fn1(fn2(t));
        }

        public static Action<T> Comp<T, U>(Action<U> action, Func<T, U> fn)
        {
            return t => action(fn(t));
        }
    }

    public class MultiMethod<T, U, R>
    {
        readonly IDictionary<U, Func<T, R>> dict;
        readonly Func<T, U> dispatchFn;
        readonly R defaultVal;

        public static MultiMethod<T, U, R> Over(Func<T, U> dispatchFn)
        {
            return Over(dispatchFn, default);
        }

        public static MultiMethod<T, U, R> Over(Func<T, U> dispatchFn, R defaultVal)
        {
            IDictionary<U, Func<T, R>> dict = new Dictionary<U, Func<T, R>>();
            return new MultiMethod<T, U, R>(dict, dispatchFn, defaultVal);
        }

        private MultiMethod(IDictionary<U, Func<T, R>> dict, Func<T, U> dispatchFn, R defaultVal)
        {
            this.dict = dict;
            this.dispatchFn = dispatchFn;
            this.defaultVal = defaultVal;
        }

        public MultiMethod<T, U, R> AddMethod(U dispatchVal, Func<T, R> fn)
        {
            dict[dispatchVal] = fn;
            return this;
        }

        public MultiMethod<T, U, R> RemoveMethod(U dispatchVal)
        {
            dict.Remove(dispatchVal);
            return this;
        }

        public Func<T, R> Fn()
        {
            return input =>
            {
                U dispatchVal = dispatchFn(input);
                return dict.ContainsKey(dispatchVal)
                    ? dict[dispatchVal](input)
                    : defaultVal;
            };
        }
    }

    public class MultiAction<T, U>
    {
        readonly IDictionary<U, Action<T>> dict;
        readonly Func<T, U> dispatchFn;

        public static MultiAction<T, U> Over(Func<T, U> dispatchFn)
        {
            IDictionary<U, Action<T>> dict = new Dictionary<U, Action<T>>();
            return new MultiAction<T, U>(dict, dispatchFn);
        }

        private MultiAction(IDictionary<U, Action<T>> dict, Func<T, U> dispatchFn)
        {
            this.dict = dict;
            this.dispatchFn = dispatchFn;
        }

        public MultiAction<T, U> AddMethod(U dispatchVal, Action<T> fn)
        {
            dict[dispatchVal] = fn;
            return this;
        }

        public MultiAction<T, U> RemoveMethod(U dispatchVal)
        {
            dict.Remove(dispatchVal);
            return this;
        }

        public Action<T> Action()
        {
            return input =>
            {
                U dispatchVal = dispatchFn(input);
                if (dict.ContainsKey(dispatchVal))
                {
                    dict[dispatchVal](input);
                }
            };
        }
    }
}
