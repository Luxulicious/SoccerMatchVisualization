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

    public virtual UnityEngine.Object ConstructObject(Object o) 
    {
        foreach (var t in _typeToPrefabMap.Keys)
        {
            if (o.GetType() == t)
            {
                if (!_createCollectionParentObject)
                    return Instantiate(_typeToPrefabMap[t], o.Position, Quaternion.identity, _parentTransform);
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
                    return Instantiate(_typeToPrefabMap[t], o.Position, Quaternion.identity, parentTransform);
                }
            }
        }
        Debug.LogError($"Failed to instatiate object: {o}");
        return null;
    }
}
