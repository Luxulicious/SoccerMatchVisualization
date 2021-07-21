using Sirenix.OdinInspector;
using System;
using UnityEngine;
using Object = TheLuxGames.Visualizer.Models.Object;

namespace TheLuxGames.Visualizer.Behaviours
{
    public abstract class ObjectComponent : MonoBehaviour
    {
        public abstract Object Object { get; }

        public abstract void AdvanceFrame(Object value);
    }

    public abstract class ObjectComponent<TObject> : ObjectComponent where TObject : Object
    {
        [SerializeField, InlineProperty, HideLabel, ReadOnly] private TObject _value;

        public virtual TObject Value { get => _value; set => _value = value; }
        public override Object Object { get => _value; }

        [SerializeField] private Transform _transform;

        protected virtual void Awake()
        {
            if (!_transform)
                _transform = transform;
        }

        public virtual void AdvanceFrame(TObject value)
        {
            if (Value.Id != value.Id) throw new Exception($"Invalid id! Expected {Value.Id} but got {value.Id}");
            Value = value;
            _transform.position = value.Position;
        }

        public override void AdvanceFrame(Object value)
        {
            AdvanceFrame((TObject)value);
        }
    }
}