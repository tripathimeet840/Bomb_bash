using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingAIEnemy : MonoBehaviour
{
    [SerializeField] RagdolMaintainer ragdolMaintainer;
    bool Destroyed;
    private void Start()
    {
        GetComponent<Animator>().SetLayerWeight(1, 0);
    }
    public void KnockBack()
    {
        if (!Destroyed)
        {
            Destroyed = true;
            GetComponent<Target>().enabled = false;
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerTraining>().TargetDown();

            ragdolMaintainer.TurnOnRagdoll();
        }
    }
}
