
using UnityEngine;

public class ObjectBehaviour : MonoBehaviour
{
    public GameObject[] ObjectToEnable;
    public MeshRenderer[] Mats;
    public Material burnMat;
    public bool Training,blasted;

    public void DoAction()
    {
        for(int i = 0; i < ObjectToEnable.Length; i++)
        {
            ObjectToEnable[i].SetActive(true);
        }

        for(int i = 0; i < Mats.Length; i++)
        {
            Mats[i].material = burnMat;
        }

        if (Training&&!blasted)
        {
            blasted = true;
            GetComponent<Target>().enabled = false;
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerTraining>().TargetDown();
        }
    }
}
