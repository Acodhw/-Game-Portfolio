using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillGet : MonoBehaviour
{
    private PlayerState ps;
    public int GettingSkillNum;
    // Start is called before the first frame update
    void Start()
    {
        ps = GameObject.Find("GameManager").GetComponent<PlayerState>();
        if (ps.getCanSelectSkills(GettingSkillNum)) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player") {
            ps.setCanSelectSkills(GettingSkillNum, true);
            Destroy(gameObject);
        }
    }
}
