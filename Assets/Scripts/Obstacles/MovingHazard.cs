using UnityEngine;

public class MovingHazard : MonoBehaviour
{
    public Vector3 pointA;
    public Vector3 pointB;
    public float speed = 5f;
    private Vector3 target;

    void Start()
    {
        target = pointB;
        transform.position = pointA;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            // Swap target
            target = target == pointA ? pointB : pointA;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Health>().TakeDamage(10); // Adjust damage
            other.GetComponent<PlayerMovement>().KnockBack(); // Optional
        }
    }
}
