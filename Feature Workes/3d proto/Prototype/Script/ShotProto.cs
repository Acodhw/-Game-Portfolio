using UnityEngine;

public class ShotProto : MonoBehaviour
{
    public float removetime;
    void Start()
    {
        Invoke("des", removetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != 6 && other.gameObject.layer != 8 && && other.gameObject.layer != 9 && other.gameObject.layer != 4) Destroy(gameObject);
    }

    void des() { Destroy(gameObject); }

}
