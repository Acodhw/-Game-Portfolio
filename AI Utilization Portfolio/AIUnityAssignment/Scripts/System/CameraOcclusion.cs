using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraOcclusion : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("카메라가 바라보는 대상 (플레이어)")]
    public Transform player;

    [Tooltip("가려질 수 있는 오브젝트들의 레이어 (벽, 땅 등)")]
    public LayerMask occlusionLayer;

    private Dictionary<Renderer, ShadowCastingMode> originalShadowModes = new Dictionary<Renderer, ShadowCastingMode>();
    private HashSet<Renderer> currentlyBlockingRenderers = new HashSet<Renderer>();
    private HashSet<Renderer> previouslyBlockingRenderers = new HashSet<Renderer>();

    void Update()
    {
        if (player == null) return;

        previouslyBlockingRenderers.Clear();
        foreach (var r in currentlyBlockingRenderers)
        {
            previouslyBlockingRenderers.Add(r);
        }
        currentlyBlockingRenderers.Clear();
        Vector3 startPos = transform.position;
        Vector3 endPos = player.position + Vector3.up * 1.0f;
        Vector3 direction = endPos - startPos;
        float distance = direction.magnitude;

        RaycastHit[] hits = Physics.RaycastAll(startPos, direction.normalized, distance, occlusionLayer, QueryTriggerInteraction.Ignore);

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform == player || hit.transform.IsChildOf(player)) continue;

            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer != null)
            {
                currentlyBlockingRenderers.Add(renderer);
                if (!originalShadowModes.ContainsKey(renderer))
                {
                    originalShadowModes[renderer] = renderer.shadowCastingMode;
                    renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                }
            }
        }
        foreach (var renderer in previouslyBlockingRenderers)
        {
            if (!currentlyBlockingRenderers.Contains(renderer))
            {
                RestoreRenderer(renderer);
            }
        }
    }
    private void RestoreRenderer(Renderer renderer)
    {
        if (renderer != null && originalShadowModes.ContainsKey(renderer))
        {
            renderer.shadowCastingMode = originalShadowModes[renderer];
            originalShadowModes.Remove(renderer);
        }
    }
    private void OnDisable()
    {
        foreach (var kvp in originalShadowModes)
        {
            if (kvp.Key != null)
            {
                kvp.Key.shadowCastingMode = kvp.Value;
            }
        }
        originalShadowModes.Clear();
        currentlyBlockingRenderers.Clear();
        previouslyBlockingRenderers.Clear();
    }
}