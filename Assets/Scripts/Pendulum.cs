using UnityEngine;

public class Pendulum : MonoBehaviour
{
    [Header("Pendulum Settings")]
    public float speed = 2f;        
    public float angle = 45f;  

    private float startAngle;

    void Start()
    {
        startAngle = transform.rotation.z;
    }

    void Update()
    {
        float z = angle * Mathf.Sin(Time.time * speed);
        transform.localRotation = Quaternion.Euler(0, 0, z);
    }
}
