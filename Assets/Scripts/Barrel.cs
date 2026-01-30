using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    [SerializeField] float explosionForce = 10f;
    [SerializeField] float yForce = 5f;
    [SerializeField] float explosionRadius = 5f;
	public float damageAmt = 50;
  

    private void OnCollisionEnter(Collision other)
    {
		if (other.gameObject.CompareTag("Player"))
		{
			SoundManager.soundManager.PlayBigExplosionSound();
			gameObject.SetActive(false);

			GameObject explosive = ObjectPooler.instance.GetObject("LargeExplosive");
			explosive.GetComponent<ParticleSystem>().Clear();
			explosive.transform.position = transform.position;
			explosive.SetActive(true);

			PlayerMovement playerMovement = other.transform.GetComponentInParent<PlayerMovement>();
			if (playerMovement != null)
			{
				// Turn on the ragdoll
				playerMovement.KnockBack();
				playerMovement.GetComponent<Health>().TakeDamage(50);

				// Apply explosion force to each limb
				Rigidbody[] limbs = playerMovement.GetComponentsInChildren<Rigidbody>();
				foreach (Rigidbody limb in limbs)
				{
					Vector3 direction = (limb.transform.position - transform.position).normalized;
					direction.y = yForce;

					// Normalize the direction again to keep the force magnitude consistent
					direction.Normalize();

					// Apply constant force
					limb.AddForce(direction * explosionForce, ForceMode.Impulse);
				}
			}
		}

        //if (other.gameObject.CompareTag("Ice"))
        //{
        //    SoundManager.soundManager.PlayBigExplosionSound();
        //    gameObject.SetActive(false);

        //    GameObject explosive = ObjectPooler.instance.GetObject("LargeExplosive");
        //    explosive.GetComponent<ParticleSystem>().Clear();
        //    explosive.transform.position = transform.position;
        //    explosive.SetActive(true);
        //}

        if (other.gameObject.CompareTag("Enemy"))
		{
			gameObject.SetActive(false);
			SoundManager.soundManager.PlayBigExplosionSound();
			GameObject explosive = ObjectPooler.instance.GetObject("LargeExplosive");
			explosive.GetComponent<ParticleSystem>().Clear();
			explosive.transform.position = transform.position;
			explosive.SetActive(true);

			AIMovement aiMovement = other.transform.GetComponentInParent<AIMovement>();
			if (aiMovement != null)
			{
				// Turn on the ragdoll
				aiMovement.KnockBack();
				aiMovement.GetComponent<Health>().TakeDamage(50);
				// Apply explosion force to each limb
				Rigidbody[] limbs = aiMovement.GetComponentsInChildren<Rigidbody>();
				foreach (Rigidbody limb in limbs)
				{
					Vector3 direction = (limb.transform.position - transform.position).normalized;
					direction.y = yForce; // Ensure the Y component of the force is constant

					// Normalize the direction again to keep the force magnitude consistent
					direction.Normalize();

					// Apply constant force
					limb.AddForce(direction * explosionForce, ForceMode.Impulse);
				}
			}
		}
	}
}
