using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomBullet : MonoBehaviour
{
    [SerializeField] GameObject boom;
    [SerializeField] GameObject parts;
    PlayerManager playerManager;
    Transform player;
    private void OnDestroy()
    {
        GameObject g;
        DamageInfo di;

        g = Instantiate(boom, transform.position, transform.rotation);
        g.transform.localScale = new Vector3(GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);   
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = player;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 1.3f * playerManager.GetEditedState("SkillPercent") * 0.01f);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);

        float degree;
        for (int i = 0; i <= 8; i++)
        {
            degree = Random.Range(100f, 80f);
            g = Instantiate(parts, transform.position + (Vector3)new Vector2(Mathf.Cos(Mathf.Deg2Rad * degree), Mathf.Sin(Mathf.Deg2Rad * degree)) * 1.5f, transform.rotation);
            g.transform.localScale = new Vector3(GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
            g.GetComponent<ShotsEvent>().SetVelocityFixValue(GetComponent<SpriteRenderer>().flipX ? -1 : 1);
            g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2(Mathf.Cos(Mathf.Deg2Rad * degree), Mathf.Sin(Mathf.Deg2Rad * degree)), 22.5f));
            di = g.GetComponent<PlayerAttacks>().GetDamage();
            di.order = player;
            di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.25f * playerManager.GetEditedState("SkillPercent") * 0.01f);
            g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
            g.GetComponent<ShotsEvent>().Shot();
        }
    }

    private void Start()
    {
        playerManager = GameObject.Find("GameManager").GetComponent<PlayerManager>();
    }

    public void SetPlayer(Transform transform) {
        player = transform;
    }
}
