using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Attack : MonoBehaviour
{
    private float damage;
    private float attackRange;

    // Player로부터 공격 관련 데이터만 받아서 초기화
    public void Init(float damage, float attackRange)
    {
        this.damage = damage;
        this.attackRange = attackRange;
        Debug.Log($"공격 컴포넌트 초기화 완료! 데미지: {damage}, 사거리: {attackRange}");
    }

    /*
    public void Attack()
    {
        if (targetHP == null)
        {
            Debug.LogWarning("공격할 타겟이 없습니다!");
            return;
        }
        // 타겟의 HP 컴포넌트로 데미지 전달
        targetHP.TakeDamage(damage);
    }*/
}
