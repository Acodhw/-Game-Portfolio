using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public enum ShootDirectionMode
    {
        FixedVector,
        TransformForward   
    }

    [Header("References")]
    [Tooltip("발사할 투사체 프리팹")]
    public GameObject projectilePrefab;
    [Tooltip("투사체가 생성될 위치")]
    public Transform firePoint;

    [Header("Shoot Settings")]
    [Tooltip("활성화(OnEnable) 될 때 자동으로 발사할지 여부")]
    public bool shootOnEnable = false;
    [Tooltip("투사체의 이동 속도")]
    public float projectileSpeed = 10f;
    [Tooltip("투사체의 생존 시간 (초)")]
    public float projectileLifeTime = 3f;

    [Header("Direction Settings")]
    [Tooltip("발사 방향 결정 방식")]
    public ShootDirectionMode directionMode = ShootDirectionMode.TransformForward;

    [Tooltip("FixedVector 모드일 때 사용할 고정 방향 (예: (1,0,0) 이면 X축 방향)")]
    public Vector3 fixedDirection = Vector3.forward;

    [Tooltip("TransformForward 모드일 때 기준이 될 Transform")]
    public Transform forwardReference;

    private void OnEnable()
    {
        if (shootOnEnable)
        {
            Shoot();
        }
    }
    public void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            return;
        }
        Vector3 shootDir = Vector3.zero;

        if (directionMode == ShootDirectionMode.FixedVector)
        {
            shootDir = fixedDirection;
        }
        else if (directionMode == ShootDirectionMode.TransformForward)
        {
            Transform refTransform = (forwardReference != null) ? forwardReference : firePoint;
            shootDir = refTransform.forward;
        }
        Quaternion spawnRotation = firePoint.rotation;
        if (shootDir != Vector3.zero)
        {
            spawnRotation = Quaternion.LookRotation(shootDir);
        }
        GameObject projObj = Instantiate(projectilePrefab, firePoint.position, spawnRotation);
        EnemyShots enemyShot = projObj.GetComponent<EnemyShots>();
        if (enemyShot != null)
        {
            enemyShot.Initialize(shootDir, projectileSpeed, projectileLifeTime);
        }
    }
}