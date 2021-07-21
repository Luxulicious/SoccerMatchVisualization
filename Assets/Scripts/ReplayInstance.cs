using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using TheLuxGames.Visualizer.Domain;
using UnityEngine;
using Object = TheLuxGames.Visualizer.Domain.Object;

public class ReplayInstance : SerializedMonoBehaviour
{
    private const string COLLECTIONSUFFIX = "Collection";
    [OdinSerialize, FoldoutGroup("Construction")] private Dictionary<Type, UnityEngine.Object> _typeToPrefabMap = new Dictionary<Type, UnityEngine.Object>();
    [SerializeField, FoldoutGroup("Construction")] private Transform _parentTransform;
    [SerializeField, FoldoutGroup("Construction")] private bool _createCollectionParentObject = true;

    [ReadOnly, HideInEditorMode, ShowInInspector, FoldoutGroup("Instance")]
    private Dictionary<int, ObjectComponent> _objectIdToInstanceMap = new Dictionary<int, ObjectComponent>();

    public void Destruct()
    {
        if (_objectIdToInstanceMap.Any())
        {
            foreach (var instance in _objectIdToInstanceMap.Values)
            {
                GameObject.DestroyImmediate(instance.gameObject);
            }
            if (_parentTransform)
            {
                foreach (Transform transform in _parentTransform)
                {
                    GameObject.DestroyImmediate(transform.gameObject);
                }
            }
        }
        _objectIdToInstanceMap.Clear();
    }

    public void Construct(Replay replay)
    {
        Construct(replay, replay.FirstFrameIndex.Value);
    }

    public void Construct(Replay replay, int frame)
    {
        foreach (var o in replay.Frames[frame].Objects)
            ConstructObject(o);
    }

    private UnityEngine.GameObject ConstructObject(Object o)
    {
        foreach (var t in _typeToPrefabMap.Keys)
        {
            var objectType = o.GetType();
            if (objectType == t)
            {
                if (!_createCollectionParentObject)
                    return InstantiateObject(o, t, _parentTransform);
                else
                {
                    string parentName = t.Name + COLLECTIONSUFFIX;
                    GameObject parentGameObject = GameObject.Find(parentName);
                    if (parentGameObject == null)
                    {
                        parentGameObject = new GameObject(parentName);
                        parentGameObject.transform.SetParent(_parentTransform);
                    }
                    Transform parentTransform = parentGameObject.transform;
                    return InstantiateObject(o, t, parentTransform);
                }
            }
        }
        Debug.LogError($"Failed to instatiate object: {o}");
        return null;
    }

    protected virtual GameObject InstantiateObject(Object o, Type t, Transform parentTransform)
    {
        return (GameObject)Instantiate(_typeToPrefabMap[t], o.Position, Quaternion.identity, parentTransform);
    }

    protected TObjectComponent AddObjectComponent<TObject, TObjectComponent>(GameObject gameObject, Object value)
    where TObjectComponent : ObjectComponent<TObject>
    where TObject : Object
    {
        var component = gameObject.GetComponent<TObjectComponent>();
        if (component == null)
            component = gameObject.AddComponent<TObjectComponent>();
        component.Value = value as TObject;
        _objectIdToInstanceMap.Add(value.Id, component);
        return component;
    }

    public void AdvanceFrame(Frame next, Frame previous)
    {
        foreach (var o in next.Objects)
        {
            int key = o.Id;
            if (_objectIdToInstanceMap.ContainsKey(key))
                _objectIdToInstanceMap[key].AdvanceFrame(o);
            else
                Debug.LogError($"Could not find object with id: {o.Id} in map!" +
                    $"A new instance should be created in this case.");
        }
    }
}