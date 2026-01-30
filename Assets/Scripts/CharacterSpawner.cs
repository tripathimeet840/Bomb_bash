using Unity.Cinemachine;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    [Header("Characters in Game Scene")]
    public GameObject[] characters;
    private int selectedCharacter;

    [Header("Camera")]
    public CinemachineCamera followCam; 
    public CinemachineCamera wonCam;

    private void Start()
    {
        selectedCharacter = PlayerPrefs.GetInt("SelectedCharacter", 0);

        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == selectedCharacter);
        }

        GameObject activeCharacter = characters[selectedCharacter];

        Transform followTarget = activeCharacter.transform.Find("FollowTarget");

        if (followTarget == null)
            followTarget = activeCharacter.transform;

        if (followCam != null)
        {
            followCam.Follow = followTarget;
            followCam.LookAt = followTarget;
            wonCam.Follow = followTarget;
            wonCam.LookAt = followTarget;
        }
    }
}
