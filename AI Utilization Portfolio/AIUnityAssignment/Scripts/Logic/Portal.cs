using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class Portal : MonoBehaviour
{
    public enum PortalMode { EventOnly, Interact, Touch }

    [Header("Destination Settings")]
    public string nextSceneName;
    public Vector3 nextPlayerPosition;
    public Vector3 nextMapRotationEuler;

    [Header("Portal Settings")]
    public PortalMode portalMode = PortalMode.Interact;
    public AudioClip portalSound;
    public float fadeDuration = 1.0f;

    private bool isExecuting = false;

    private void Start()
    {  
        if (portalMode == PortalMode.Touch)
        {
            Collider col = GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
        }
    }

    public void StartInteraction()
    {
        if (portalMode == PortalMode.Interact || portalMode == PortalMode.EventOnly)
        {
            ExecutePortal();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isExecuting) return;
        if (portalMode == PortalMode.Touch && other.CompareTag("Player"))
        {
            ExecutePortal();
        }
    }

    public void ExecutePortal()
    {
        if (isExecuting) return;
        isExecuting = true;

        if (AudioManager.Instance != null && portalSound != null)
        {

            AudioManager.Instance.PlaySFX(portalSound, true);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetPortalData(nextSceneName, nextPlayerPosition, nextMapRotationEuler);
        }

        StartCoroutine(PortalRoutine());
    }

    private IEnumerator PortalRoutine()
    {
        if (UIManager.Instance != null && UIManager.Instance.fadeImage != null)
        {
            UIManager.Instance.isTalking = true;
            UIManager.Instance.fadeImage.gameObject.SetActive(true);

            Color color = UIManager.Instance.fadeImage.color;
            float time = 0f;

            while (time < fadeDuration)
            {
                time += Time.unscaledDeltaTime;
                color.a = Mathf.Lerp(0f, 1f, time / fadeDuration);
                UIManager.Instance.fadeImage.color = color;
                yield return null;
            }

            color.a = 1f;
            UIManager.Instance.fadeImage.color = color;
        }
        else
        {
            yield return new WaitForSeconds(fadeDuration);
        }

        LoadingScene.LoadScene(nextSceneName);
    }
}