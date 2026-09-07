using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SkillIcons //행에 해당되는 이름
{
    public Sprite[] AttackSkills;
}

public class PlayerCharactor_Proto : MonoBehaviour
{
    PlayerState_Proto ps;
    public GameObject text;
    public Image MoveSkillIcon1;
    public Image MoveSkillIcon2;
    public Image AttackSkillIcon1;
    public Image AttackSkillIcon2;

    public bool canMoveSkill1 = true;
    public bool canMoveSkill2 = true;
    public bool MoveSkill1Act = false;
    public bool MoveSkill2Act = false;

    public bool canAttackSkill1 = true;
    public bool canAttackSkill2 = true;
    public bool AttackSkill1Act = false;
    public bool AttackSkill2Act = false;

    public bool skillonbutcanAct = false;
    public bool skillonbutcantMove = false;

    public Sprite DefaultSkillSprite;
    public Sprite[] MoveSkills;
    public SkillIcons[] AttackSkills;

    public GameObject[] skillOBJ;

    public Image tokenImg;
    public Image maxTokenImg;

    // Start is called before the first frame update
    void Start()
    {
        ps = GameObject.Find("GameManager").GetComponent<PlayerState_Proto>();
    }

    // Update is called once per frame
    void Update()
    {
        tokenImg.fillAmount = ps.CardToken * 0.1f;
        maxTokenImg.fillAmount = ps.MaxCardToken * 0.1f;

        if (ps.MoveSkill1 == -1)
            MoveSkillIcon1.sprite = DefaultSkillSprite;
        else
            MoveSkillIcon1.sprite = MoveSkills[ps.MoveSkill1];

        if (ps.MoveSkill2 == -1)
            MoveSkillIcon2.sprite = DefaultSkillSprite;
        else
            MoveSkillIcon2.sprite = MoveSkills[ps.MoveSkill2];

        if (ps.AttackSkill1[ps.getCharactorKey()] == -1)
            AttackSkillIcon1.sprite = DefaultSkillSprite;
        else
            AttackSkillIcon1.sprite = AttackSkills[ps.getCharactorKey()].AttackSkills[ps.AttackSkill1[ps.getCharactorKey()]];
        
        if (ps.AttackSkill2[ps.getCharactorKey()] == -1)
            AttackSkillIcon2.sprite = DefaultSkillSprite;
        else
            AttackSkillIcon2.sprite = AttackSkills[ps.getCharactorKey()].AttackSkills[ps.AttackSkill2[ps.getCharactorKey()]];

        if (MoveSkill1Act)
        {
            MoveSkillIcon1.color = new Color(1, 1, 0.5f);
        }
        else if (!canMoveSkill1)
        {
            MoveSkillIcon1.color = new Color(0.5f, 0.5f, 0.5f);
        }
        else
        {
            MoveSkillIcon1.color = new Color(1, 1, 1);
        }

        if (MoveSkill2Act)
        {
            MoveSkillIcon2.color = new Color(1, 1, 0.5f);
        }
        else if (!canMoveSkill2)
        {
            MoveSkillIcon2.color = new Color(0.5f, 0.5f, 0.5f);
        }
        else
        {
            MoveSkillIcon2.color = new Color(1, 1, 1);
        }

        if (AttackSkill1Act)
        {
            AttackSkillIcon1.color = new Color(1, 1, 0.5f);
        }
        else if (!canAttackSkill1)
        {
            AttackSkillIcon1.color = new Color(0.5f, 0.5f, 0.5f);
        }
        else
        {
            AttackSkillIcon1.color = new Color(1, 1, 1);
        }

        if (AttackSkill2Act)
        {
            AttackSkillIcon2.color = new Color(1, 1, 0.5f);
        }
        else if (!canAttackSkill2)
        {
            AttackSkillIcon2.color = new Color(0.5f, 0.5f, 0.5f);
        }
        else
        {
            AttackSkillIcon2.color = new Color(1, 1, 1);
        }
    }

    public void usingSkill(int skillcode, int code)
    {
        if ((canAttackSkill1 && code == 1) || (canAttackSkill2 && code == 2))
        {
            if (code == 1)
                canAttackSkill1 = false;
            else if (code == 2)
                canAttackSkill2 = false;

            switch (ps.getCharactorKey())
            {
                case 0:
                    switch (skillcode)
                    {
                        case 0:
                            if (ps.MP > 10)
                            {
                                ps.MP -= 10;
                                StartCoroutine(twoCut(code));
                            }
                            else
                            {
                                if (code == 1)
                                    canAttackSkill1 = true;
                                else
                                    canAttackSkill2 = true;
                            }
                            break;
                        case 1:
                            if (ps.MP > 15)
                            {
                                ps.MP -= 15;
                                StartCoroutine(UpperHit(code));
                            }
                            else
                            {
                                if (code == 1)
                                    canAttackSkill1 = true;
                                else
                                    canAttackSkill2 = true;
                            }
                            break;
                        case 2:
                            if (ps.MP > 30)
                            {
                                ps.MP -= 30;
                                StartCoroutine(ToupAndSlash(code));
                            }
                            else
                            {
                                if (code == 1)
                                    canAttackSkill1 = true;
                                else
                                    canAttackSkill2 = true;
                            }
                            break;
                    }
                    break;
                case 1:
                    switch (skillcode)
                    {
                        case 0:
                            if (ps.MP > 50)
                            {
                                ps.MP -= 50;
                                StartCoroutine(Lightning(code));
                            }
                            else
                            {
                                if (code == 1)
                                    canAttackSkill1 = true;
                                else
                                    canAttackSkill2 = true;
                            }                            
                            break;
                        case 1:
                            if (ps.MP > 65)
                            {
                                ps.MP -= 65;
                                StartCoroutine(Beam(code));
                            }
                            else
                            {
                                if (code == 1)
                                    canAttackSkill1 = true;
                                else
                                    canAttackSkill2 = true;
                            }
                            break;
                        case 2:
                            if (ps.MP > 30)
                            {
                                ps.MP -= 30;
                                StartCoroutine(Barrior(code));
                            }
                            else
                            {
                                if (code == 1)
                                    canAttackSkill1 = true;
                                else
                                    canAttackSkill2 = true;
                            }
                            break;
                    }
                    break;
                case 2:
                    switch (skillcode)
                    {
                        case 0:
                            if (ps.MP > 55)
                            {
                                ps.MP -= 55;
                                StartCoroutine(SplitShot(code));
                            }
                            else
                            {
                                if (code == 1)
                                    canAttackSkill1 = true;
                                else
                                    canAttackSkill2 = true;
                            }
                            break;
                        case 1:
                            if (ps.MP > 35)
                            {
                                ps.MP -= 35;
                                StartCoroutine(SnipeShot(code));
                            }
                            else
                            {
                                if (code == 1)
                                    canAttackSkill1 = true;
                                else
                                    canAttackSkill2 = true;
                            }
                            break;
                        case 2:
                            if (ps.MP > 60)
                            {
                                ps.MP -= 60;
                                StartCoroutine(boomShot(code));
                            }
                            else
                            {
                                if (code == 1)
                                    canAttackSkill1 = true;
                                else
                                    canAttackSkill2 = true;
                            }
                            break;
                    }
                    break;
                case 3:
                    switch (skillcode)
                    {
                        case 0:
                            if (ps.MP > 45)
                            {
                                ps.MP -= 45;
                                StartCoroutine(SideSpear(code));
                            }
                            else
                            {
                                if (code == 1)
                                    canAttackSkill1 = true;
                                else
                                    canAttackSkill2 = true;
                            }
                            break;
                        case 1:
                            if (ps.MP > 55)
                            {
                                ps.MP -= 55;
                                StartCoroutine(UpSpear(code));
                            }
                            else
                            {
                                if (code == 1)
                                    canAttackSkill1 = true;
                                else
                                    canAttackSkill2 = true;
                            }
                            break;
                        case 2:
                            if (ps.MP > 30)
                            {
                                if (!((Physics2D.Raycast(new Vector2(GetComponent<Rigidbody2D>().position.x - 0.3f, GetComponent<Rigidbody2D>().position.y), Vector2.down, 0.7f, 1 << LayerMask.NameToLayer("Ground")).collider != null) || (Physics2D.Raycast(new Vector2(GetComponent<Rigidbody2D>().position.x + 0.3f, GetComponent<Rigidbody2D>().position.y), Vector2.down, 0.7f, 1 << LayerMask.NameToLayer("Ground")).collider != null)))
                                {
                                    ps.MP -= 30;
                                    StartCoroutine(DownSpear(code));
                                }
                                else
                                {
                                    TextMesh tm = Instantiate(text, new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, -0.5f), transform.rotation).GetComponent<TextMesh>();
                                    tm.text = "공중에 떠야 합니다.";
                                    if (code == 1)
                                        canAttackSkill1 = true;
                                    else
                                        canAttackSkill2 = true;
                                }
                            }
                            else
                            {
                                if (code == 1)
                                    canAttackSkill1 = true;
                                else
                                    canAttackSkill2 = true;
                            }
                            break;
                    }
                    break;
                case 4:
                    switch (skillcode)
                    {
                        case 0:
                            if (ps.MP > 55)
                            {
                                ps.MP -= 55;
                                StartCoroutine(DownAxeKick(code));
                            }
                            else
                            {
                                if (code == 1)
                                    canAttackSkill1 = true;
                                else
                                    canAttackSkill2 = true;
                            }
                            break;
                        case 1:
                            if (ps.MP > 25)
                            {
                                ps.MP -= 25;
                                StartCoroutine(AxeSideCut(code));
                            }
                            else
                            {
                                if (code == 1)
                                    canAttackSkill1 = true;
                                else
                                    canAttackSkill2 = true;
                            }
                            break;
                        case 2:
                            if (ps.MP > 35)
                            {
                                ps.MP -= 35;
                                StartCoroutine(AxeSlashbeem(code));
                            }
                            else
                            {
                                if (code == 1)
                                    canAttackSkill1 = true;
                                else
                                    canAttackSkill2 = true;
                            }
                            break;
                    }
                    break;
                case 5:
                    switch (skillcode)
                    {
                        case 0:
                            if (ps.MP > 30)
                            {
                                ps.MP -= 30;
                                StartCoroutine(ClawBig(code));
                            }
                            else
                            {
                                if (code == 1)
                                    canAttackSkill1 = true;
                                else
                                    canAttackSkill2 = true;
                            }
                            break;
                        case 1:
                            if (ps.MP > 60)
                            {
                                ps.MP -= 60;
                                StartCoroutine(ClawUpper(code));
                            }
                            else
                            {
                                if (code == 1)
                                    canAttackSkill1 = true;
                                else
                                    canAttackSkill2 = true;
                            }
                            break;
                        case 2:
                            if (ps.MP > 55)
                            {
                                ps.MP -= 55;
                                StartCoroutine(ClawFast(code));
                            }
                            else
                            {
                                if (code == 1)
                                    canAttackSkill1 = true;
                                else
                                    canAttackSkill2 = true;
                            }
                            break;
                    }
                    break;
                case 6:
                    switch (skillcode)
                    {
                        case 0:
                            if (ps.MP > 60)
                            {
                                ps.MP -= 60;
                                StartCoroutine(FastPunch(code));
                            }
                            else
                            {
                                if (code == 1)
                                    canAttackSkill1 = true;
                                else
                                    canAttackSkill2 = true;
                            }
                            break;
                        case 1:
                            if (ps.MP > 45)
                            {
                                ps.MP -= 45;
                                StartCoroutine(PushPunch(code));
                            }
                            else
                            {
                                if (code == 1)
                                    canAttackSkill1 = true;
                                else
                                    canAttackSkill2 = true;
                            }
                            break;
                        case 2:
                            if (ps.MP > 40)
                            {
                                if (!((Physics2D.Raycast(new Vector2(GetComponent<Rigidbody2D>().position.x - 0.3f, GetComponent<Rigidbody2D>().position.y), Vector2.down, 0.7f, 1 << LayerMask.NameToLayer("Ground")).collider != null) || (Physics2D.Raycast(new Vector2(GetComponent<Rigidbody2D>().position.x + 0.3f, GetComponent<Rigidbody2D>().position.y), Vector2.down, 0.7f, 1 << LayerMask.NameToLayer("Ground")).collider != null)))
                                {
                                    ps.MP -= 40;
                                    StartCoroutine(DownPunch(code));
                                }
                                else
                                {
                                    TextMesh tm = Instantiate(text, new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, -0.5f), transform.rotation).GetComponent<TextMesh>();
                                    tm.text = "공중에 떠야 합니다.";
                                    if (code == 1)
                                        canAttackSkill1 = true;
                                    else
                                        canAttackSkill2 = true;
                                }
                            }
                            else
                            {
                                if (code == 1)
                                    canAttackSkill1 = true;
                                else
                                    canAttackSkill2 = true;
                            }
                            break;
                    }
                    break;
                case 7:
                    switch (skillcode)
                    {
                        case 0:
                            if (ps.MP > 10)
                            {                                
                                if (ps.HP > ps.MaxHP * 0.15f)
                                {
                                    ps.HP -= ps.MaxHP * 0.15f;
                                    ps.MP -= 10;
                                    StartCoroutine(BackAttack(code));
                                }
                                else
                                {
                                    if (code == 1)
                                        canAttackSkill1 = true;
                                    else
                                        canAttackSkill2 = true;
                                }
                            }
                            else
                            {
                                if (code == 1)
                                    canAttackSkill1 = true;
                                else
                                    canAttackSkill2 = true;
                            }
                            break;
                        case 1:
                            if (ps.MP > 5)
                            {                                
                                if (ps.HP > ps.MaxHP * 0.15f)
                                {
                                    ps.HP -= ps.MaxHP * 0.15f;
                                    ps.MP -= 5;
                                    StartCoroutine(SplashAttack(code));
                                }
                                else
                                {
                                    if (code == 1)
                                        canAttackSkill1 = true;
                                    else
                                        canAttackSkill2 = true;
                                }
                            }
                            else
                            {
                                if (code == 1)
                                    canAttackSkill1 = true;
                                else
                                    canAttackSkill2 = true;
                            }
                            break;
                        case 2:
                            if (ps.MP > 10)
                            {
                                if (ps.HP > ps.MaxHP * 0.35f)
                                {
                                    ps.MP -= 10;
                                    ps.HP -= ps.MaxHP * 0.35f;
                                    StartCoroutine(Zeolite(code));
                                }
                                else
                                {
                                    if (code == 1)
                                        canAttackSkill1 = true;
                                    else
                                        canAttackSkill2 = true;
                                }
                            }
                            else
                            {
                                if (code == 1)
                                    canAttackSkill1 = true;
                                else
                                    canAttackSkill2 = true;
                            }
                            break;
                    }
                    break;
            }
        }
    }

    // 검 스킬
    IEnumerator twoCut(int code)
    {
        if (code == 1)
            AttackSkill1Act = true;
        else if(code == 2)
            AttackSkill2Act = true;

        if (GetComponent<SpriteRenderer>().flipX)
            Instantiate(skillOBJ[0], transform.position + Vector3.left, transform.rotation, transform);
        else
            Instantiate(skillOBJ[0], transform.position + Vector3.right, transform.rotation, transform);
        yield return new WaitForSeconds(0.1f);
        if (GetComponent<SpriteRenderer>().flipX)
            Instantiate(skillOBJ[1], transform.position + Vector3.left, transform.rotation, transform);
        else
            Instantiate(skillOBJ[1], transform.position + Vector3.right, transform.rotation, transform);
        yield return new WaitForSeconds(0.1f);

        if (code == 1)
            AttackSkill1Act = false;
        else if(code == 2)
            AttackSkill2Act = false;
        StartCoroutine(skillCooltime(code, 4));
    }
    IEnumerator UpperHit(int code)
    {
        if (code == 1)
            AttackSkill1Act = true;
        else if (code == 2)
            AttackSkill2Act = true;

        if (GetComponent<SpriteRenderer>().flipX)
            Instantiate(skillOBJ[2], transform.position + Vector3.left * 1.5f, transform.rotation, transform);
        else
            Instantiate(skillOBJ[2], transform.position + Vector3.right * 1.5f, transform.rotation, transform);
        yield return new WaitForSeconds(0.3f);

        if (code == 1)
            AttackSkill1Act = false;
        else if (code == 2)
            AttackSkill2Act = false;
        StartCoroutine(skillCooltime(code, 5));
    }
    IEnumerator ToupAndSlash(int code)
    {
        if (code == 1)
            AttackSkill1Act = true;
        else if (code == 2)
            AttackSkill2Act = true;

        GetComponent<Rigidbody2D>().velocity = Vector2.up * 20;
        Instantiate(skillOBJ[3], transform.position + Vector3.up * 0.65f, transform.rotation);
        yield return new WaitForSeconds(0.1f);
        Instantiate(skillOBJ[3], transform.position + Vector3.up * 0.65f, transform.rotation);
        yield return new WaitForSeconds(0.1f);
        Instantiate(skillOBJ[3], transform.position + Vector3.up * 0.65f, transform.rotation);
        yield return new WaitForSeconds(0.1f);
        Instantiate(skillOBJ[3], transform.position + Vector3.up * 0.65f, transform.rotation);
        yield return new WaitForSeconds(0.1f);

        if (code == 1)
            AttackSkill1Act = false;
        else if (code == 2)
            AttackSkill2Act = false;
        StartCoroutine(skillCooltime(code, 7));
    }
    // 마법 스킬

    IEnumerator Lightning(int code)
    {

        Vector3 point = transform.position + Vector3.down + Vector3.right * 5;
        Vector3 point2 = transform.position + Vector3.down + Vector3.left * 5;
        bool a = GetComponent<SpriteRenderer>().flipX;
        if (!a)
            Instantiate(skillOBJ[4], point + Vector3.up * 1.5f, transform.rotation);
        else
            Instantiate(skillOBJ[4], point2 + Vector3.up * 1.5f, transform.rotation);

        yield return new WaitForSeconds(1f);

        if (!a)
            Instantiate(skillOBJ[5], point + Vector3.up * 5.2f, transform.rotation);
        else
            Instantiate(skillOBJ[5], point2 + Vector3.up * 5.2f, transform.rotation);

        StartCoroutine(skillCooltime(code, 7));
    }
    IEnumerator Beam(int code)
    {
        GameObject beam;
        skillonbutcanAct = true;
        if (GetComponent<SpriteRenderer>().flipX)
        {
            beam = Instantiate(skillOBJ[6], transform.position + Vector3.left * 6, transform.rotation, transform);
            beam.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
            beam = Instantiate(skillOBJ[6], transform.position + Vector3.right * 6, transform.rotation, transform);
        for (int i = 0; i < 6; i++)
        {
            beam.SetActive(true);
            yield return new WaitForSeconds(0.333f);
            beam.SetActive(false);
            yield return new WaitForSeconds(0.02f);
            beam.SetActive(true);
        }
        Destroy(beam);
        skillonbutcanAct = false;
        StartCoroutine(skillCooltime(code, 12));
    }
    IEnumerator Barrior(int code)
    {
        GameObject barrior;
        float a = (ps.intellect * 0.5f) + 10;
        ps.barrior += a;
        barrior = Instantiate(skillOBJ[7], transform.position, transform.rotation, transform);
        yield return new WaitForSeconds(5f);
        if (ps.barrior < a)
            ps.barrior = 0;
        else
            ps.barrior -= ps.getMaxHP() * 1.25f;
        Destroy(barrior);

        StartCoroutine(skillCooltime(code, 5));
    }
    // 총 스킬

    IEnumerator SplitShot(int code)
    {
        float randomangle;
        if (code == 1)
            AttackSkill1Act = true;
        else if (code == 2)
            AttackSkill2Act = true;
        GameObject shot;
        for (int i = 0; i < 15; i++)
        {
            randomangle = Random.Range(0f, 20f);
            shot = Instantiate(skillOBJ[8], transform.position + new Vector3(Mathf.Cos(randomangle * Mathf.PI / 180), Mathf.Sin(randomangle * Mathf.PI / 180)).normalized * 1.2f, transform.rotation);
            shot.GetComponent<shotWantVector>().angle = randomangle;
            randomangle = Random.Range(135f, 180f);
            shot = Instantiate(skillOBJ[8], transform.position + new Vector3(Mathf.Cos(randomangle * Mathf.PI / 180), Mathf.Sin(randomangle * Mathf.PI / 180)).normalized * 1.2f, transform.rotation);            
            shot.GetComponent<shotWantVector>().angle = randomangle;
            randomangle = Random.Range(0f, 35f);
            shot = Instantiate(skillOBJ[8], transform.position + new Vector3(Mathf.Cos(randomangle * Mathf.PI / 180), Mathf.Sin(randomangle * Mathf.PI / 180)).normalized * 1.2f, transform.rotation);
            shot.GetComponent<shotWantVector>().angle = randomangle;
            randomangle = Random.Range(160f, 180f);
            shot = Instantiate(skillOBJ[8], transform.position + new Vector3(Mathf.Cos(randomangle * Mathf.PI / 180), Mathf.Sin(randomangle * Mathf.PI / 180)).normalized * 1.2f, transform.rotation);
            shot.GetComponent<shotWantVector>().angle = randomangle;
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(0.3f);
        shot = Instantiate(skillOBJ[8], transform.position + new Vector3(1, 0).normalized * 1.2f, transform.rotation);
        shot.GetComponent<shotWantVector>().angle = 0;
        shot = Instantiate(skillOBJ[8], transform.position + new Vector3(1, 1).normalized * 1.2f, transform.rotation);
        shot.GetComponent<shotWantVector>().angle = 45;
        shot = Instantiate(skillOBJ[8], transform.position + new Vector3(0, 1).normalized * 1.2f, transform.rotation);
        shot.GetComponent<shotWantVector>().angle = 90;
        shot = Instantiate(skillOBJ[8], transform.position + new Vector3(-1, 1).normalized * 1.2f, transform.rotation);
        shot.GetComponent<shotWantVector>().angle = 135;
        shot = Instantiate(skillOBJ[8], transform.position + new Vector3(-1, 0).normalized * 1.2f, transform.rotation);
        shot.GetComponent<shotWantVector>().angle = 180;
        yield return new WaitForSeconds(0.5f);
        if (code == 1)
            AttackSkill1Act = false;
        else if (code == 2)
            AttackSkill2Act = false;

        StartCoroutine(skillCooltime(code, 17));
    }
    IEnumerator SnipeShot(int code)
    {
        if (code == 1)
            AttackSkill1Act = true;
        else if (code == 2)
            AttackSkill2Act = true;
        GameObject shot;
        skillonbutcantMove = true;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);
        yield return new WaitForSeconds(0.2f);
        
        if (GetComponent<SpriteRenderer>().flipX)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(10, 15);
            shot = Instantiate(skillOBJ[9], transform.position + new Vector3(-1, 0).normalized * 1.2f, transform.rotation);
            shot.GetComponent<shotWantVector>().angle = 180;
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(-10, 15);
            shot = Instantiate(skillOBJ[9], transform.position + new Vector3(1, 0).normalized * 1.2f, transform.rotation);
            shot.GetComponent<shotWantVector>().angle = 0;
        }

        yield return new WaitForSeconds(0.3f);
        skillonbutcantMove = false;
        if (code == 1)
            AttackSkill1Act = false;
        else if (code == 2)
            AttackSkill2Act = false;

        StartCoroutine(skillCooltime(code, 5));
    }
    IEnumerator boomShot(int code)
    {
        if (code == 1)
            AttackSkill1Act = true;
        else if (code == 2)
            AttackSkill2Act = true;
        GameObject shot;
        if (GetComponent<SpriteRenderer>().flipX)
        {
            shot = Instantiate(skillOBJ[10], transform.position + new Vector3(-1, 0).normalized * 1.2f, transform.rotation);
            shot.GetComponent<shotWantVector>().angle = 180;
        }
        else
        {
            shot = Instantiate(skillOBJ[10], transform.position + new Vector3(1, 0).normalized * 1.2f, transform.rotation);
            shot.GetComponent<shotWantVector>().angle = 0;
        }

        yield return new WaitForSeconds(0.3f);
        skillonbutcantMove = false;
        if (code == 1)
            AttackSkill1Act = false;
        else if (code == 2)
            AttackSkill2Act = false;

        StartCoroutine(skillCooltime(code, 15));
    }
    // 창 스킬

    IEnumerator SideSpear(int code)
    {
        if (code == 1)
            AttackSkill1Act = true;
        else if (code == 2)
            AttackSkill2Act = true;
        skillonbutcantMove = true;
        GameObject g;
        GetComponent<Rigidbody2D>().gravityScale = 0;
        if (!GetComponent<SpriteRenderer>().flipX)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.right * 15;
            g = Instantiate(skillOBJ[11], transform.position + Vector3.up * 0 + Vector3.right * 1.402734f, skillOBJ[11].transform.rotation, transform);
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.left * 25;
            g = Instantiate(skillOBJ[11], transform.position + Vector3.up * 0 + Vector3.left * 1.402734f, skillOBJ[11].transform.rotation, transform);
        }
        g.GetComponent<SpriteRenderer>().flipX = GetComponent<SpriteRenderer>().flipX;
        yield return StartCoroutine(GetComponent<PlayerControl_Proto>().count(0.3f));
        GetComponent<Rigidbody2D>().gravityScale = 5;
        Destroy(g);
        skillonbutcantMove = false;
        if (code == 1)
            AttackSkill1Act = false;
        else if (code == 2)
            AttackSkill2Act = false;

        StartCoroutine(skillCooltime(code, 6));
    }
    IEnumerator UpSpear(int code)
    {
        if (code == 1)
            AttackSkill1Act = true;
        else if (code == 2)
            AttackSkill2Act = true;
        skillonbutcantMove = true;
        GameObject g;
        GetComponent<Rigidbody2D>().velocity = Vector2.up * 25;
        if (!GetComponent<SpriteRenderer>().flipX)
            g = Instantiate(skillOBJ[12], transform.position + Vector3.up * -0.12f + Vector3.right * 0.5079999f, skillOBJ[12].transform.rotation, transform);
        else
            g = Instantiate(skillOBJ[12], transform.position + Vector3.up * -0.12f + Vector3.left * 0.5079999f, skillOBJ[12].transform.rotation, transform);
        yield return new WaitForSeconds(0.3f);
        skillonbutcantMove = false;
        Destroy(g);
        if (code == 1)
            AttackSkill1Act = false;
        else if (code == 2)
            AttackSkill2Act = false;

        StartCoroutine(skillCooltime(code, 6));
    }
    IEnumerator DownSpear(int code)
    {
        if (code == 1)
            AttackSkill1Act = true;
        else if (code == 2)
            AttackSkill2Act = true;
        skillonbutcantMove = true;
        GameObject g;
        GetComponent<Rigidbody2D>().velocity = Vector2.down * 30;
        if (GetComponent<SpriteRenderer>().flipX)
        {
            g = Instantiate(skillOBJ[13], transform.position + Vector3.up * 0.488f + Vector3.right * -0.284f, skillOBJ[13].transform.rotation, transform);
            yield return new WaitUntil(() =>
        (Physics2D.Raycast(new Vector2(GetComponent<Rigidbody2D>().position.x - 0.3f, GetComponent<Rigidbody2D>().position.y), Vector2.down, 0.7f, 1 << LayerMask.NameToLayer("Ground")).collider != null) ||
        (Physics2D.Raycast(new Vector2(GetComponent<Rigidbody2D>().position.x + 0.3f, GetComponent<Rigidbody2D>().position.y), Vector2.down, 0.7f, 1 << LayerMask.NameToLayer("Ground")).collider != null));
            Instantiate(skillOBJ[14], transform.position + Vector3.up * -0.49f + Vector3.right * -0.284f, skillOBJ[14].transform.rotation, transform);
        }
        else
        {
            g = Instantiate(skillOBJ[13], transform.position + Vector3.up * 0.488f + Vector3.left * -0.284f, skillOBJ[13].transform.rotation, transform);
            yield return new WaitUntil(() =>
        (Physics2D.Raycast(new Vector2(GetComponent<Rigidbody2D>().position.x - 0.3f, GetComponent<Rigidbody2D>().position.y), Vector2.down, 0.7f, 1 << LayerMask.NameToLayer("Ground")).collider != null) ||
        (Physics2D.Raycast(new Vector2(GetComponent<Rigidbody2D>().position.x + 0.3f, GetComponent<Rigidbody2D>().position.y), Vector2.down, 0.7f, 1 << LayerMask.NameToLayer("Ground")).collider != null));
            Instantiate(skillOBJ[14], transform.position + Vector3.up * -0.49f + Vector3.left * -0.284f, skillOBJ[14].transform.rotation, transform);
        }
        skillonbutcantMove = false;
        Destroy(g);
        if (code == 1)
            AttackSkill1Act = false;
        else if (code == 2)
            AttackSkill2Act = false;

        StartCoroutine(skillCooltime(code, 10));
    }
    // 도끼 스킬

    IEnumerator DownAxeKick(int code)
    {
        if (code == 1)
            AttackSkill1Act = true;
        else if (code == 2)
            AttackSkill2Act = true;

        if (GetComponent<SpriteRenderer>().flipX)
            Instantiate(skillOBJ[16], transform.position + Vector3.left * 1.5f, transform.rotation, transform);
        else
            Instantiate(skillOBJ[16], transform.position + Vector3.right * 1.5f, transform.rotation, transform);
        yield return new WaitForSeconds(0.2f);

        if((Physics2D.Raycast(new Vector2(GetComponent<Rigidbody2D>().position.x - 0.3f, GetComponent<Rigidbody2D>().position.y), Vector2.down, 0.7f, 1 << LayerMask.NameToLayer("Ground")).collider != null) ||
        (Physics2D.Raycast(new Vector2(GetComponent<Rigidbody2D>().position.x + 0.3f, GetComponent<Rigidbody2D>().position.y), Vector2.down, 0.7f, 1 << LayerMask.NameToLayer("Ground")).collider != null))
            Instantiate(skillOBJ[14], transform.position + Vector3.up * -0.49f, skillOBJ[14].transform.rotation, transform);

        if (code == 1)
            AttackSkill1Act = false;
        else if (code == 2)
            AttackSkill2Act = false;
        StartCoroutine(skillCooltime(code, 7));
    }
    IEnumerator AxeSideCut(int code)
    {
        if (code == 1)
            AttackSkill1Act = true;
        else if (code == 2)
            AttackSkill2Act = true;

        Instantiate(skillOBJ[15], transform.position + Vector3.up * 0.65f, transform.rotation);
        yield return new WaitForSeconds(0.1f);

        if (code == 1)
            AttackSkill1Act = false;
        else if (code == 2)
            AttackSkill2Act = false;
        StartCoroutine(skillCooltime(code, 3));
    }
    IEnumerator AxeSlashbeem(int code)
    {
        if (code == 1)
            AttackSkill1Act = true;
        else if (code == 2)
            AttackSkill2Act = true;
        GameObject shot;
        if (GetComponent<SpriteRenderer>().flipX)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(2, 5);
            shot = Instantiate(skillOBJ[17], transform.position + new Vector3(-1, 0).normalized * 1.2f, transform.rotation);
            shot.GetComponent<shotWantVector>().angle = 180;
            shot.GetComponent<SpriteRenderer>().flipY = true;
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(-2, 5);
            shot = Instantiate(skillOBJ[17], transform.position + new Vector3(1, 0).normalized * 1.2f, transform.rotation);
            shot.GetComponent<shotWantVector>().angle = 0;
        }
        yield return new WaitForSeconds(0.6f);
        if (code == 1)
            AttackSkill1Act = false;
        else if (code == 2)
            AttackSkill2Act = false;

        StartCoroutine(skillCooltime(code, 5));
    }
    // 갈퀴 스킬

    IEnumerator ClawBig(int code)
    {
        if (code == 1)
            AttackSkill1Act = true;
        else if (code == 2)
            AttackSkill2Act = true;

        if (GetComponent<SpriteRenderer>().flipX)
            Instantiate(skillOBJ[18], transform.position + Vector3.left * 2f, transform.rotation, transform);
        else
            Instantiate(skillOBJ[18], transform.position + Vector3.right * 2f, transform.rotation, transform);
        yield return new WaitForSeconds(0.3f);

        if (code == 1)
            AttackSkill1Act = false;
        else if (code == 2)
            AttackSkill2Act = false;
        StartCoroutine(skillCooltime(code, 5));
    }
    IEnumerator ClawUpper(int code)
    {
        if (code == 1)
            AttackSkill1Act = true;
        else if (code == 2)
            AttackSkill2Act = true;

        Instantiate(skillOBJ[19], transform.position + Vector3.up * 1.26f + Vector3.back * 0.1f, transform.rotation, transform);
        yield return new WaitForSeconds(0.3f);

        if (code == 1)
            AttackSkill1Act = false;
        else if (code == 2)
            AttackSkill2Act = false;
        StartCoroutine(skillCooltime(code, 8));
    }
    IEnumerator ClawFast(int code)
    {
        if (code == 1)
            AttackSkill1Act = true;
        else if (code == 2)
            AttackSkill2Act = true;
        GameObject claw;
        claw = Instantiate(skillOBJ[20], transform.position, transform.rotation, transform);
        claw.GetComponent<SpriteRenderer>().flipX = true;
        for (int i = 0; i < 4; i++)
        {
            claw.SetActive(true);
            yield return new WaitForSeconds(0.333f);
            claw.SetActive(false);
            yield return new WaitForSeconds(0.02f);
            claw.SetActive(true);
        }
        Destroy(claw);
        if (code == 1)
            AttackSkill1Act = false;
        else if (code == 2)
            AttackSkill2Act = false;
        StartCoroutine(skillCooltime(code, 6));
    }
    // 주먹 스킬

    IEnumerator FastPunch(int code)
    {
        if (code == 1)
            AttackSkill1Act = true;
        else if (code == 2)
            AttackSkill2Act = true;
        GameObject punch;
        if (GetComponent<SpriteRenderer>().flipX)
        {
            punch = Instantiate(skillOBJ[21], transform.position + Vector3.left * 0.5f, transform.rotation, transform);
            punch.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
            punch = Instantiate(skillOBJ[21], transform.position + Vector3.right * 0.5f, transform.rotation, transform);
        for (int i = 0; i < 12; i++)
        {
            if (GetComponent<SpriteRenderer>().flipX)
                punch.transform.position = transform.position + Vector3.left * 0.5f;            
            else
                punch.transform.position = transform.position + Vector3.right * 0.5f;

            punch.GetComponent<SpriteRenderer>().flipX = GetComponent<SpriteRenderer>().flipX;

            punch.SetActive(true);
            yield return new WaitForSeconds(0.06f);
            punch.SetActive(false);
            yield return new WaitForSeconds(0.01f);
            punch.SetActive(true);
        }
        yield return new WaitForSeconds(0.01f);
        Destroy(punch);
        skillonbutcantMove = true;
        if (GetComponent<SpriteRenderer>().flipX)
        {
            punch = Instantiate(skillOBJ[22], transform.position + Vector3.left * 1.25f, transform.rotation, transform);
            punch.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
            punch = Instantiate(skillOBJ[22], transform.position + Vector3.right * 1.25f, transform.rotation, transform);
        yield return new WaitForSeconds(0.1f);
        Destroy(punch);
        skillonbutcantMove = false;        
        if (code == 1)
            AttackSkill1Act = false;
        else if (code == 2)
            AttackSkill2Act = false;

        StartCoroutine(skillCooltime(code, 15));
    }
    IEnumerator PushPunch(int code)
    {
        if (code == 1)
            AttackSkill1Act = true;
        else if (code == 2)
            AttackSkill2Act = true;
        GameObject punch;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);
        skillonbutcantMove = true;
        yield return new WaitForSeconds(0.75f);
        if (GetComponent<SpriteRenderer>().flipX)
        {
            punch = Instantiate(skillOBJ[23], transform.position + Vector3.left * 3f, transform.rotation, transform);
            punch.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
            punch = Instantiate(skillOBJ[23], transform.position + Vector3.right * 3f, transform.rotation, transform);
        
        yield return new WaitForSeconds(0.1f);
        skillonbutcantMove = false;
        Destroy(punch);
        if (code == 1)
            AttackSkill1Act = false;
        else if (code == 2)
            AttackSkill2Act = false;

        StartCoroutine(skillCooltime(code, 10));
    }
    IEnumerator DownPunch(int code)
    {
        if (code == 1)
            AttackSkill1Act = true;
        else if (code == 2)
            AttackSkill2Act = true;
        GameObject punch;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);
        if (GetComponent<SpriteRenderer>().flipX)
        {
            punch = Instantiate(skillOBJ[24], transform.position, transform.rotation, transform);
            punch.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
            punch = Instantiate(skillOBJ[24], transform.position, transform.rotation, transform);

        yield return new WaitForSeconds(0.2f);
        Destroy(punch);
        if (code == 1)
            AttackSkill1Act = false;
        else if (code == 2)
            AttackSkill2Act = false;

        StartCoroutine(skillCooltime(code, 7));
    }
    // 착취 스킬

    IEnumerator BackAttack(int code)
    {
        if (code == 1)
            AttackSkill1Act = true;
        else if (code == 2)
            AttackSkill2Act = true;
        GameObject explo;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);
        if (GetComponent<SpriteRenderer>().flipX)
        {
            explo = Instantiate(skillOBJ[25], transform.position + Vector3.left * -3.4f, transform.rotation, transform);
            explo.transform.localScale = new Vector3(-explo.transform.localScale.x, explo.transform.localScale.y, 1);
        }
        else
            explo = Instantiate(skillOBJ[25], transform.position + Vector3.left * 3.4f, transform.rotation, transform);

        yield return new WaitForSeconds(0.2f);
        Destroy(explo);
        if (code == 1)
            AttackSkill1Act = false;
        else if (code == 2)
            AttackSkill2Act = false;

        StartCoroutine(skillCooltime(code, 2));
    }
    IEnumerator SplashAttack(int code)
    {
        if (code == 1)
            AttackSkill1Act = true;
        else if (code == 2)
            AttackSkill2Act = true;
        GameObject explo;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);
        if (GetComponent<SpriteRenderer>().flipX)
        {
            explo = Instantiate(skillOBJ[26], transform.position + Vector3.right * -2 + Vector3.up * 1.3f, transform.rotation, transform);
            explo.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
            explo = Instantiate(skillOBJ[26], transform.position + Vector3.right * 2 + Vector3.up * 1.3f, transform.rotation, transform);

        yield return new WaitForSeconds(0.2f);
        Destroy(explo);
        if (code == 1)
            AttackSkill1Act = false;
        else if (code == 2)
            AttackSkill2Act = false;

        StartCoroutine(skillCooltime(code, 8));
    }
    IEnumerator Zeolite(int code)
    {
        skillonbutcanAct = true;
        GameObject g;

        g = Instantiate(skillOBJ[27], transform.position + Vector3.up * 6, transform.rotation);

        yield return new WaitUntil(() => g);
        skillonbutcanAct = false;
        StartCoroutine(skillCooltime(code, 15));
    }

    // 그 외 코루틴
    IEnumerator skillCooltime(int code, float cooltime)
    {
        yield return new WaitForSeconds(cooltime);
        switch (code)
        {
            case 1:
                canAttackSkill1 = true;
                break;
            case 2:
                canAttackSkill2 = true;
                break;
        }
    }

}
