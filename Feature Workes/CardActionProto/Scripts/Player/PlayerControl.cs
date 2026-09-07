using Cinemachine;
using Cinemachine.Utility;
using JetBrains.Annotations;
using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] CinemachineTargetGroup targetGroup;
    [SerializeField] Transform SetCameraTransform;

    CharacterBase[] characters;
    PlayerManager playerManager;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    
    CCinfo[] cc = new CCinfo[4];

    bool isPause = false;
    bool isOnGround = false;
    bool rayCaston = true;
    bool jumpKeyCheck = false;
    bool ccMoveCheck = false;
    bool isHit = false;
    bool isCanGetDamageToTrap = true;
    bool retire = false;
    bool frameAvoid = false;
    bool frameAvoidCool = true;
    bool isonSlop = false;

    float jumpT;
    float jumpP = 10f;
    float jumpTL = 0.125f;
    float speedValue = 1;

    [SerializeField] Vector3 GroundnormalVector;
    [SerializeField]Vector2 GroundSpeed;

    private int[] selectedMoveSkills = new int[3];

    private Color nowColor;

    [Header("Setting Value")]
    [SerializeField] float maxSpeed = 8;
    [SerializeField] Material hitMatetrial;
    Material originalMatetrial;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        playerManager = GameObject.Find("GameManager").GetComponent<PlayerManager>();
        playerManager.SetPlayer(this);
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMatetrial = spriteRenderer.material;
        System.Array.Resize(ref characters, playerManager.transform.childCount);
        for (int i = 0; i < playerManager.transform.childCount; i++)
        {
            characters[i] = playerManager.transform.GetChild(i).GetComponent<CharacterBase>();
            if (characters[i] != null) characters[i].setPlayerControl(this);
        }
        characters[playerManager.GetCharacter()].ChildObjectSetting();
    }

    private void Update()
    {
        RaycastHit2D ray = Physics2D.Raycast(rigid.transform.position + (spriteRenderer.flipX ? Vector3.left : Vector3.right) * 0.14f, Vector2.down, 1.7f, LayerMask.GetMask("Ground"));
        GroundnormalVector = ray.normal.normalized;
        isonSlop = (GroundnormalVector != Vector3.up && isOnGround);
        if (playerManager.GetIsDie() && !retire) 
        {
            retire = true;
            anim.SetBool("Retire", ChangeMotionCheck("Retire", true));
        }
        if (retire)
        {                     
            Invoke("SceneRestart", 5);
            return;
        }
        GetComponent<CapsuleCollider2D>().size = characters[playerManager.GetCharacter()].GetSizeValue();
        GetComponent<CapsuleCollider2D>().offset = new Vector2(0, (characters[playerManager.GetCharacter()].GetSizeValue().y - 1.84f) * 0.5f - 0.05f);

        if (!isHit) playerManager.SetIsHit(false);
        if (cc[0].cc != ClowdControl.Stun && cc[3].cc == ClowdControl.None)
        {            
            if (isPause) {
                anim.SetBool("IsMoving", false);
                rigid.drag = isOnGround && rigid.velocity.x != 0 ? 5 : 0;
                return; 
            }
            if (characters[playerManager.GetCharacter()].GetIsAttaking(true) && characters[playerManager.GetCharacter()].GetIsUsingSkill(true) && !playerManager.GetIsUsingMoveSkill())
            {
                Move();
                Sprint();
                if (characters[playerManager.GetCharacter()].GetIsAttaking() && characters[playerManager.GetCharacter()].GetIsUsingSkill() && !playerManager.GetIsFlying()) { 
                    FrameAvoid();
                    if (Input.GetButtonDown("CharacterChange"))
                    {
                        CharacterChange(Input.GetAxisRaw("CharacterChange") < 0 ? true : false);
                    }           
                }
            }           
            Jump();
            if (playerManager.GetIsFlying()  && !isOnGround && (rigid.velocity.y > 0 || rigid.velocity.y < 0.2f)) rigid.velocity = rigid.velocity.x * Vector2.right;
            
            if (isonSlop) rigid.gravityScale = 0;
            else if(!characters[playerManager.GetCharacter()].GetisMovingBySkill() && !playerManager.GetIsUsingMoveSkill() && !playerManager.GetIsFlying()) rigid.gravityScale = 6;

            if (characters[playerManager.GetCharacter()].GetIsArmorSetting())
            {
                if (!playerManager.GetIsFlying()) playerManager.UsingMoveSkill();
                characters[playerManager.GetCharacter()].Attack();
                if (cc[2].cc == ClowdControl.None && !playerManager.GetIsUsingMoveSkill())
                    characters[playerManager.GetCharacter()].UsingSkills();
            }
        }
        else
        {            
            if (cc[3].cc != ClowdControl.None && !ccMoveCheck) StartCoroutine("MovingWithCC");
        }
    }

    private void FixedUpdate()
    {
        anim.SetFloat("Y Velocity", rigid.velocity.y);
        RaycastHit2D raycastHit = Physics2D.BoxCast(rigid.transform.position, new Vector3(0.7f, 0.35f, 0), 0f, Vector2.down, 0.925f, LayerMask.GetMask("Ground")); ;     
        if (rayCaston)
        {
            isOnGround = (raycastHit.collider != null);
            if (characters[playerManager.GetCharacter()].GetIsUsingSkill() && !playerManager.GetIsUsingMoveSkill() && characters[playerManager.GetCharacter()].GetIsAttaking()) anim.SetBool("OnGround", ChangeMotionCheck("OnGround", isOnGround));
            else anim.SetBool("OnGround", isOnGround);
        }

        if (isOnGround)
        {
            transform.SetParent(raycastHit.collider.transform);          
            //GroundSpeed = raycastHit.collider.GetComponent<Rigidbody2D>().velocity;
            MovingPlatform mp = raycastHit.collider.GetComponent<MovingPlatform>();
            if (mp == null)
            {            
                GroundSpeed = Vector2.zero;
            }
            else {
                GroundSpeed = mp.GetTileVelocity();
            }
        }
        else
        {
            transform.SetParent(null);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((collision.tag.Equals("EnemyAttack") && !frameAvoid) || collision.tag.Equals("Trap")) && !playerManager.GetPlayerHasCondition("_Invincible") && !playerManager.GetIsDie())
        {
            isHit = true;
            if (!playerManager.GetPlayerHasCondition("_Protection")) spriteRenderer.material = hitMatetrial;
            if (collision.tag.Equals("EnemyAttack")) playerManager.SetIsHit(true);           
            if (!(collision.tag.Equals("Trap") && !isCanGetDamageToTrap)) playerManager.Damage(collision.GetComponent<PlayerGetDamage>().GetDamage(), true);
            if (collision.tag.Equals("Trap")) isCanGetDamageToTrap = false;
            StartCoroutine("ResetMatarial", 0.1f);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //if ((collision.tag.Equals("EnemyAttack") || collision.tag.Equals("Trap")) && !playerManager.GetPlayerHasCondition("_Invincible") && !playerManager.GetIsDie()) isHit = true;
        if (collision.tag.Equals("DeathZone"))
        {
            rigid.velocity = (rigid.velocity.y * Vector2.up);
            playerManager.Execute(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("EnemyAttack") || collision.tag.Equals("Trap") && !playerManager.GetIsDie()) isHit = false;       
    }

    IEnumerator ResetMatarial(float time)
    {
        yield return new WaitForSeconds(time);
        isCanGetDamageToTrap = true;
        if (!isHit) resetMatatrial();       
        else
        {
            isHit = false;
            StartCoroutine("ResetMatarial", 0.1f);
        }
    }

    public void SceneRestart()
    {
        foreach (CharacterBase c in characters) if (c != null) {  c.StoppingCourutineWhenCCed(); c.ResetCooltime(); }
        playerManager.StopCourutineWhenCCed();
        playerManager.ResetState();    
        playerManager.ResetCooltime();
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void resetMatatrial() {
        spriteRenderer.material = originalMatetrial;
    }

    private void Move()
    {
        float characterSpeedValue = characters[playerManager.GetCharacter()].ReturnSpeedValue();
        bool ismoving;
        float CCMoveValue = cc[0].cc == ClowdControl.Slow || cc[0].cc == ClowdControl.Weakleg ? 1 - (cc[0].ccStrength * 0.01f) : 1;
        if (Input.GetButton("Horizontal") && cc[0].cc != ClowdControl.Bound)
        {
            rigid.drag = 0;
            if (characters[playerManager.GetCharacter()].GetCanFlip()) spriteRenderer.flipX = (Input.GetAxisRaw("Horizontal") < 0);
            else {
                if((spriteRenderer.flipX && Input.GetAxisRaw("Horizontal") == 1) || (!spriteRenderer.flipX && Input.GetAxisRaw("Horizontal") == -1)) anim.SetFloat("BackSteping", -1);
                else anim.SetFloat("BackSteping", 1);
            }

            if (isOnGround) { 
                rigid.AddForce(Input.GetAxisRaw("Horizontal") * Vector2.right * 1.5f * speedValue * characterSpeedValue, ForceMode2D.Impulse);
            }
            else rigid.AddForce(Input.GetAxisRaw("Horizontal") * Vector2.right * 0.19f * speedValue * characterSpeedValue, ForceMode2D.Impulse);
          
            ismoving = (Input.GetAxisRaw("Horizontal") == 0) ? false : true;            
        
        }
        else
        {
            rigid.drag = (isOnGround && rigid.velocity.x != 0) ? 5 : 0;
            ismoving = false;       
        }
        //if (isOnGround && Input.GetAxisRaw("Horizontal") == 0) rigid.velocity = rigid.velocity.x * 0.9f * Vector2.right + Vector2.up * rigid.velocity.y;
        anim.SetBool("IsMoving", ismoving);

        if (isOnGround)
        {
            if (rigid.velocity.x > maxSpeed * speedValue * CCMoveValue * characterSpeedValue) rigid.velocity = Vector3.ProjectOnPlane(new Vector2(maxSpeed * speedValue * CCMoveValue * characterSpeedValue, rigid.velocity.y), GroundnormalVector);
            else if (rigid.velocity.x < -1 * maxSpeed * speedValue * CCMoveValue * characterSpeedValue) rigid.velocity = Vector3.ProjectOnPlane(new Vector2(-1 * maxSpeed * speedValue * CCMoveValue * characterSpeedValue, rigid.velocity.y), GroundnormalVector);
            rigid.velocity = Vector3.ProjectOnPlane(rigid.velocity, GroundnormalVector);                       
        }
        else {
            if (rigid.velocity.x > maxSpeed * speedValue * CCMoveValue * characterSpeedValue) rigid.velocity = new Vector2(maxSpeed * speedValue * CCMoveValue * characterSpeedValue, rigid.velocity.y);
            else if (rigid.velocity.x < -1 * maxSpeed * speedValue * CCMoveValue * characterSpeedValue) rigid.velocity = new Vector2(-1 * maxSpeed * speedValue * CCMoveValue * characterSpeedValue, rigid.velocity.y);
        }
    }

    private void Sprint()
    {
        if (isOnGround)
        {
            if (characters[playerManager.GetCharacter()].GetIsAttaking() && characters[playerManager.GetCharacter()].GetIsUsingSkill())
                speedValue = Input.GetButton("Sprint") ? 2f : 1;
            else
                speedValue = 1;
        }
        anim.SetBool("Sprint", (speedValue > 1 ? true : false));
    }

    private void Jump()
    {
        if (cc[0].cc != ClowdControl.Bound && Input.GetButton("Jump") && isOnGround && jumpKeyCheck && characters[playerManager.GetCharacter()].GetIsAttaking(true) && characters[playerManager.GetCharacter()].GetIsUsingSkill(true) && !playerManager.GetIsFlying())
        {          
            rayCaston = false;
            isOnGround = false;
            jumpKeyCheck = false;
            rigid.velocity = Vector2.right * rigid.velocity.x + GroundSpeed.y * Vector2.up * 6;
        }

        if (!Input.GetButton("Jump") || jumpT >= jumpTL)
        {
            rayCaston = true;
            jumpT = 0;
        }

        RaycastHit2D raycastHit = Physics2D.BoxCast(rigid.transform.position, new Vector3(0.7f, 1.55f, 0), 0f, Vector2.down, 1.525f, LayerMask.GetMask("Ground"));
        if (rayCaston && raycastHit.collider != null && Input.GetButtonDown("Jump")) jumpKeyCheck = true;
        float LongJumpSpeedX = (Input.GetAxisRaw("Horizontal") * maxSpeed * speedValue * 0.45f + GroundSpeed.x);
        if (!rayCaston)
        {
            float CCJumpValue = cc[0].cc == ClowdControl.Weakleg ? 1 - (cc[0].ccStrength * 0.01f) : 1;
            rigid.velocity = new Vector2(Mathf.Abs(rigid.velocity.x) < Mathf.Abs(LongJumpSpeedX) ? LongJumpSpeedX : rigid.velocity.x, 0f);
            rigid.AddForce((Vector2.up * jumpP * CCJumpValue* ((jumpT * 10) + 1f)), ForceMode2D.Impulse);
            jumpT += Time.deltaTime;
        }
    }

    private void FrameAvoid() 
    {
        if (frameAvoidCool && Input.GetButtonDown("Horizontal") && (int)Input.GetAxisRaw("Horizontal") != 0) {
            frameAvoidCool = false;
            StartCoroutine("FAEvent", (int)Input.GetAxisRaw("Horizontal"));
        }
    }

    IEnumerator FAEvent(int a) {
        yield return new WaitForFixedUpdate();
        float t = 0.1f;
        int v = 0;
        bool doubleclicked = false;
        for (t = 0.15f; t > 0; t -= Time.deltaTime) {
            if (cc[0].cc != ClowdControl.Stun && cc[3].cc == ClowdControl.None && Input.GetButtonDown("Horizontal") && (int)Input.GetAxisRaw("Horizontal") == a) {
                v = (int)Input.GetAxisRaw("Horizontal");
                doubleclicked = true;
                break;
            }
            yield return null;
        }
        if (!doubleclicked) {
            yield return new WaitForSeconds(0.15f);
            frameAvoidCool = true;
        }
        else {
            frameAvoid = true;
            //spriteRenderer.material = hitMatetrial;
            spriteRenderer.color = Color.white * 0.5f;
            rigid.drag = 0;
            rigid.velocity = new Vector2(v * 20f, rigid.velocity.y * 0.4f);
            yield return new WaitForSeconds(0.05f);
            //resetMatatrial();
            yield return new WaitForSeconds(0.12f);
            spriteRenderer.color = Color.white;
            frameAvoid = false;
            yield return new WaitForSeconds(3f);
            frameAvoidCool = true;
        }
    }

    public Animator GetAnimator() {
        return anim;
    }

    void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + Vector3.down * 0.925f, new Vector3(0.7f, 0.35f, 0));

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + Vector3.down * 1.525f, new Vector3(0.7f, 1.55f, 0));

    }
    
    public int ChangeMotionCheck(string paraName, int checkValue)
    {
        if (anim.GetInteger(paraName) != checkValue && (paraName == "Skill On" || !anim.GetBool("Skill On"))) anim.SetTrigger("Move Changed");

        return checkValue;
    }

    public float ChangeMotionCheck(string paraName, float checkValue)
    {
        if (anim.GetFloat(paraName) != checkValue && (paraName == "Skill On" || !anim.GetBool("Skill On"))) anim.SetTrigger("Move Changed");

        return checkValue;
    }

    public bool ChangeMotionCheck(string paraName, bool checkValue)
    {
        if (anim.GetBool(paraName) != checkValue && (paraName == "Skill On" || !anim.GetBool("Skill On"))) anim.SetTrigger("Move Changed");
        return checkValue;
    }

    public bool GetIsGround()
    {
        return isOnGround;
    }

    public void SetIsPause(bool p) {       
        isPause = p;     
    }

    public void ResetCCInfo()
    {
        for (int i = 0; i < cc.Length; i++) cc[i] = new CCinfo();
    }

    public void setCCInfo(CCinfo cc, Transform order)
    {
        float indomitable = 1 - (playerManager.GetEditedState("Indomitable") * 0.9f * 0.01f);
        if (cc.cc == ClowdControl.Airborne) 
        {
            characters[playerManager.GetCharacter()].StoppingCourutineWhenCCed();
            rigid.velocity = Vector2.up * cc.ccStrength * 6 * indomitable;
            this.cc[0].ccTime = cc.ccStrength * cc.ccStrength * 0.02f;
            if (this.cc[0].cc == ClowdControl.None) StartCoroutine(CCTime(0));
            this.cc[0].cc = ClowdControl.Stun;
            anim.SetBool("getCC", ChangeMotionCheck("getCC", true));
        }
        else if (cc.cc == ClowdControl.Push || cc.cc == ClowdControl.Pull)
        {
            characters[playerManager.GetCharacter()].StoppingCourutineWhenCCed();
            rigid.velocity = new Vector2((transform.position.x > order.transform.position.x ? -6 : (transform.position.x < order.transform.position.x ? 6 : (spriteRenderer.flipX ? -6 : 6))) * (cc.cc == ClowdControl.Push ? -1 : 1), 5) * cc.ccStrength * indomitable;
            this.cc[0].ccTime = cc.ccStrength * cc.ccStrength * 0.05f;
            if (this.cc[0].cc == ClowdControl.None) StartCoroutine(CCTime(0));
            this.cc[0].cc = ClowdControl.Stun;
            anim.SetBool("getCC", ChangeMotionCheck("getCC", true));
        }
        else if (cc.cc == ClowdControl.Slow || cc.cc == ClowdControl.Weakleg || cc.cc == ClowdControl.Bound || cc.cc == ClowdControl.Stun)
        {      
            if (this.cc[0].cc == ClowdControl.None)
            {
                if (cc.cc == ClowdControl.Stun)
                {
                    anim.SetBool("getCC", ChangeMotionCheck("getCC", true));
                    characters[playerManager.GetCharacter()].StoppingCourutineWhenCCed();
                }
                this.cc[0].cc = cc.cc;
                this.cc[0].ccStrength = cc.ccStrength;
                this.cc[0].ccTime = cc.ccTime * indomitable;
                StartCoroutine(CCTime(0));
                return;
            }
            if (GetCCOrder(this.cc[0].cc) > GetCCOrder(cc.cc)) return;
            
            else if (GetCCOrder(this.cc[0].cc) < GetCCOrder(cc.cc))
            {
                if (cc.cc == ClowdControl.Stun)
                {
                    characters[playerManager.GetCharacter()].StoppingCourutineWhenCCed();
                    anim.SetBool("getCC", ChangeMotionCheck("getCC", true));
                }
                this.cc[0].cc = cc.cc;
                this.cc[0].ccStrength = cc.ccStrength;
                this.cc[0].ccTime = cc.ccTime * indomitable;
            }
            else if (cc.ccStrength >= this.cc[0].ccStrength)
            {
                this.cc[0].ccStrength = cc.ccStrength;
                this.cc[0].ccTime = cc.ccTime * indomitable;               
            }
            
        }
        else if (cc.cc == ClowdControl.Blind)
        {
            this.cc[1].ccTime = cc.ccTime * indomitable;
            if (this.cc[1].cc == ClowdControl.None) StartCoroutine(CCTime(1));
            this.cc[1].cc = cc.cc;
        }
        else if (cc.cc == ClowdControl.Silance)
        {
            this.cc[2].ccTime = cc.ccTime * indomitable;
            if (this.cc[2].cc == ClowdControl.None) StartCoroutine(CCTime(2));
            this.cc[2].cc = cc.cc;
        }
        else if (cc.cc == ClowdControl.Fear || cc.cc == ClowdControl.Confusion || cc.cc == ClowdControl.Madness)
        {
            characters[playerManager.GetCharacter()].StoppingCourutineWhenCCed();
            this.cc[3].ccStrength = cc.ccStrength;
            this.cc[3].ccTime = cc.ccTime * indomitable;
            if (this.cc[3].cc == ClowdControl.None) StartCoroutine(CCTime(3));
            this.cc[3].cc = cc.cc;
        }
    }

    private IEnumerator CCTime(int index)
    {
        do
        {
            yield return null;
            cc[index].ccTime -= Time.deltaTime;
        }
        while (cc[index].ccTime > 0);
        cc[index].cc = ClowdControl.None;
        cc[index].ccStrength = 0;
        cc[index].ccTime = 0;
        anim.SetBool("getCC", ChangeMotionCheck("getCC", false));
    }

    private IEnumerator MovingWithCC()
    {
        ccMoveCheck = true;
        float t = 1;
        float CCMoveValue = cc[0].cc == ClowdControl.Slow || cc[0].cc == ClowdControl.Weakleg ? 1 - (cc[0].ccStrength * 0.01f) : 1;
        int dir = 0;
        float speed = (maxSpeed > cc[3].ccStrength ? cc[3].ccStrength : maxSpeed) * speedValue * CCMoveValue + GroundSpeed.x;
        bool flip = spriteRenderer.flipX;
        while (cc[3].cc != ClowdControl.None)
        {
            rigid.drag = 0;
            t += Time.deltaTime;
            if (t >= 1)
            {
                switch (cc[3].cc)
                {
                    case ClowdControl.Fear:
                        dir = -1;
                        break;
                    case ClowdControl.Madness:
                        dir = 1;
                        break;
                    case ClowdControl.Confusion:
                        dir = Random.Range(0, 2) == 0 ? -1 : 1;
                        break;
                }
                t = 0;
            }
            spriteRenderer.flipX = dir == -1 ? !flip : flip;
            anim.SetTrigger("Run");
            rigid.velocity = new Vector2(speed * dir * (flip ? -1 : 1), rigid.velocity.y);
            yield return null;
        }

        ccMoveCheck = false;
    }

    private int GetCCOrder(ClowdControl cc) 
    {
        switch (cc)
        {
            case ClowdControl.Slow:
                return 0;
            case ClowdControl.Weakleg:
                return 1;
            case ClowdControl.Bound:
                return 2;
            case ClowdControl.Stun:
                return 3;
        }
        return 0;
    }

    public CCinfo getCCInfo(int index)
    {
        return cc[index];
    }

    private void CharacterChange(bool isBack) {
        if (isBack) {
            int i = playerManager.GetCharacter();
            do
            {
                i = (i == 0 ? 7 : i - 1);
                if (playerManager.GetCharacterActivate(i))
                {
                    playerManager.SetChatacter(i);
                    characters[playerManager.GetCharacter()].ChildObjectSetting();
                    anim.runtimeAnimatorController = characters[playerManager.GetCharacter()].GetCharacterAnimator();
                    return;
                }
            }
            while (i != playerManager.GetCharacter());
        }
        else
        {
            int i = playerManager.GetCharacter();
            do
            {
                i = (i == 7 ? 0 : i + 1);
                if (playerManager.GetCharacterActivate(i))
                {
                    playerManager.SetChatacter(i);
                    characters[playerManager.GetCharacter()].ChildObjectSetting();
                    anim.runtimeAnimatorController = characters[playerManager.GetCharacter()].GetCharacterAnimator();
                    return;
                }
            }
            while (i != playerManager.GetCharacter());
        }
    }

    public void CameraPointSetting(bool MoveCameraPoint, Vector3 CameraSetPosition)
    {
        SetCameraTransform.position = CameraSetPosition;
        targetGroup.m_Targets[1].weight = MoveCameraPoint ? 999 : 0;
    }

    public void ChangeVelocity(Vector2 velocity) {
        if (isOnGround) rigid.velocity = Vector3.ProjectOnPlane((Vector3)velocity /*+ GroundSpeed.x * Vector3.right*/, GroundnormalVector);     
        else rigid.velocity = velocity/*+ GroundSpeed.x * Vector3.right*/;
    }
}


[System.Serializable]
public struct Skills
{   
    [Tooltip("스킬의 아이콘을 지정합니다.")]
    public Sprite Icon;
    [Tooltip("스킬의 쿨타임을 지정합니다. 궁극기는 지정 안해도 됩니다.")]
    public int skillCooltime;
    [Tooltip("스킬에서 쓰일 코스트의 종류를 지정합니다.")]
    public string costKey;
    [Tooltip("스킬의 코스트를 지정합니다.")]
    public int cost;
    [Tooltip("스킬의 이름을 지정합니다.")]
    public string skillName;
    [Tooltip("스킬을 설명합니다.")]   
    [TextArea(5, 150)]
    public string skillInfo;
};

[System.Serializable]
public struct Passive
{
    [Tooltip("패시브 아이콘을 지정합니다.")]
    public Sprite Icon;
    [Tooltip("패시브의 이름을 지정합니다.")]
    public string Name;
    [Tooltip("패시브를 설명합니다.")]
    [TextArea(5, 150)]
    public string Info;
};

public class CharacterBase : MonoBehaviour
{
    [Space(3f)]
    [Tooltip("캐릭터 코드를 지정합니다.")]
    [SerializeField] protected int characterCode;
    [Space(5f)]
    [Header("Character Infomation")]
    [Tooltip("캐릭터 이름을 저장합니다.")]
    [SerializeField] protected string characterName;
    [Tooltip("캐릭터 설명을 입력합니다.")]
    [TextArea(5, 150)]
    [SerializeField] protected string characterInfo;
    [Tooltip("캐릭터의 패시브 작동 여부를 설정합니다.")]
    [SerializeField] protected bool passiveOn = true;
    [Tooltip("캐릭터의 패시브를 작성합니다.")]
    [SerializeField] protected Passive passive;
    [Tooltip("캐릭터의 스킬을 입력합니다.\n0번 인덱스는 궁극기입니다.")]
    [SerializeField] protected Skills[] skills;
    [Space(5f)]
    [Header("Character Base Resources")]
    [Tooltip("캐릭터 전용 애니메이터를 대입합니다.")]
    [SerializeField] protected RuntimeAnimatorController anim;
    [Tooltip("캐릭터의 메인 이미지들를 지정합니다. 각 인덱스는 다음과 같습니다.\n0. 캐릭터의 메인 이미지입니다.\n1. 캐릭터의 서브 이미지입니다.\n2. 캐릭터의 메인 도트 이미지입니다.\n3. 캐릭터의 서브 도트 이미지입니다.\n4. 캐릭터를 상징하는 아이콘입니다.\n5. 얼굴 확대 이미지입니다.")]
    [SerializeField] protected Sprite[] characterImages;
    [Space(5f)]
    [Header("Character Base Value")]
    [Tooltip("캐릭터의 공격 시 움직임 가능 여부를 설정합니다.")]
    [SerializeField] protected bool CanMoveAttack = false;
    [Tooltip("기본 공격의 사이클 수를 지정합니다.")]
    [SerializeField] protected int attackCount;
    [Tooltip("한 사이클 당 캐릭터의 공격 속도를 지정합니다.")]
    [SerializeField] protected float[] attackSpeedTime;
    [Tooltip("캐릭터 충돌 판정의 크기 보정치를 지정합니다.")]
    [SerializeField] protected Vector2 sizeFixValue;
    [Tooltip("캐릭터 이동 속도 보정치를 지정합니다.")]    
    [SerializeField] protected float speedValue;
    [Space(5f)]
    [Header("Character Objects")]
    [Tooltip("캐릭터가 쓸 오브젝트를 지정합니다.")]
    [SerializeField] protected GameObject[] characterObjects;

    protected PlayerControl playerControl;
    protected PlayerManager playerManager;
    protected ParticularEvent particular;
    protected Animator playerAnimation;
    protected Rigidbody2D playerRigid;
    protected Transform PlayerOBJ;
    IEnumerator courutine;
    protected IEnumerator[] skillCourutine;
    protected bool attackCooling = true;
    protected bool SkillCooling = true;
    protected bool canAttack = true;
    protected bool usingCanMoveSkill = false;
    protected float[,] skillCoolingInfo = { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } };
    protected bool[] skillDoingInfo = { false, false, false, false, false, false, false, false, false, false, false, false, false };
    protected bool[] skillcoolWhenCCed = { true, true, true, true, true, true, true, true, true, true, true, true, true };
    protected bool isArmorOn = true;
    protected bool canFlip = true;
    protected bool isMovingBySkill = false;
    protected float skillRank;

    public void setPlayerControl(PlayerControl pc)
    {
        playerControl = pc;
        playerAnimation = pc.GetComponent<Animator>();
        playerRigid = pc.GetComponent<Rigidbody2D>();
        playerManager = transform.parent.GetComponent<PlayerManager>();
        particular = playerManager.GetComponent<ParticularEvent>();
    }

    [ContextMenu("Send String to LanguageManager")]
    public virtual void SaveString()
    {
        List<string> list = new List<string>() { characterName, characterInfo, passive.Name, passive.Info };
        foreach (Skills i in skills)
        {
            list.Add(i.skillName);
            list.Add(i.skillInfo);
        }
        GameObject.Find("GameManager").GetComponent<LanguageManager>().SetTextList(("Playable " + characterCode + " string value"), list);
    }

    [ContextMenu("Get String to LanguageManager")]
    public virtual void LoadString()
    {
        List<string> list = GameObject.Find("GameManager").GetComponent<LanguageManager>().GetTextList(("Playable " + characterCode + " string value"));
        characterName = list[0];
        characterInfo = list[1];
        passive.Name = list[2];
        passive.Info = list[3];
        int j = 0;
        for (int i = 4; i < list.Count; i += 2)
        {
            skills[j].skillName = list[i];
            skills[j].skillInfo = list[i + 1];
            j++;
        }
    }


    public virtual RuntimeAnimatorController GetCharacterAnimator()
    {
        return anim;
    }

    public virtual Sprite GetCharacterImagewithIndex(int index)
    {
        return characterImages[index];
    }

    public virtual Passive GetPassive()
    {
        return passive;
    }

    public virtual Skills GetSkillwithIndex(int index)
    {
        return skills[index];
    }

    public virtual string[] GetCharacterInfomation()
    {
        return new string[] { characterName, characterInfo };
    }

    public virtual Vector2 GetSizeValue()
    {
        return sizeFixValue;
    }

    public virtual bool GetIsAttaking(bool checkCanMove = false)
    {
        return attackCooling || (checkCanMove ? CanMoveAttack : false);
    }

    public virtual bool GetIsUsingSkill(bool checkMoveSkill = false)
    {
        return SkillCooling || (checkMoveSkill ? usingCanMoveSkill : false);
    }

    public virtual bool GetCanFlip()
    {
        return canFlip;
    }

    public virtual void Attack()
    {
        if (Input.GetButton("Attack") && attackCooling && canAttack && SkillCooling && !playerManager.GetIsUsingMoveSkill())
        {
            playerRigid.drag = 0;
            attackCooling = false;
            courutine = Attacks();
            StartCoroutine(courutine);
        }
    }

    public virtual void UsingSkills()
    {
        int skillcode = 0;
        if (Input.GetButtonDown("Skill1"))
        {
            skillcode = playerManager.GetSettingSkillCode(characterCode, 0);
        }
        if (Input.GetButtonDown("Skill2"))
        {
            skillcode = playerManager.GetSettingSkillCode(characterCode, 1);
        }
        if (Input.GetButtonDown("Skill3"))
        {
            skillcode = playerManager.GetSettingSkillCode(characterCode, 2);
        }

        if (skillcode != 0 && canAttack)
        {
            courutine = AttackSkillUsing(skillcode);
            StartCoroutine(courutine);
        }

        if (Input.GetKeyDown(KeyCode.R))
        { 
            skillCourutine[0] = UltimateSkill();
            StartCoroutine(skillCourutine[0]);
        }

    }

    public virtual void StoppingCourutineWhenCCed()
    {
        playerManager.StopCourutineWhenCCed();
        if (courutine != null)
            StopCoroutine(courutine);
        foreach (IEnumerator e in skillCourutine) if(e != null) StopCoroutine(e);
        playerAnimation.SetBool("Attack", playerControl.ChangeMotionCheck("Attack", false));
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", false));
        foreach (IEnumerator j in skillCourutine) 
        for (int i = 0; i < 13; i++)
        {
            if (skillDoingInfo[i] && skillcoolWhenCCed[i]) StartCoroutine(SkillCooltime(i, skills[i].skillCooltime));
            skillDoingInfo[i] = false;
        }
        playerRigid.gravityScale = 6;
        ChildObjectSetDisactive();
        attackCooling = true;
        SkillCooling = true;
        usingCanMoveSkill = false;
    }

    public virtual void ResetCooltime()
    {
        for (int i = 0; i < 13; i++) skillCoolingInfo[i, 0] = 0;
    }

    protected virtual void ChildObjectSetDisactive() {
        for (int i = 0; i < playerControl.transform.GetChild(0).childCount-1; i++) {
            playerControl.transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
        }
    }

    protected virtual IEnumerator Attacks()
    {
        yield return null;
    }

    protected virtual IEnumerator AttackSkillUsing(int code)
    {
        yield return null;
    }

    protected virtual IEnumerator UltimateSkill()
    {
        yield return null;
    }

    public virtual void ChildObjectSetting()
    {
        Transform[] childlist = playerControl.gameObject.GetComponentsInChildren<Transform>();
        if (childlist != null) {
            foreach (Transform t in childlist) {
                if (t != playerControl.transform && t.tag != "NotChild") Destroy(t.gameObject);
            }
        }
    }

    public virtual bool CostCheckForUsingSkill(string costKey, int cost)
    {
        return playerManager.GetState(costKey) >= cost;
    }

    public virtual float GetSkillCooltime(int skillCode, int firstornow)
    {
        return skillCoolingInfo[skillCode, firstornow];
    }

    public virtual bool GetisSkillDoing(int skillCode)
    {
        return skillDoingInfo[skillCode];
    }

    public virtual bool GetIsArmorSetting() {
        return isArmorOn;
    }

    public virtual float ReturnSpeedValue() {
        return speedValue;
    }

    protected virtual IEnumerator SkillCooltime(int skillcode, float time)
    {
        if (skillCoolingInfo[skillcode, 1] < time) skillCoolingInfo[skillcode, 1] = time;
        skillCoolingInfo[skillcode, 0] = time;
        do
        {
            yield return null;
            skillCoolingInfo[skillcode, 0] -= Time.deltaTime;
        }
        while (skillCoolingInfo[skillcode, 0] > 0);
        skillCoolingInfo[skillcode, 0] = 0;
        skillCoolingInfo[skillcode, 1] = 0;
    }

    public virtual bool GetisMovingBySkill() {
        return isMovingBySkill;
    }
}