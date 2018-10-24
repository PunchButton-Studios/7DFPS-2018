using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

    protected static bool WithinView(Transform viewer, Transform target, float viewAngle, float viewRange, LayerMask layerMask)
        => WithinView(viewer, target.position, viewAngle, viewRange, layerMask);

    protected static bool WithinView(Transform viewer, Vector3 target, float viewAngle, float viewRange, LayerMask layerMask)
    {
        if (Vector3.Distance(target, viewer.position) <= viewRange)
        {
            Vector3 dir = target - viewer.position;
            float angle = Vector3.Angle(dir, viewer.forward);
            if (angle <= viewAngle)
                return !Physics.Raycast(viewer.position, dir.normalized, dir.magnitude, layerMask);
            else
                return false;
        }
        else
            return false;
    }
}