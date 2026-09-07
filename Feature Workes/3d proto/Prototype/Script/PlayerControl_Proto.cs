using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.Cinemachine;

public enum CameraFocusMode {
    Distance,
    PlayerSight,
    CameraSight,
}

public class PlayerControl_Proto : MonoBehaviour
{ 
    private CharacterController controller; // 현재 캐릭터가 가지고있는 캐릭터 컨트롤러 콜라이더.
    private CinemachineFreeLook playerCamera; // 캐릭터 카메라
    private CinemachineTargetGroup targetGroup; // 물체를 포커싱할때 쓰일 그룹
    private Transform characterModel; // 캐릭터 본체
    private Transform cameraSight; // 실질 카메라
    private Transform focusEnemy; // 집중 중인 물체

    private Vector3 MoveDir;// 캐릭터의 움직임 저장

    private float cameraDistance = 15f; // 조정된 카메라의 시야 길이
    private bool isOnGround; // 땅에 닿았는지 체크
    private bool isInFocus; // 현재 집중모드인지 체크


    // 상수
    private const float gravityAcc = 9.81f; // 중력가속도
    private const float speedDefValue = 50; // 스피드 벨류
    private const float cameraDistMax = 25f; // 카메라 거리 한계(최대치)
    private const float cameraDistMin = 5f; // 카메라 거리 한계(최소치)
    private const float cameraAngleLimit = 35; // 주목 시 캐릭터와 카메라의 각도가 이정도 차이가 나면 돌아갑니다

    [Header("Player Setting Value")]
    [SerializeField][Tooltip("이동 속도를 지정합니다")][Range(0, 100)]
    private float speed = 10;
    [SerializeField][Tooltip("점프 세기를 지정합니다")][Range(0, 50)]
    private float jumpSpeed = 5;
    [SerializeField][Tooltip("땅부터 캐릭터 높이를 지정합니다")][Range(0, 3)]
    private float groundHeight = 1;
    [SerializeField][Tooltip("중력 강도를 정합니다")]
    private float gravityScale = 1;
    [SerializeField][Tooltip("집중으로 찾는 범위를 지정합니다")][Range(0, 100)]
    private float enemyFoundRange = 25;
    [SerializeField][Tooltip("집중 우선순위를 설정합니다")]
    private CameraFocusMode fmode;

    [Header("Camera Setting Value")]
    [SerializeField][Tooltip("카메라 줌인 속도를 정합니다")][Range(0, 100)]
    private float cameraZoomSpeed;

    [Header("UI Settings")]
    [SerializeField][Tooltip("타깃을 지정할때 나타나는 점입니다.")]
    private RectTransform targetPointImg;

    // Start is called before the first frame update
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        characterModel = transform.GetChild(1);
        playerCamera = transform.GetChild(0).GetComponent<CinemachineFreeLook>();
        cameraSight = playerCamera.transform.GetChild(0);
        targetGroup = transform.GetChild(2).GetComponent<CinemachineTargetGroup>();
    }

    // Update is called once per frame
    private void Update()
    {
        FocusUI();
        EnemyFocus();
        CameraDistanceSetting();
        MoveCharacter();
    }

    private void FixedUpdate()
    {
        GroundCheck();
    }

    // 캐릭터 이동에 관한 함수
    private void GroundCheck()
    {
        RaycastHit rh;
        Physics.BoxCast(transform.position, new Vector3(0.75f, 0.5f, 0.75f) * 0.5f, Vector3.down, out rh, transform.rotation, groundHeight, LayerMask.GetMask("Ground"));
        isOnGround = (rh.collider != null);
    }

    private void MoveCharacter()
    {
        RaycastHit rh;
        Physics.BoxCast(transform.position, new Vector3(0.75f, 0.5f, 0.75f) * 0.5f, Vector3.down, out rh, transform.rotation, groundHeight);
        //포커싱 중이면 캐릭터를 적 방향으로 바라보게
        if (isInFocus) characterModel.rotation = Quaternion.Slerp(characterModel.rotation, Quaternion.LookRotation(new Vector3(focusEnemy.position.x, transform.position.y, focusEnemy.position.z) - transform.position), 0.2f);

        if (isOnGround && Vector3.Angle(rh.normal, Vector3.up) <= controller.slopeLimit)
        {
            // 입력 움직임 세팅. 
            Vector3 vec = (Vector3.forward * Input.GetAxisRaw("Vertical")
                + Vector3.right * Input.GetAxisRaw("Horizontal")).normalized; ;
            if (!isInFocus) vec = Quaternion.AngleAxis(playerCamera.m_XAxis.Value, Vector3.up) * vec;
            // 포커싱 x => 카메라 기반 움직임
            else vec = Quaternion.AngleAxis(characterModel.eulerAngles.y, Vector3.up) * vec;
            // 포커싱 o => 캐릭터 기반 움직임

            //입력값이 존재하지 않는다면 앞 방향 가리키기
            if ((Input.GetButton("Vertical") || Input.GetButton("Horizontal")) && vec.magnitude <= 0) vec = Quaternion.AngleAxis(playerCamera.m_XAxis.Value, Vector3.up) * Vector3.forward;

            // 플레이어 회전
            if (vec.magnitude > 0 && !isInFocus) characterModel.rotation = Quaternion.Slerp(characterModel.rotation, Quaternion.LookRotation(vec), 0.2f);
            // 캐릭터 점프
            MoveDir.y = -9.81f;
            if (Input.GetButton("Jump"))
            {
                //점프로 바로 플레이어 움직이기
                if (!isInFocus) characterModel.rotation = Quaternion.LookRotation(vec);
                MoveDir.y = jumpSpeed;
            }

            // 입력된 방향으로 움직인다
            MoveDir = vec * speed * vec.magnitude + Vector3.up * MoveDir.y;
        }
        else
        {
            MoveDir.y -= gravityAcc * Time.deltaTime * gravityScale; //중력 처리
            //바닥이 charactercontroller에서 제시한 각도보다 큰 각도일경우 미끄러짐 처리
            if (rh.collider != null && Vector3.Angle(rh.normal, Vector3.up) > controller.slopeLimit)
            {
                Vector3 vec = -Vector3.ProjectOnPlane(-Physics.gravity, rh.normal).normalized;
                MoveDir = vec * (MoveDir.y / vec.y);
            }
        }
        controller.Move(MoveDir * Time.deltaTime); // 캐릭터 움직임.
    }

    // 카메라 이동에 관한 함수
    private void CameraDistanceSetting()
    {
        
        if (isInFocus)
        {
            float cdist  = Vector3.Distance(characterModel.position, focusEnemy.position) * 1.01f + 0.5f;
            cdist = Mathf.Clamp(cdist, cameraDistMin, cameraDistMax);
            playerCamera.m_Orbits[0].m_Height = cdist / 3;
            playerCamera.m_Orbits[0].m_Radius = cdist / 2;
            playerCamera.m_Orbits[1].m_Radius = cdist;
            playerCamera.m_Orbits[2].m_Height = -cdist / 4;
            playerCamera.m_Orbits[2].m_Radius = cdist / 2;
        }
        else
        {
            cameraDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * cameraZoomSpeed * speedDefValue;
            cameraDistance = Mathf.Clamp(cameraDistance, cameraDistMin, cameraDistMax);
            playerCamera.m_Orbits[0].m_Height = cameraDistance - 0.5f;
            playerCamera.m_Orbits[1].m_Radius = cameraDistance;
            playerCamera.m_Orbits[2].m_Height = -cameraDistance + 0.5f;
        }
    }

    private void FocusCameraRotate()
    {
        // 각도 차 구하기
        float diff = (playerCamera.m_XAxis.Value - characterModel.eulerAngles.y) % 360;
        if (diff < -180) diff += 360;
        if (diff > 180) diff -= 360;
        // 플레이어 각도가 너무 벗어나 있을 때, 플레이어 방향으로 카메라를 이동시킨다
        if (Mathf.Abs(diff) > cameraAngleLimit)
        {
            float x = Vector3.Distance(
                characterModel.position - characterModel.position.y * Vector3.up,
                focusEnemy.position - focusEnemy.position.y * Vector3.up);

            playerCamera.m_XAxis.Value -= (diff - ((diff > 0 ? 1 : -1) * cameraAngleLimit)) * Time.deltaTime * 50 * (1/x);
        }
    }

    // 락 온 시스템 함수
    private void EnemyFocus()
    {
        if (isInFocus) FocusCameraRotate();
        // 포커스 명령 입력 - 홀드
        if (Input.GetButton("Focus"))
        {
            Transform[] focusFinded;
            focusFinded = Array.ConvertAll(Physics.OverlapSphere(transform.position, enemyFoundRange, LayerMask.GetMask("Enemy")), item => item.transform);
            // 포커스 대상이 포커스에 없으면 포커스를 해제
            if (!Array.Exists(focusFinded, x => x.Equals(focusEnemy))) FocusCancel();

            if (focusFinded.Length <= 0) FocusCancel(); // 포커스 대상이 아예 없으면 끝내기
            else if (!isInFocus)
            {
                FocusingEnemy(focusFinded); // 홀드 상태면 포커스 온
            }
        }
        else FocusCancel(); // 홀드 상태가 아니면 포커스 끄기
        targetGroup.m_Targets[1].Object = focusEnemy;
    }

    private void FocusCancel()
    {
        focusEnemy = null;
        isInFocus = false;
    }

    private void FocusingEnemy(Transform[] enemys)
    {
        //현재 모드에 따라 찾은 오브젝트를 정렬한다
        switch (fmode)
        {
            case CameraFocusMode.Distance:
                focusEnemy = enemys.OrderBy(x =>
                    Vector3.Distance(transform.position, x.position)).ToArray()[0];
                break;
            case CameraFocusMode.PlayerSight:
                focusEnemy = enemys.OrderBy(x =>
                    Vector3.Angle(characterModel.forward, x.position - characterModel.position)).ToArray()[0];
                break;
            case CameraFocusMode.CameraSight:
                focusEnemy = enemys.OrderBy(x =>
                    Vector3.Angle(cameraSight.forward, x.position - cameraSight.position)).ToArray()[0];
 
                break;
        }
        isInFocus = true;
    }

    private void FocusUI() {
        if (isInFocus)
        {
            targetPointImg.gameObject.SetActive(true);
            targetPointImg.position = Camera.main.WorldToScreenPoint(focusEnemy.position);
        }
        else {
            targetPointImg.gameObject.SetActive(false);
        }
    }

    // 디버그
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position + Vector3.down * groundHeight, new Vector3(0.75f, 0.5f, 0.75f));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemyFoundRange);
    }
}
