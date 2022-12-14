using UnityEngine;
using Game.System;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Game.Utils;
using System.Threading.Tasks;
using System.Threading;

namespace System.Runtime.CompilerServices { public class IsExternalInit { } }

public class GameSession : MonoBehaviour {
    IDictionary<string, ISet<GameObject>> taggedObjects;
    IDictionary<GameObject, ISet<string>> objectTags;
    GameSystem system;
    IDictionary<Type, Task> tasks;

    public void RegisterTags(IEnumerable<string> tags, GameObject obj) {
        tags.ForEach(tag => {
            Colls.Update(
                taggedObjects,
                tag,
                objs => Colls.Add(objs, obj),
                () => new HashSet<GameObject>());
            Colls.Update(
                objectTags,
                obj,
                tags => Colls.Add(tags, tag),
                () => new HashSet<string>());
        });
    }

    public void UnregisterTags(GameObject obj) {
        objectTags.Get(obj, new HashSet<string>())
            .ForEach(tag => taggedObjects
                .Update(tag, set => Colls
                    .Remove(set, obj), () => new HashSet<GameObject>()));
        objectTags.Remove(obj);
    }

    public GameObject GetTaggedObject(string tag) {
        ISet<GameObject> objects = GetTaggedObjects(tag);
        if (objects.Count > 1)
            throw new InvalidOperationException("More than one item found for tag: " + tag);
        return objects.First();
    }

    public ISet<string> GetObjectTags(GameObject obj) => Colls.Get(objectTags, obj);
    public ISet<GameObject> GetTaggedObjects(string tag) => Colls.Get(taggedObjects, tag, new HashSet<GameObject>());
    public GameObject GetPlayer() => GetTaggedObject("player");
    public T Get<T>() => system.Get<T>();

    public void Register<T>(T component) where T : IComponent {
        StartComponent(typeof(T), component);
        system.Register(component);
    }

    public void Unregister<T>() {
        system.Unregister(typeof(T));
        StopComponent(typeof(T));
    }

    void Awake() {
        if (FindObjectsOfType<GameSession>().Length > 1) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Init();
    }

    void Init() {
        system = GameSystem.Default(this);
        taggedObjects = new Dictionary<string, ISet<GameObject>>();
        objectTags = new Dictionary<GameObject, ISet<string>>();
        tasks = new Dictionary<Type, Task>();
        system.ForEach(tpl => StartComponent(tpl.Item1, tpl.Item2));
    }

    void Start() {
    }

    void StartComponent(Type type, IComponent component) {
        if (tasks.ContainsKey(type))
            throw new NotSupportedException("component has already been started");

        tasks[type] = new Task(() => {
            while (true) {
                component.Tick(this);
                Thread.Sleep(16);
            }
        });
        tasks[type].Start();
    }

    void StopComponent(Type type) {
        Task task = Colls.Get(tasks, type);
        try {
            task.Dispose();
        } catch (Exception) { }
    }

    void OnDestroy() => (system ?? Sequences.Empty<(Type, IComponent)>()
        ).ForEach(tpl => StopComponent(tpl.Item1));
}
