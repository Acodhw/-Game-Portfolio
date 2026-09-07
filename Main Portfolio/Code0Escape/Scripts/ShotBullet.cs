using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotBullet : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform playerCamera;
    // Start is called before the first frame update
    private void OnEnable()
    {
        Instantiate(bullet, playerCamera.position + playerCamera.forward * 1.75f, playerCamera.rotation);
    }
}
