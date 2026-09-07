using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfoCameraShow : MonoBehaviour
{
    
    private enum Mode
    {
        LookAt,
        LookAtInverted, //* 반전 시켜 보기
        CameraForward,
        CameraForwardInverted, //* 반전 시켜 보기
    }
    [SerializeField] private Transform csamera;
    [SerializeField] private Mode mode;

    private void Start()
    {
        csamera = GameObject.Find("PlayerPrefeb").transform.GetChild(1).GetChild(0).GetChild(0);
    }
    private void LateUpdate()
    {
        switch (mode)
        {
            case Mode.LookAt:
                transform.LookAt(csamera);
                break;
            case Mode.LookAtInverted:
                //* 카메라 방향을 알아내서 그 방향 만큼 돌려줘서 반전시키기
                Vector3 dirFromCamera = transform.position - csamera.position;
                transform.LookAt(transform.position + dirFromCamera);
                break;
            case Mode.CameraForward:
                //* 카메라 방향으로 Z축 (앞뒤)을 바꿔주기
                transform.forward = csamera.forward;
                break;
            case Mode.CameraForwardInverted:
                //* 카메라 방향으로 Z축 (앞뒤)을 바꿔주고 반전시키기
                transform.forward = -csamera.forward;
                break;
            default:

                break;
        }
    }
}
