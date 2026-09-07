using UnityEngine;
using UnityEngine.UI;

public class EnemyState : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;

    [Tooltip("머리 위에 띄워줄 체력바 슬라이더 (UI)")]
    public Slider hpSlider;

    private void Start()
    {
        currentHP = maxHP;
        UpdateUI();
    }
    public void TakeDamage(int damage, GameObject effectPrefab = null)
    {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;

        if (effectPrefab != null)
        {
            Vector3 centerPos = transform.position + Vector3.up * 1.0f;
            Vector2 rand = Random.insideUnitCircle * 1.0f;
            Vector3 randomPos = centerPos + new Vector3(rand.x, rand.y, 0f);
            if (Camera.main != null)
            {
                randomPos -= Camera.main.transform.forward * 0.5f;
            }
            GameObject effect = Instantiate(effectPrefab, randomPos, effectPrefab.transform.rotation);
            Destroy(effect, 2f);
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;
            if (currentHP <= 0)
            {
                if (hpSlider.transform.parent != null)
                {
                    hpSlider.transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    hpSlider.gameObject.SetActive(false);
                }
            }
        }
    }
}