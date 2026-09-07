using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycloneBullet : MonoBehaviour
{

    [SerializeField] GameObject Cyclone;
    PlayerManager playerManager;
    Transform player;
    bool velo;
    private void Start()
    {
        playerManager = GameObject.Find("GameManager").GetComponent<PlayerManager>();
        velo = GetComponent<Rigidbody2D>().velocity.x <= 0;
    }

    public void SetPlayer(Transform transform)
    {
        player = transform;
    }

    private void OnDestroy()
    {
        GameObject g = Instantiate(Cyclone, transform.position, transform.rotation);
        g.transform.localScale = new Vector3(GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(velo ? -1 : 1);
        DamageInfo di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = player;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.1f);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g.GetComponent<ShotsEvent>().Shot();

    }
}
