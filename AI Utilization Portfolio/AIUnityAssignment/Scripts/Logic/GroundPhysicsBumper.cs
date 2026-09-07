using UnityEngine;

public class GroundPhysicsBumper : MonoBehaviour
{
    void Start()
    {
        CharacterController playerController = FindObjectOfType<CharacterController>();
        Collider myCollider = GetComponent<Collider>();

        if (playerController != null && myCollider != null)
        {
            Physics.IgnoreCollision(playerController, myCollider, true);
        }
    }
}