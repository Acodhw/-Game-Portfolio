using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("Map Settings")]
    public string sceneID;
    public string sceneDisplayName = "미지의 구역";
    public Transform map;

    [Header("Save/Load Objects (New Categories)")]
    [Tooltip("이동 가능한 범용 오브젝트 리스트")]
    public List<GameObject> movableObjects = new List<GameObject>();

    [Tooltip("컴포넌트 변형 오브젝트 리스트")]
    public List<ComponentChangingObj> componentObjects = new List<ComponentChangingObj>();

    private Vector3 savedMapPos;
    private Quaternion savedMapRot;
    private Dictionary<Rigidbody, bool> savedRbStates = new Dictionary<Rigidbody, bool>();
    private bool hasSavedState = false;

    public bool HasSavedState => hasSavedState;
    public Vector3 SavedMapPos => savedMapPos;
    public Quaternion SavedMapRot => savedMapRot;

    private void Awake()
    {
        if (map == null)
        {
            map = this.transform;
        }
    }

    void Start()
    {
        if (string.IsNullOrEmpty(sceneID))
            sceneID = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (GameManager.Instance != null)
        {
            MapData savedData = GameManager.Instance.GetMapData(sceneID);
            if (savedData != null)
            {
                ApplyMapData(savedData);
            }
            else
            {
                GameManager.Instance.UpdateMapData(sceneID, ExtractCurrentMapData());
            }

            if (GameManager.Instance.hasPendingPortal)
            {
                PlayerControl player = FindObjectOfType<PlayerControl>();
                if (player != null)
                {
                    CharacterController cc = player.GetComponent<CharacterController>();
                    if (cc != null) cc.enabled = false;
                    Physics.SyncTransforms();

                    player.transform.position = GameManager.Instance.pendingPlayerPosition;

                    if (cc != null) cc.enabled = true;
                }

                GameManager.Instance.hasPendingPortal = false;
            }
        }
    }

    public MapData ExtractCurrentMapData()
    {
        MapData data = new MapData();

        if (map != null)
            data.mapRotation = map.rotation;

        foreach (var obj in movableObjects)
        {
            if (obj == null)
            {
                data.movableObjects.Add(new ObjectStateData { isActive = false });
                continue;
            }

            data.movableObjects.Add(new ObjectStateData
            {
                localPosition = obj.transform.localPosition,
                localRotation = obj.transform.localRotation,
                localScale = obj.transform.localScale,
                isActive = obj.activeSelf
            });
        }

        foreach (var compObj in componentObjects)
        {
            if (compObj == null)
            {
                data.componentObjects.Add(new ComponentChangingSaveData { isActive = false });
                continue;
            }
            data.componentObjects.Add(compObj.ExtractSaveData());
        }

        return data;
    }

    private void ApplyMapData(MapData data)
    {
        if (map != null)
            map.rotation = data.mapRotation;

        for (int i = 0; i < movableObjects.Count && i < data.movableObjects.Count; i++)
        {
            if (movableObjects[i] == null) continue;

            movableObjects[i].SetActive(data.movableObjects[i].isActive);
            if (data.movableObjects[i].isActive)
            {
                movableObjects[i].transform.localPosition = data.movableObjects[i].localPosition;
                movableObjects[i].transform.localRotation = data.movableObjects[i].localRotation;
                movableObjects[i].transform.localScale = data.movableObjects[i].localScale;
            }
        }

        for (int i = 0; i < componentObjects.Count && i < data.componentObjects.Count; i++)
        {
            if (componentObjects[i] == null) continue;

            componentObjects[i].gameObject.SetActive(data.componentObjects[i].isActive);
            if (data.componentObjects[i].isActive)
            {
                componentObjects[i].ApplySaveData(data.componentObjects[i]);
            }
        }
    }
    public void SaveState()
    {
        if (map == null) return;

        savedMapPos = map.position;
        savedMapRot = map.rotation;

        savedRbStates.Clear();
        foreach (Rigidbody rb in map.GetComponentsInChildren<Rigidbody>())
        {
            savedRbStates[rb] = rb.isKinematic;
            rb.isKinematic = true;
        }
        hasSavedState = true;
    }

    public void RestoreState()
    {
        if (!hasSavedState || map == null) return;
        map.position = savedMapPos;
        map.rotation = savedMapRot;
        RestoreRigidbodies();
        hasSavedState = false;
    }

    public void ApplyState(Vector3 newPos, Quaternion newRot)
    {
        if (map == null) return;
        map.position = newPos;
        map.rotation = newRot;
        RestoreRigidbodies();
        hasSavedState = false;
    }

    public void RestoreRigidbodies()
    {
        foreach (var kvp in savedRbStates)
        {
            if (kvp.Key != null) kvp.Key.isKinematic = kvp.Value;
        }
        savedRbStates.Clear();
    }
}