using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundShellShot : MonoBehaviour
{
    ShotsEvent se;
    public Sprite changeform;
    // Start is called before the first frame update
    void Start()
    {
        se = GetComponent<ShotsEvent>();
        Invoke("goDown", 0.5f);
    }
    void goDown() {
        GetComponent<SpriteRenderer>().sprite = changeform;
        GetComponent<PlayerAttack>().Damage = 12;
        se.shotDirection = Vector2.down;
        se.shotSpeed = 20;
        se.ShotObj();
    }
}
