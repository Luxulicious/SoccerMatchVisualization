using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public abstract class FindNearestObjectComponent<TComponent, TEvent> : MonoBehaviour where TComponent : ObjectComponent where TEvent : UnityEvent<TComponent, TComponent>, new()
{
    [SerializeField, Required] protected Transform _transformOrigin;
    [SerializeField] protected float _radius = 1;
    [SerializeField] protected LayerMask _layerMask;
    [SerializeField, ReadOnly] protected TComponent _nearbiest = null;

    [Tooltip("First param is the new value; Second param is the previous value")]
    [SerializeField] protected TEvent _onNearbiestColliderChangedWithHistory = new TEvent();

    public virtual TComponent Nearbiest
    {
        get
        {
            return _nearbiest;
        }

        protected set
        {
            if (value != _nearbiest)
                _onNearbiestColliderChangedWithHistory.Invoke(value, _nearbiest);
            _nearbiest = value;
        }
    }

    private void FixedUpdate()
    {
        CastAll();
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 originPosition = _transformOrigin.position;
        if (Nearbiest != null)
        {
            var nearbiestPosition = Nearbiest.transform.position;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(originPosition, nearbiestPosition);
        }
        Gizmos.color = new Color(Color.green.r, Color.green.g, Color.green.b, 0.3f);
        Gizmos.DrawSphere(originPosition, _radius);
    }

    public virtual void CastAll()
    {
        RaycastHit[] hits = Physics.SphereCastAll(_transformOrigin.position, Mathf.Abs(_radius), _transformOrigin.position, _radius, _layerMask, QueryTriggerInteraction.Collide);
        RaycastHit? nearbiestHit =
        hits.Where(x => x.collider.GetComponent<TComponent>() != null)?
            .OrderBy(x => x.distance)?
            .FirstOrDefault();
        if (!hits.Any() || nearbiestHit == null || nearbiestHit.Value.collider == null)
        {
            Nearbiest = null;
            return;
        }
        Nearbiest = nearbiestHit.Value.collider.GetComponent<TComponent>();
    }
}