using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope_Proto : MonoBehaviour
{
    Transform Player;
    public Transform Point;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("mainChar_proto").transform;
    }

    // Update is called once per frame
    void Update()
    {        
        transform.position = (Player.position + Point.position) * 0.5f;
        Vector2 dir = Point.position - Player.position;
        float angle = Vector2.SignedAngle(Vector2.right, dir);
        transform.rotation = Quaternion.Euler(new Vector3(0,0,angle));
        transform.localScale = new Vector2(dir.magnitude, 0.2f);
    }
}
