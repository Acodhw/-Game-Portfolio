using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class ComponentChangingObj : MonoBehaviour
{
    [System.Serializable]
    public class ComponentInitData
    {
        public bool isAdded = false;
        public float value = 0f;
    }

    [Header("Inspector Initial Settings (Transform & Mass)")]
    public ComponentInitData ScaleX = new ComponentInitData { value = 1f };
    public ComponentInitData ScaleY = new ComponentInitData { value = 1f };
    public ComponentInitData ScaleZ = new ComponentInitData { value = 1f };
    public ComponentInitData RotX = new ComponentInitData { value = 0f };
    public ComponentInitData RotY = new ComponentInitData { value = 0f };
    public ComponentInitData RotZ = new ComponentInitData { value = 0f };
    public ComponentInitData Mass = new ComponentInitData { value = 1f };

    [Header("Inspector Initial Settings (Flags Only)")]
    public bool Trigger = false;
    public bool Physics = false;
    public bool Elasticity = false;

    [Header("Physics Materials")]
    public PhysicMaterial normalMaterial;
    public PhysicMaterial bouncyMaterial;

    [HideInInspector]
    public Dictionary<string, float> changedTransform;

    [HideInInspector]
    public Dictionary<string, bool> AddedComp;

    public bool HasTriggerSkill
    {
        get { return AddedComp != null && AddedComp.ContainsKey("Trigger") && AddedComp["Trigger"]; }
    }

    private Vector3 originalScale;
    private Quaternion originalLocalRotation;
    private float originalMass;
    private PhysicMaterial originalMaterial;

    private Rigidbody rb;
    private bool isPaused = false;
    private bool wasKinematicBeforePause;
    private bool isInitialized = false;

    void Awake()
    {
        InitBaseStatus();
    }

    private void InitBaseStatus()
    {
        if (isInitialized) return;

        rb = GetComponent<Rigidbody>();
        InitializeDictionaries();

        originalScale = transform.localScale;
        originalLocalRotation = transform.localRotation;
        originalMass = rb.mass;

        Collider firstCol = GetComponent<Collider>();
        if (firstCol != null) originalMaterial = firstCol.sharedMaterial;

        isInitialized = true;
    }

    void Start()
    {
        ApplyAttributes();
    }

    private void InitializeDictionaries()
    {
        changedTransform = new Dictionary<string, float>() {
            {"ScaleXMul", ScaleX.value}, {"ScaleYMul", ScaleY.value}, {"ScaleZMul", ScaleZ.value},
            {"RotX", RotX.value}, {"RotY", RotY.value}, {"RotZ", RotZ.value}, {"MassMul", Mass.value}
        };

        AddedComp = new Dictionary<string, bool>() {
            {"ScaleXMul", ScaleX.isAdded}, {"ScaleYMul", ScaleY.isAdded}, {"ScaleZMul", ScaleZ.isAdded},
            {"RotX", RotX.isAdded}, {"RotY", RotY.isAdded}, {"RotZ", RotZ.isAdded},
            {"Trigger", Trigger}, {"MassMul", Mass.isAdded}, {"Physics", Physics}, {"Elasticity", Elasticity}
        };
    }

    public void PausePhysics()
    {
        isPaused = true;
        wasKinematicBeforePause = rb.isKinematic;
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void ResumePhysics()
    {
        isPaused = false;
        ApplyAttributes();
        if (!rb.isKinematic) rb.WakeUp();
    }

    public void ApplyAttributes()
    {
        if (!isInitialized) InitBaseStatus();

        float sx = AddedComp["ScaleXMul"] ? changedTransform["ScaleXMul"] : 1f;
        float sy = AddedComp["ScaleYMul"] ? changedTransform["ScaleYMul"] : 1f;
        float sz = AddedComp["ScaleZMul"] ? changedTransform["ScaleZMul"] : 1f;
        transform.localScale = new Vector3(originalScale.x * sx, originalScale.y * sy, originalScale.z * sz);

        float rx = AddedComp["RotX"] ? changedTransform["RotX"] : 0f;
        float ry = AddedComp["RotY"] ? changedTransform["RotY"] : 0f;
        float rz = AddedComp["RotZ"] ? changedTransform["RotZ"] : 0f;
        transform.localRotation = originalLocalRotation * Quaternion.Euler(rx, ry, rz);

        rb.mass = AddedComp["MassMul"] ? (originalMass * changedTransform["MassMul"]) : originalMass;
        rb.isKinematic = !AddedComp["Physics"];

        bool applyTrigger = AddedComp["Trigger"];
        

        Collider[] allColliders = GetComponents<Collider>();

        foreach (Collider c in allColliders)
        {
            if (c is MeshCollider meshCollider && applyTrigger) meshCollider.convex = true;
            c.isTrigger = applyTrigger;

            if (AddedComp["Elasticity"])
            {
                if (bouncyMaterial != null) c.sharedMaterial = bouncyMaterial;
            }
            else
            {
                if (normalMaterial != null) c.sharedMaterial = normalMaterial;
                else c.sharedMaterial = originalMaterial;
            }
        }
    }

    public ComponentChangingSaveData ExtractSaveData()
    {
        if (!isInitialized) InitBaseStatus();

        ComponentChangingSaveData data = new ComponentChangingSaveData();
        data.isActive = gameObject.activeSelf;

        foreach (var kvp in changedTransform)
            data.floatData.Add(new StringFloatPair { key = kvp.Key, value = kvp.Value });

        foreach (var kvp in AddedComp)
            data.boolData.Add(new StringBoolPair { key = kvp.Key, value = kvp.Value });

        return data;
    }

    public void ApplySaveData(ComponentChangingSaveData data)
    {
        if (!isInitialized) InitBaseStatus();

        foreach (var pair in data.floatData)
            if (changedTransform.ContainsKey(pair.key)) changedTransform[pair.key] = pair.value;

        foreach (var pair in data.boolData)
            if (AddedComp.ContainsKey(pair.key)) AddedComp[pair.key] = pair.value;

        ApplyAttributes(); 
    }
}