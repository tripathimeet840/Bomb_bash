using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CartoonFX;
public class Bomb : MonoBehaviour
{
	public Rigidbody ball;
	public Transform target;

	public float h = 1;
	public float gravity = -40;

	public bool debugPath;
	public float lifeTime = 0.75f;
    protected ObjectPooler objectPooler;
	float BombThrowerIndex;

	[Header("Damage")]
	public float yForce = 1;
	public float PlayerexplosionRadius = 3.5f;
	public float EnemyexplosionRadius = 3f;

	public float explosionForce = 20;
    protected Coroutine blastCorutine;
	//[SerializeField] GameObject[] AllSkins;

    protected virtual void Start()
    {
		objectPooler = ObjectPooler.instance;
		
	}
	public void SetData(Transform _target,int _index)
    {
		BombThrowerIndex=_index;
		target = _target;
		Launch();
    }

	//public void SetSkin()
 //   {
	//	int skinIndex= PlayerPrefs.GetInt("BombSkin", 0);

	//	foreach(GameObject skin in AllSkins)
 //       {
	//		skin.SetActive(false);
 //       }

	//	AllSkins[skinIndex].SetActive(true);
	//}


    private void OnDisable()
    {
		ball.useGravity = false;
		ball.linearVelocity = Vector3.zero;
		if (blastCorutine != null)
		{
			StopCoroutine(blastCorutine);
			blastCorutine = null;
		}
		//foreach (GameObject skin in AllSkins)
		//{
		//	skin.SetActive(false);
		//}
		//AllSkins[0].SetActive(true);
	}

    protected virtual void Launch()
	{
		Physics.gravity = Vector3.up * gravity;
		ball.useGravity = true;
		Vector3 calculatedVelocity = CalculateLaunchData().initialVelocity;

		// Check if the calculated velocity contains NaN values
		if (float.IsNaN(calculatedVelocity.x) || float.IsNaN(calculatedVelocity.y) || float.IsNaN(calculatedVelocity.z))
		{
			//Debug.LogError("Calculated velocity contains NaN values! Resetting velocity to Vector3.zero.");
			calculatedVelocity = Vector3.zero;
		}

		ball.linearVelocity = calculatedVelocity;
		blastCorutine = StartCoroutine(BlastAfterTime(lifeTime));
	}

	LaunchData CalculateLaunchData()
	{
		float displacementY = target.position.y - transform.position.y;
		Vector3 displacementXZ = new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z);
		float time = Mathf.Sqrt(-2 * h / gravity) + Mathf.Sqrt(2 * (displacementY - h) / gravity);
		Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * h);
		Vector3 velocityXZ = displacementXZ / time;

		return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);
	}

	

	struct LaunchData
	{
		public readonly Vector3 initialVelocity;
		public readonly float timeToTarget;

		public LaunchData(Vector3 initialVelocity, float timeToTarget)
		{
			this.initialVelocity = initialVelocity;
			this.timeToTarget = timeToTarget;
		}

	}
	

	protected IEnumerator BlastAfterTime(float _time)
    {
		yield return new WaitForSeconds(_time);
		GameObject bombvfx = objectPooler.GetObject("PlayerBombVfx");
		bombvfx.transform.position = transform.position;
		if (BombThrowerIndex == 1)
			bombvfx.GetComponent<CFXR_Effect>().cameraShake.enabled = true;
		else
			bombvfx.GetComponent<CFXR_Effect>().cameraShake.enabled = false;

		bombvfx.SetActive(true);
		gameObject.SetActive(false);
        if (BombThrowerIndex == 1)
        {
			SoundManager.soundManager.PlayBombSound();
        }
        else
        {
			SoundManager.soundManager.PlayEnemyBombSound();
        }
		CheckPlayersInContact();
	}

	public void CheckPlayersInContact()
	{
		// Find all colliders within the explosion radius
		float explosiveRadius=0;
		if (BombThrowerIndex == 1)
			explosiveRadius = PlayerexplosionRadius;
		else
			explosiveRadius = EnemyexplosionRadius;

		Collider[] hitColliders = Physics.OverlapSphere(transform.position,explosiveRadius );
		foreach (Collider hitCollider in hitColliders)
		{
            #region PlayerDamage
            PlayerMovement playerMovement = hitCollider.GetComponentInParent<PlayerMovement>();
			if (playerMovement != null && playerMovement.PlayerIndex!=BombThrowerIndex && !playerMovement.gettingDamage)
			{
				// Turn on the ragdoll
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
			}
            #endregion

			#region enmey damage
            AIMovement enemyMovement = hitCollider.GetComponentInParent<AIMovement>();
			if (enemyMovement != null &&!enemyMovement.gettingDamage  && enemyMovement.PlayerIndex!=BombThrowerIndex)
			{
				
				// Turn on the ragdoll
				enemyMovement.KnockBack();

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
			}
			#endregion

			if (hitCollider.CompareTag("Impact"))
			{
				Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
				if (rb != null)
				{
					ApplyExplosionForce(rb);
				}


				if (hitCollider.transform.GetComponent<ObjectBehaviour>() != null)
					hitCollider.transform.GetComponent<ObjectBehaviour>().DoAction();
			}

            if (hitCollider.CompareTag("TrainingEnemy"))
            { 
				TrainingAIEnemy trainingAIEnemy = hitCollider.GetComponent<TrainingAIEnemy>();
				trainingAIEnemy.KnockBack();
				Rigidbody[] limbs = trainingAIEnemy.GetComponentsInChildren<Rigidbody>();
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

	private void OnDrawGizmos()
	{
		// Visualize the explosion radius in the Scene view
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, EnemyexplosionRadius);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, PlayerexplosionRadius);
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
			if (blastCorutine != null && collision.transform.GetComponentInParent<AIMovement>().PlayerIndex != BombThrowerIndex)
			{
				StopCoroutine(blastCorutine);
				StartCoroutine(BlastAfterTime(0));
			}
        }

        if (collision.gameObject.CompareTag("Player"))
        {
			if (blastCorutine != null && collision.transform.GetComponentInParent<PlayerMovement>().PlayerIndex != BombThrowerIndex)
			{
				StopCoroutine(blastCorutine);
				StartCoroutine(BlastAfterTime(0));
			}
		}

        if (collision.gameObject.CompareTag("Bomb"))
        {
			if(blastCorutine!=null)
				StopCoroutine(blastCorutine);
			StartCoroutine(BlastAfterTime(0));
		}
		if (collision.gameObject.CompareTag("Impact"))
		{
			if (blastCorutine != null)
				StopCoroutine(blastCorutine);
			StartCoroutine(BlastAfterTime(0));
			ApplyExplosionForce(collision.transform.GetComponent<Rigidbody>());

			if (collision.transform.GetComponent<ObjectBehaviour>() != null)
				collision.transform.GetComponent<ObjectBehaviour>().DoAction();
		}
		if (collision.gameObject.CompareTag("TrainingEnemy"))
		{
			Debug.Log("Here");
			StopCoroutine(blastCorutine);
			StartCoroutine(BlastAfterTime(0));
		}
	}

	private void ApplyExplosionForce(Rigidbody rb)
	{
		Vector3 direction = (rb.transform.position - transform.position).normalized;
		direction.y = yForce;
		direction.Normalize();
		rb.AddForce(direction * explosionForce, ForceMode.Impulse);
	}
}
