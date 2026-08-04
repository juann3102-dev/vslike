using UnityEngine;
using TMPro;

public class BulletImage : MonoBehaviour
{

    public TextMeshProUGUI bulletIdText;

    public void SetId(int id)
    {
        bulletIdText.text = id.ToString();
        bulletIdText.color = Color.black;
    }
    
    void Update()
    {
        
    }
}
