using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private DamageInfo damag;
    public void setDamage(DamageInfo d) {  damag = d; }

    public DamageInfo GetDamage() { return damag; }
}
