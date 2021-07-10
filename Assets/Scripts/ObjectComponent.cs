using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = TheLuxGames.Visualizer.Domain.Object;

public abstract class ObjectComponent<TObject> : MonoBehaviour where TObject : Object
{
    [SerializeField, InlineProperty, HideLabel, ReadOnly] private TObject _value;

    public virtual TObject Value { get => _value; set => this._value = value; }
}
