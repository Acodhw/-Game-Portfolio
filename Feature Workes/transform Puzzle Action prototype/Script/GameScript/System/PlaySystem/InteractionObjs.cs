using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class InteractObjs : MonoBehaviour
{
    private Rigidbody2D rigid;
    private SpriteRenderer sprite;

    private float[] EnergyTime;
    [SerializeField] private GameObject hanging;

    [SerializeField]
    private GameObject[] EnergyObjs;

    [SerializeField]
    private float mess;

    [SerializeField]
    private int objForm;

    [SerializeField]
    private Image uiImg;

    [SerializeField]
    private ParticleSystem fluidAirParticle;

    [SerializeField]
    private Sprite[] formIcons;

    [SerializeField]
    private PhysicsMaterial2D fullFriction;
    [SerializeField]  
    private PhysicsMaterial2D noFriction;
    [SerializeField]
    private PhysicsMaterial2D fullElast;

    private void Awake()
    {
        EnergyTime = new float[2];
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject != EnergyObjs[0] && collision.gameObject != EnergyObjs[1])
        {
            if (collision.gameObject.layer == 9 && (objForm == 1 || objForm == 2))
            {
                if (collision.tag.Equals("Fire")) EnergyTime[0] = 1f;
                if (collision.tag.Equals("Electric") && objForm == 1) EnergyTime[1] = 1f;
            }
        }

        if(collision.tag.Equals("BackWall"))
        {
            if(hanging == null) hanging = collision.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("BackWall"))
        {
            hanging = null;
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 2; i++)
        {
            if (EnergyTime[i] <= 0)
            {
                EnergyTime[i] = 0;
                if(i == 0) fluidAirParticle.gameObject.SetActive(false);
                EnergyObjs[i].SetActive(false);
            }
            else
            {
                EnergyTime[i] -= Time.deltaTime;
                if (i == 0 && objForm == 2) fluidAirParticle.gameObject.SetActive(true);
                else EnergyObjs[i].SetActive(true);
            }
        }

        var tmp = fluidAirParticle.shape;
        tmp.scale = new Vector3(transform.localScale.x, 1, 1);
        tmp.position = new Vector3(0, -transform.localScale.y / 2, 0);
        var tmp2 = fluidAirParticle.main;
        tmp2.startLifetime = (5.5f / 3) * transform.localScale.y;

        if (objForm == 0)
        {
            uiImg.gameObject.SetActive(false);
            rigid.bodyType = RigidbodyType2D.Kinematic;
        }
        else
        {
            uiImg.rectTransform.sizeDelta = new Vector2(2 / transform.localScale.x, 2 / transform.localScale.y);
            uiImg.gameObject.SetActive(true);
            uiImg.sprite = formIcons[objForm];
            if (objForm != 2)
            {
                rigid.bodyType = RigidbodyType2D.Dynamic;
                rigid.mass = mess;
            }
            else rigid.bodyType = RigidbodyType2D.Kinematic;
        }

        switch (objForm)
        {
            case 0:
                rigid.linearVelocity = Vector2.zero;
                rigid.gameObject.layer = 8;
                rigid.GetComponent<Collider2D>().isTrigger = false;
                rigid.sharedMaterial = noFriction;
                rigid.linearDamping = 0;
                sprite.color = new Color(1, 0, 1);
                break;
            case 1:
                rigid.gameObject.layer = 8;
                rigid.GetComponent<Collider2D>().isTrigger = false;
                rigid.sharedMaterial = noFriction;
                rigid.linearDamping = 0f;
                sprite.color = new Color(0.5f, 0.5f, 0.7f);
                break;
            case 2:
                rigid.gameObject.layer = 4;
                rigid.GetComponent<Collider2D>().isTrigger = true;
                rigid.linearDamping = 0f;
                sprite.color = new Color(0, 1, 1);
                break;
            case 3:
                rigid.gameObject.layer = 8;
                rigid.GetComponent<Collider2D>().isTrigger = false;
                rigid.sharedMaterial = fullElast;
                rigid.linearDamping = 0f;
                sprite.color = new Color(1, 1, 0);
                break;
            case 4:
                rigid.gameObject.layer = 8;
                rigid.GetComponent<Collider2D>().isTrigger = false;
                rigid.sharedMaterial = fullFriction;
                rigid.linearDamping = hanging != null ? 30f : 0f;
                sprite.color = new Color(0, 0.7f, 0);
                break;
        }
    }

    public void SettingObjForm(int form)
    {
        if (objForm != form)
        {
            EnergyTime[0] = 0;
            EnergyTime[1] = 0;
        }
        objForm = form;
    }

    public int GetObjForm()
    {
        return objForm;
    }
}
