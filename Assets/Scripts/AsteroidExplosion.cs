using UnityEngine;

public class AsteroidCollision : MonoBehaviour
{
    private ObjectPooler objectPooler;

    void Start()
    {
        objectPooler = ObjectPooler.instance;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject explosion = objectPooler.GetObject("TimeBombVfx");
        explosion.transform.position = transform.position;
        explosion.SetActive(true);

        gameObject.SetActive(false);
    }
}
