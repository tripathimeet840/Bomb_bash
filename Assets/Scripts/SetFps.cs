using UnityEngine;

public class SetFps : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = 60;
    }
}
