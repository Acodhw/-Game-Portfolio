using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class WeightTrigger : MonoBehaviour
{
    [Tooltip("이벤트를 발생시키기 위한 최소 무게 합")]
    public float weightThreshold = 10f;

    [Tooltip("무게를 충족했을 때 실행할 이벤트")]
    public UnityEvent onWeightReached;

    private HashSet<Rigidbody> rigidbodiesOnPlate = new HashSet<Rigidbody>();
    private bool isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            rigidbodiesOnPlate.Add(rb);
            CheckWeight();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isTriggered) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && rigidbodiesOnPlate.Contains(rb))
        {
            rigidbodiesOnPlate.Remove(rb);
        }
    }

    private void CheckWeight()
    {
        float totalWeight = 0f;

        rigidbodiesOnPlate.RemoveWhere(rb => rb == null);

        foreach (Rigidbody rb in rigidbodiesOnPlate)
        {
            totalWeight += rb.mass;
        }
        if (totalWeight >= weightThreshold)
        {
            isTriggered = true;
            onWeightReached?.Invoke();
        }
    }
}