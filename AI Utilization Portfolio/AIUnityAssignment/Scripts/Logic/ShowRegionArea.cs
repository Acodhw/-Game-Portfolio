using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ShowRegionArea : MonoBehaviour
{
    [Tooltip("지역 이름.")]
    public string regionName = "새로운 구역";

    private BoxCollider col;

    private void Awake()
    {
        col = GetComponent<BoxCollider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null)
        {
            if (ShowRegion.Instance != null)
            {
                ShowRegion.Instance.ShowRegionName(regionName);
            }
        }
    }
}