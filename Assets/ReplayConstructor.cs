using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using TheLuxGames.Visualizer.Domain;
using UnityEngine;
using Object = TheLuxGames.Visualizer.Domain.Object;

public class ReplayConstructor : SerializedMonoBehaviour
{
    private const string COLLECTIONSUFFIX = "Collection";
    [OdinSerialize] private Dictionary<Type, UnityEngine.Object> _typeToPrefabMap;
    [SerializeField] private Transform _parentTransform;
    [SerializeField] private bool _createCollectionParentObject = true;

    public void Construct(Replay replay)
    {
        foreach (var o in replay.Frames[replay.FirstFrameIndex.Value].Objects)
            ConstructObject(o);
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
        return component;
    }
}
