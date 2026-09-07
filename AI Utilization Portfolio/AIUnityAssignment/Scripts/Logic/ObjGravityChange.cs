using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjGravityChange : MonoBehaviour
{
    [Header("Map Reference")]
    [Tooltip("맵 오브젝트")]
    public Transform mapTransform;

    [Header("Gravity Settings")]
    [Tooltip("기본 중력 방향을 설정할 각도 (맵 회전이 0,0,0일 때 기준)")]
    public Vector3 baseGravityEuler = Vector3.zero;

    [Tooltip("중력 배수 (1 = 기본 중력 9.81, 2 = 2배 빠르게 추락, -1 = 반대로 추락)")]
    public float gravityScale = 1f;

    [Header("Knockback Settings")]
    [Tooltip("PlayerAttack에 맞았을 때 밀려나는 힘의 크기")]
    public float knockbackForce = 10f;

    [Tooltip("2D 모드 활성화 (체크 시 좌/우로만 넉백 적용)")]
    public bool is2DMode = false;

    private Rigidbody rb;
    private Vector3 baseGravityDir;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        if (mapTransform == null)
        {
            mapTransform = transform.root;
        }
        baseGravityDir = Quaternion.Euler(baseGravityEuler) * Vector3.down;
    }

    void FixedUpdate()
    {
        if (mapTransform == null) return;
        Vector3 currentGravityDir = mapTransform.rotation * baseGravityDir;
        float standardGravity = Physics.gravity.magnitude;
        rb.AddForce(currentGravityDir * (standardGravity * gravityScale), ForceMode.Acceleration);
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerAttack attack = other.GetComponent<PlayerAttack>();
        if (attack != null)
        {
            Debug.Log($"[ObjGravityChange] OnTriggerEnter 감지됨! 공격자: {other.name}, AttackVector: {attack.AttackVector}");
            ApplyKnockback(attack.AttackVector);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        PlayerAttack attack = collision.gameObject.GetComponent<PlayerAttack>();
        if (attack != null)
        {
            ApplyKnockback(attack.AttackVector);
        }
    }

    private void ApplyKnockback(Vector3 rawAttackDir)
    {
        if (rawAttackDir.sqrMagnitude < 0.001f)
        {
            return;
        }

        Vector3 snappedDirection;

        if (is2DMode)
        {
            snappedDirection = rawAttackDir.x >= 0 ? Vector3.right : Vector3.left;
        }
        else
        {
            snappedDirection = SnapToCardinalDirection(rawAttackDir);
        }
        rb.velocity = new Vector3(
            snappedDirection.x * knockbackForce,
            rb.velocity.y,
            snappedDirection.z * knockbackForce
        );
    }

    private Vector3 SnapToCardinalDirection(Vector3 dir)
    {
        dir.y = 0;
        if (dir.sqrMagnitude < 0.001f) return Vector3.zero;
        dir.Normalize();

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
        {
            return dir.x > 0 ? Vector3.right : Vector3.left;
        }
        else
        {
            return dir.z > 0 ? Vector3.forward : Vector3.back;
        }
    }
}