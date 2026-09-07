using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField]
    private DamageInfo damag;

    public void setDamage(DamageInfo d) { damag = d; }

    public DamageInfo GetDamage() { return damag; }
}
