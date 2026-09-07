using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class NullObjProto : MonoBehaviour
{
    private QuarterviewPlayerProto player;
    private MeshRenderer mRenderer;
    private Rigidbody rigid;
    private Collider myCol;
    private GameObject fire;
    private GameObject electric;
    private GameObject gipo;

    private Vector3 normalsize;
    private float normalMess;

    private int nowForm = 0;

    [Header("Setting Value")]
    [SerializeField][Tooltip("물체 변환 메테리얼을 지정합니다.")]
    private Material[] formMaterial;
    [SerializeField][Tooltip("물체 변환 피직스 메테리얼을 지정합니다.")]
    private PhysicsMaterial[] physicsMaterials;
    [SerializeField][Tooltip("물체 변환이 유지되는 거리를 지정합니다")][Range(0, 100)]
    private float limitDistance = 20;
    [SerializeField][Tooltip("기본 레이어를 지정합니다.")]
    private int normalLayer = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<QuarterviewPlayerProto>();
        mRenderer = GetComponent<MeshRenderer>();
        rigid = GetComponent<Rigidbody>();
        myCol = GetComponent<Collider>();
        fire = transform.GetChild(1).gameObject;
        electric = transform.GetChild(2).gameObject;
        gipo = transform.GetChild(3).gameObject;
        normalsize = transform.localScale;
        normalMess = rigid.mass;
    }


    // Update is called once per frame
    void Update()
    {
        mRenderer.material = formMaterial[nowForm];
        ReturnPower();
        EnergyInteraction();
    }

    private void EnergyInteraction() {
        if (nowForm != 1 && nowForm != 3)
        {
            fire.SetActive(false);
            electric.SetActive(false);
            gipo.SetActive(false);
            return;
        }
        Collider[] ls;
        bool fi = false, el = false;
        if (GetComponent<Collider>().GetType() == typeof(BoxCollider))
            ls = Physics.OverlapBox(transform.position, transform.localScale * 0.5f, transform.rotation, LayerMask.GetMask("Energy"));
        else 
            ls = Physics.OverlapSphere(transform.position, Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z) * 0.5f, LayerMask.GetMask("Energy"));
        Debug.Log(ls.Length);
        foreach (Collider c in ls) {
            Debug.Log(c.tag);
            if (!c.gameObject.Equals(fire) && !c.gameObject.Equals(electric)) {
                if (c.tag.Equals("Fire")) fi = true;
                if (c.tag.Equals("Electric")) el = true;
            }
        }
        if (nowForm == 1)
        {
            fire.SetActive(fi);
            electric.SetActive(el);
        }
        else if (nowForm == 3) {
            gipo.SetActive(fi);
        }
    }


    public void SetForm() {
        if (nowForm == 0)
        {
            if (player.GetWeapon() == 0) return;
            nowForm = player.GetWeapon();
            player.SetWeaponAble(nowForm);
        }
        else 
        {
            player.SetWeaponAble(nowForm);
            nowForm = 0;            
        }

        myCol.isTrigger = false;
        myCol.material = physicsMaterials[0];
        rigid.isKinematic = false;
        transform.localScale = normalsize;
        rigid.mass = normalMess;
        gameObject.layer = normalLayer;
        
        switch (nowForm)
        {
            case 0:
                rigid.isKinematic = true;
                break;
            case 1:
                break;
            case 2:
                myCol.material = physicsMaterials[2];
                rigid.isKinematic = true;
                break;
            case 3:
                rigid.isKinematic = true;
                myCol.isTrigger = true;
                gameObject.layer = 4;
                break;
            case 4:
                myCol.material = physicsMaterials[1];
                break;
            case 5:
                if (myCol.GetType() == typeof(BoxCollider))
                {
                    Vector3 vec = player.transform.GetChild(0).forward;
                    vec.y = 0;
                    vec = vec.normalized / Mathf.Sqrt(2);
                    vec.x = Mathf.Round(vec.x);
                    vec.z = Mathf.Round(vec.z);
                    if (vec.x != 0) Observable.FromCoroutine(x => ScaleChange(new Vector3(normalsize.x, normalsize.y, normalsize.z * 2)), publishEveryYield: false).Subscribe();
                    else Observable.FromCoroutine(x => ScaleChange(new Vector3(normalsize.x * 2, normalsize.y, normalsize.z)), publishEveryYield: false).Subscribe(); 
                }
                else
                    Observable.FromCoroutine(x => ScaleChange(normalsize * 2), publishEveryYield: false).Subscribe();                            
                break;
            case 6:
                rigid.mass = normalMess * 2;
                break;
        }
    }

    IEnumerator ScaleChange(Vector3 size) {
        Vector3 psize = size - transform.localScale;
        while (transform.localScale.x < size.x || transform.localScale.y < size.y || transform.localScale.z < size.z) {
            if (nowForm != 5) break;
            transform.localScale += psize * Time.deltaTime / (psize.magnitude * 0.3f);
            yield return null;
        }
        if (nowForm == 5) transform.localScale = size;
        else transform.localScale = normalsize;
    } 

    private void ReturnPower() {
        if (nowForm == 0) return;
        if (Vector3.Distance(player.transform.position, transform.position) > limitDistance && rigid.linearVelocity.magnitude < 0.5f) {
            player.SetWeaponAble(nowForm);
            nowForm = 0;
            myCol.isTrigger = false;
            myCol.material = physicsMaterials[0];
            rigid.isKinematic = true;
            transform.localScale = normalsize;
            rigid.mass = normalMess;
            gameObject.layer = normalLayer;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, limitDistance);
    }
}
