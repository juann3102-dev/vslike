using UnityEngine;

public class Sword : MonoBehaviour
{
    private float rotSpeed = 2f;
    private Vector3 rotVec = new Vector3(0f, 0f, 30f);

    void Start()
    {
        transform.eulerAngles = rotVec;
    }

    void Update()
    {
        rotVec.z -= rotSpeed;
        transform.eulerAngles = rotVec;

        if (rotVec.z < -30f) Destroy(gameObject);
    }
}
