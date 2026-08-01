using UnityEngine;
using TMPro;

public class HPDisplay : MonoBehaviour
{
    public TextMeshProUGUI hpText;
    
    public void UpdateHP(int currentHP)
    {
        hpText.text = currentHP.ToString();
    }
}
