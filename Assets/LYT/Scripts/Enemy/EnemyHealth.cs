using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("HP Settings")]
    private float maxHealth;
    private float currentHealth;

    [Header("UI Reference")]
    [SerializeField] private Image hpFillImage; // EnemyCanvas 내부의 Fill Image 참조
    [SerializeField] private Text HPtext;

    public void UIInitialize(float Health)
    {
        maxHealth = Health;
        currentHealth = maxHealth;
        UpdateHPBar(currentHealth);
    }


    public void UpdateHPBar(float currentHP)
    {
        HPtext.text = $"{currentHP} / {maxHealth}";
        if (hpFillImage != null)
        {
            hpFillImage.fillAmount = currentHP / maxHealth;
        }
    }

}