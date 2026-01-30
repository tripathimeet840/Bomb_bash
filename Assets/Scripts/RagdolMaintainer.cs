using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdolMaintainer : MonoBehaviour
{
    CapsuleCollider mainCollider;
    Rigidbody rb;
    Animator anim;
    Rigidbody[] limbsBody;
    Collider[] limbsCollider;
    public float damageTime;
    public Transform hip; // Reference to the root bone (usually Hip or Pelvis)

    private void Awake()
    {
        GetComponents();
        TurnOffRagdoll();
        mainCollider.isTrigger = false;
    }

    public void TurnOffRagdoll()
    {
        // Disable ragdoll
        anim.enabled = true;
        for (int i = 0; i < limbsBody.Length; i++)
        {
            limbsBody[i].isKinematic = true;
        }

        for (int i = 0; i < limbsCollider.Length; i++)
        {
            limbsCollider[i].enabled = false;
        }
        rb.isKinematic = false;
        mainCollider.enabled = true;
        mainCollider.isTrigger = false;
    }

    public void TurnOnRagdoll()
    {
        for (int i = 0; i < limbsBody.Length; i++)
        {
            limbsBody[i].isKinematic = false;
        }

        for (int i = 0; i < limbsCollider.Length; i++)
        {
            limbsCollider[i].enabled = true;
        }
        rb.isKinematic = true;
        mainCollider.isTrigger = true;
        anim.enabled = false;
    }

    void GetComponents()
    {
        anim = GetComponent<Animator>();
        mainCollider = GetComponent<CapsuleCollider>();
        limbsBody = GetComponentsInChildren<Rigidbody>();
        limbsCollider = GetComponentsInChildren<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    public void ResetPos()
    {
        Vector3 newPosition = hip.position;
        Quaternion rotation = hip.rotation;
        // Turn off ragdoll
        TurnOffRagdoll();

        // Update the player's position and rotation
        transform.position = newPosition;
        transform.rotation = Quaternion.Euler(rotation.x,rotation.y,0);
    }


    public void StopFalling()
    {
        StartCoroutine(Stopwait());
    }

    IEnumerator Stopwait()
    {
        yield return new WaitForSeconds(5f);
        for (int i = 0; i < limbsBody.Length; i++)
        {
            limbsBody[i].isKinematic = true;
        }
        rb.isKinematic = true;
    }
}
