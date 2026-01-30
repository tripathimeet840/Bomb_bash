using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class TouchToDie : MonoBehaviour
{
    public CinemachineVirtualCamera playerFollowCam;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<Health>().TakeDamage(100);
            playerFollowCam.Follow = null;
            other.GetComponent<RagdolMaintainer>().StopFalling();
            
        }
        if (other.transform.root.CompareTag("Player"))
        {
            other.transform.root.GetComponent<Health>().TakeDamage(100);
            playerFollowCam.Follow = null;
            other.transform.root.GetComponent<RagdolMaintainer>().StopFalling();
        }

        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Health>().TakeDamage(100);
            other.GetComponent<AIMovement>().DisableGameObject();
            other.GetComponent<RagdolMaintainer>().StopFalling();

        }
        if (other.transform.root.CompareTag("Enemy"))
        {
            other.transform.root.GetComponent<Health>().TakeDamage(100);
            other.transform.root.GetComponent<AIMovement>().DisableGameObject();
            other.transform.root.GetComponent<RagdolMaintainer>().StopFalling();
        }

        if (other.CompareTag("Barel"))
        {
            other.gameObject.SetActive(false);
            GameObject explosive = ObjectPooler.instance.GetObject("LargeExplosive");
            explosive.GetComponent<ParticleSystem>().Clear();
            explosive.transform.position = transform.position;
            explosive.SetActive(true);
        }
    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Player"))
        {
            other.transform.GetComponent<Health>().TakeDamage(100);
            playerFollowCam.Follow = null;
            other.transform.GetComponent<RagdolMaintainer>().StopFalling();

        }
        if (other.transform.root.CompareTag("Player"))
        {
            other.transform.root.GetComponent<Health>().TakeDamage(100);
            playerFollowCam.Follow = null;
            other.transform.root.GetComponent<RagdolMaintainer>().StopFalling();
        }

        if (other.transform.CompareTag("Enemy"))
        {
            other.transform.GetComponent<Health>().TakeDamage(100);
            other.transform.GetComponent<AIMovement>().DisableGameObject();
            other.transform.GetComponent<RagdolMaintainer>().StopFalling();

        }
        if (other.transform.root.CompareTag("Enemy"))
        {
            other.transform.root.GetComponent<Health>().TakeDamage(100);
            other.transform.root.GetComponent<AIMovement>().DisableGameObject();
            other.transform.root.GetComponent<RagdolMaintainer>().StopFalling();
        }
    }
}
