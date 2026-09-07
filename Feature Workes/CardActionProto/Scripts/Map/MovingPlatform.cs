using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    TmpMoveTile tmt;

    private Vector2 tileVelocity;
    // Start is called before the first frame update
    void Start()
    {
        tmt = GetComponent<TmpMoveTile>();
    }

    // Update is called once per frame
    void Update()
    {
        tileVelocity = tmt.toMove * tmt.i;
    }

    public Vector2 GetTileVelocity() {
        return tileVelocity;
    }
}
