
using UnityEngine;

public class Puncher : MonoBehaviour
{
    public bool Damagable;
	public float yForce = 2f;
	[SerializeField] AudioClip punch;
	public float explosionForce = 5f;
	SoundManager soundManager;
	public bool asteroid;
    private void Start()
    {
		soundManager = SoundManager.soundManager;
    }

    private void OnEnable()
    {
        if (asteroid)
        {
			GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
			GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!Damagable) return;
        if (collision.gameObject.CompareTag("Player"))
        {
			soundManager.PlayClip(punch);
			PlayerMovement playerMovement = collision.transform.GetComponent<PlayerMovement>();
			playerMovement.KnockBack();
			playerMovement.GetComponent<Health>().TakeDamage(playerMovement.EnemyPower);
			// Apply explosion force to each limb
			Rigidbody[] limbs = playerMovement.GetComponentsInChildren<Rigidbody>();
			foreach (Rigidbody limb in limbs)
			{
				Vector3 direction = (limb.transform.position - transform.position).normalized;
				direction.y = yForce; // Ensure the Y component of the force is constant

				// Normalize the direction again to keep the force magnitude consistent
				direction.Normalize();

				// Apply constant force
				limb.AddForce(direction * explosionForce, ForceMode.Impulse);
			}
			if (asteroid)
			{
				gameObject.SetActive(false);
				GameObject explosive = ObjectPooler.instance.GetObject("LargeExplosive");
				explosive.GetComponent<ParticleSystem>().Clear();
				explosive.transform.position = transform.position;
				explosive.SetActive(true);
			}
		}


		if (collision.gameObject.CompareTag("Enemy"))
		{
			AIMovement enemyMovement = collision.transform.GetComponent<AIMovement>();
			enemyMovement.KnockBack();
			soundManager.PlayClip(punch);

			// Apply explosion force to each limb
			Rigidbody[] limbs = enemyMovement.GetComponentsInChildren<Rigidbody>();
			foreach (Rigidbody limb in limbs)
			{
				Vector3 direction = (limb.transform.position - transform.position).normalized;
				direction.y = yForce; // Ensure the Y component of the force is constant

				// Normalize the direction again to keep the force magnitude consistent
				direction.Normalize();

				// Apply constant force
				limb.AddForce(direction * explosionForce, ForceMode.Impulse);
			}
			if (asteroid)
			{
				gameObject.SetActive(false);
				GameObject explosive = ObjectPooler.instance.GetObject("LargeExplosive");
				explosive.GetComponent<ParticleSystem>().Clear();
				explosive.transform.position = transform.position;
				explosive.SetActive(true);
			}
		}


		if (collision.gameObject.CompareTag("Ground") && asteroid)
        {
			gameObject.SetActive(false);
			soundManager.PlayBigExplosionSound();
			GameObject explosive = ObjectPooler.instance.GetObject("LargeExplosive");
			explosive.GetComponent<ParticleSystem>().Clear();
			explosive.transform.position = transform.position;
			explosive.SetActive(true);
		}
			
	}
}
