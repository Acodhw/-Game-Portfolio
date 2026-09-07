using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire_Proto : MonoBehaviour
{
    public bool isontheWall;
    public bool destroyBool;
    public Vector2 foward;
    public Transform point;

    float time = 0.7f;

    // Update is called once per frame
    void Update()
    {

        
        int layerMask = 1 << LayerMask.NameToLayer("Ground");

        RaycastHit2D rayhit = Physics2D.Raycast(new Vector2(point.position.x - (foward.x * 0.2f), point.position.y - (foward.y * 0.2f)), foward, 0.2f, layerMask);
        if (rayhit.collider == null)
        {
            time -= Time.deltaTime;
            point.transform.Translate(foward * Time.deltaTime * 24f);
        }
        else
        {
            isontheWall = true;
        }

        if (time <= 0) {
            gameObject.SetActive(false);
        }
    }
}
