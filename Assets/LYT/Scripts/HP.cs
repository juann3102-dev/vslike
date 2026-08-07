using UnityEngine;

public class HP : MonoBehaviour
{
    public float CurrentHp { get; private set; }
    public float MaxHp { get; private set; }

    public void Init(float maxHp)
    {
        this.MaxHp = maxHp;
        this.CurrentHp = maxHp;
        Debug.Log($"HP 컴포넌트 초기화 완료! 최대 체력: {MaxHp}");
    }

    public void TakeDamage(float amount)
    {
        CurrentHp = Mathf.Max(0, CurrentHp - amount);
    }
}
