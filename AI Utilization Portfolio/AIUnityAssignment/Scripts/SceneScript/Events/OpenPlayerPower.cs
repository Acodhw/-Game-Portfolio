using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OpenPlayerPower : MonoBehaviour
{
    PlayerState ps;

    // Start is called before the first frame update
    void Start()
    {
        ps = GameObject.Find("GameManager").GetComponent<PlayerState>();
    }

    public void ActivateAttack(bool settingActive)
    {

        ps.attackActive = settingActive;
    }

    public void ActivateProjection(bool settingActive)
    {


        ps.skillActive[0] = settingActive;

    }
    public void ActivateFly(bool settingActive)
    {

        ps.skillActive[1] = settingActive;

    }
    public void ActivateComp(bool settingActive)
    {

        ps.skillActive[2] = settingActive;

    }
    // Update is called once per frame
    void Update()
    {

    }
}
