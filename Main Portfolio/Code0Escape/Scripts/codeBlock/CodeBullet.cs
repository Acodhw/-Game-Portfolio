using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class CodeBullet : MonoBehaviour
{
    Rigidbody rigid;
    int damage = 25;
    int speed = 75;
    private PlayerState state;
    private CodeSetting codes;
    private bool isVaccineCodeOn = false;

    Dictionary<string, float> value;

    [SerializeField]
    private GameObject BoomOBJ;
    [SerializeField]
    private float limitTime = 25f;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        state = GameObject.Find("GameManager").GetComponent<PlayerState>();
        codes = GameObject.Find("GameManager").GetComponent<CodeSetting>();
        value = new Dictionary<string, float>();
        rigid.velocity = transform.forward * speed;
        StartCoroutine("codeStart");
        StartCoroutine("TimeLimitDestroy");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3 || other.gameObject.layer == 7) {
            Destroy(gameObject);
        }
    }

    IEnumerator TimeLimitDestroy() {
        yield return new WaitForSeconds(limitTime);
        Destroy(gameObject);
    }

    IEnumerator codeStart()
    {
        List<CodeBlocks> ls = codes.GetSlot(state.GetNowslot()).ToList();

        for(int i = 0; i < ls.Count; i++)
        {
            Boom bo;
            bool check = false;
            float a = 0, b = 0;
            if (ls[i].values[0] != null) 
                if (!value.ContainsKey(ls[i].values[0])) value.Add(ls[i].values[0], 0);

            if (ls[i].values[1] != null)
                if (!value.ContainsKey(ls[i].values[1])) value.Add(ls[i].values[1], 0);

            if (ls[i].values[0] != null)
                a = value[ls[i].values[0]];
            else
                a = ls[i].nums[0];

            if (ls[i].values[1] != null)
                b = value[ls[i].values[1]];
            else
                b = ls[i].nums[1];

            switch (ls[i].kind)
            {                
                case Codekinds.damamge:
                    if (damage < 35) damage++;
                    yield return null;
                    break;
                case Codekinds.boom:
                    bo = Instantiate(BoomOBJ, transform.position, Quaternion.identity).GetComponent<Boom>();
                    bo.damage = Mathf.RoundToInt(damage * 0.5f);
                    bo.vaccine = isVaccineCodeOn;
                    Destroy(gameObject);
                    yield return null;
                    break;
                case Codekinds.speed:
                    if (speed < 125) speed+=5;
                    rigid.velocity = rigid.velocity.normalized * speed;
                    yield return null;
                    break;
                case Codekinds.delay:
                    yield return new WaitForSeconds(a);
                    break;
                case Codekinds.turn:
                    transform.Rotate(0, a, 0);
                    rigid.velocity = transform.forward * speed;
                    yield return null;
                    break;
                case Codekinds.value:
                    switch (ls[i].checks) {
                        case 0:
                            value[ls[i].values[0]] = b;
                            break;
                        case 1:
                            value[ls[i].values[0]] = a + b;
                            break;
                        case 2:
                            value[ls[i].values[0]] = a - b;
                            break;
                        case 3:
                            value[ls[i].values[0]] = a + b;
                            break;
                        case 4:
                            value[ls[i].values[0]] = a / b;
                            break;
                        case 5:
                            value[ls[i].values[0]] = a % b;
                            break;
                    }
                    yield return null;
                    break;
                case Codekinds.vaccine:
                    isVaccineCodeOn = true;
                    yield return null;
                    break;
                case Codekinds.whilecode:
                    switch (ls[i].checks)
                    {
                        case 0:
                            check = (a == b);
                            break;
                        case 1:
                            check = (a > b);
                            break;
                        case 2:
                            check = (a >= b);
                            break;
                        case 3:
                            check = (a < b);
                            break;
                        case 4:
                            check = (a <= b);
                            break;
                        case 5:
                            check = (a != b);
                            break;
                    }
                    
                    while (check && ls[i].child.Count > 0) {
                        for (int j = 0; j < ls[i].child.Count; j++)
                        {
                            
                            if (ls[i].child[j].values[0] != null)
                                if (!value.ContainsKey(ls[i].child[j].values[0])) value.Add(ls[i].child[j].values[0], 0);

                            if (ls[i].child[j].values[1] != null)
                                if (!value.ContainsKey(ls[i].child[j].values[1])) value.Add(ls[i].child[j].values[1], 0);

                            if (ls[i].child[j].values[0] != null)
                                a = value[ls[i].child[j].values[0]];
                            else
                                a = ls[i].child[j].nums[0];

                            if (ls[i].child[j].values[1] != null)
                                b = value[ls[i].child[j].values[1]];
                            else
                                b = ls[i].child[j].nums[1];
                            switch (ls[i].child[j].kind)
                            {
                                case Codekinds.damamge:
                                    if (damage < 35) damage++;
                                    yield return null;
                                    break;
                                case Codekinds.boom:
                                    bo = Instantiate(BoomOBJ, transform.position, Quaternion.identity).GetComponent<Boom>();
                                    bo.damage = Mathf.RoundToInt(damage * 0.5f);
                                    bo.vaccine = isVaccineCodeOn;
                                    Destroy(gameObject);
                                    yield return null;
                                    break;
                                case Codekinds.speed:
                                    if (speed < 125) speed += 5;
                                    rigid.velocity = rigid.velocity.normalized * speed;
                                    yield return null;
                                    break;
                                case Codekinds.delay:
                                    yield return new WaitForSeconds(a);
                                    break;
                                case Codekinds.turn:
                                    transform.Rotate(0, a, 0);
                                    rigid.velocity = transform.forward * speed;
                                    yield return null;
                                    break;
                                case Codekinds.value:
                                    switch (ls[i].child[j].checks)
                                    {
                                        case 0:
                                            value[ls[i].child[j].values[0]] = b;                                           
                                            break;
                                        case 1:
                                            value[ls[i].child[j].values[0]] = a + b;
                                            break;
                                        case 2:
                                            value[ls[i].child[j].values[0]] = a - b;
                                            break;
                                        case 3:
                                            value[ls[i].child[j].values[0]] = a * b;
                                            break;
                                        case 4:
                                            value[ls[i].child[j].values[0]] = a / b;
                                            break;
                                        case 5:
                                            value[ls[i].child[j].values[0]] = a % b;
                                            break;
                                    }
                                    yield return null;
                                    break;
                                case Codekinds.vaccine:
                                    isVaccineCodeOn = true;
                                    yield return null;
                                    break;
                                default:
                                    yield return null;
                                    break;
                            }
                        }

                        if (ls[i].values[0] != null)
                            a = value[ls[i].values[0]];
                        else
                            a = ls[i].nums[0];

                        if (ls[i].values[1] != null)
                            b = value[ls[i].values[1]];
                        else
                            b = ls[i].nums[1];

                        switch (ls[i].checks)
                        {
                            case 0:
                                check = (a == b);
                                break;
                            case 1:
                                check = (a > b);
                                break;
                            case 2:
                                check = (a >= b);
                                break;
                            case 3:
                                check = (a < b);
                                break;
                            case 4:
                                check = (a <= b);
                                break;
                            case 5:
                                check = (a != b);
                                break;
                        }
                    }
                    
                    yield return null;
                    break;
                case Codekinds.ifcode:
                    switch (ls[i].checks)
                    {
                        case 0:
                            check = (a == b);
                            break;
                        case 1:
                            check = (a > b);
                            break;
                        case 2:
                            check = (a >= b);
                            break;
                        case 3:
                            check = (a < b);
                            break;
                        case 4:
                            check = (a <= b);
                            break;
                        case 5:
                            check = (a != b);
                            break;
                    }
                    if (check && ls[i].child.Count > 0)
                    {
                        for (int j = 0; j < ls[i].child.Count; j++)
                        {
                            if (ls[i].child[j].values[0] != null)
                                if (!value.ContainsKey(ls[i].child[j].values[0])) value.Add(ls[i].child[j].values[0], 0);

                            if (ls[i].child[j].values[1] != null)
                                if (!value.ContainsKey(ls[i].child[j].values[1])) value.Add(ls[i].child[j].values[1], 0);

                            if (ls[i].child[j].values[0] != null)
                                a = value[ls[i].child[j].values[0]];
                            else
                                a = ls[i].child[j].nums[0];

                            if (ls[i].child[j].values[1] != null)
                                b = value[ls[i].child[j].values[1]];
                            else
                                b = ls[i].child[j].nums[1];

                            switch (ls[i].child[j].kind)
                            {
                                case Codekinds.damamge:
                                    if (damage < 35) damage++;
                                    yield return null;
                                    break;
                                case Codekinds.boom:
                                    Destroy(gameObject);
                                    yield return null;
                                    break;
                                case Codekinds.speed:
                                    if (speed < 25) speed++;
                                    rigid.velocity = rigid.velocity.normalized * speed;
                                    yield return null;
                                    break;
                                case Codekinds.delay:
                                    yield return new WaitForSeconds(a);
                                    break;
                                case Codekinds.turn:
                                    transform.RotateAround(transform.position, transform.up, a);
                                    yield return null;
                                    break;
                                case Codekinds.value:
                                    switch (ls[i].child[j].checks)
                                    {
                                        case 0:
                                            value[ls[i].child[j].values[0]] = b;
                                            break;
                                        case 1:
                                            value[ls[i].child[j].values[0]] = a + b;
                                            break;
                                        case 2:
                                            value[ls[i].child[j].values[0]] = a - b;
                                            break;
                                        case 3:
                                            value[ls[i].child[j].values[0]] = a * b;
                                            break;
                                        case 4:
                                            value[ls[i].child[j].values[0]] = a / b;
                                            break;
                                        case 5:
                                            value[ls[i].child[j].values[0]] = a % b;
                                            break;
                                    }
                                    yield return null;
                                    break;
                                case Codekinds.vaccine:
                                    isVaccineCodeOn = true;
                                    yield return null;
                                    break;
                                default:
                                    yield return null;
                                    break;
                            }
                        }
                    }
                    yield return null;
                    break;
                default:
                    yield return null;
                    break;
            }
        }
    }

    public int GetDamage() {
        return damage;
    }

    public bool GetVaccineCode()
    {
        return isVaccineCodeOn;
    }
}
