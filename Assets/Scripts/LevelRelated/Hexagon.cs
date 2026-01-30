using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    public List<Transform> AllPlatforms = new List<Transform>();
    public float waiting_Time;
    public int limitedPlatforms; // Set this in the inspector to limit the number of platforms
    public Material RedMat;
    public float flickerDuration = 3f; // Duration of the flickering
    public float flickerInterval = 0.1f; // Interval between flickers

    private bool isFlickeringOrFalling = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlatformFall());
    }

    IEnumerator PlatformFall()
    {
        while (AllPlatforms.Count > limitedPlatforms)
        {
            yield return new WaitForSeconds(waiting_Time);

            if (!isFlickeringOrFalling)
            {
                int index = Random.Range(0, AllPlatforms.Count);
                Transform platformToBefall = AllPlatforms[index];

                // Make the platform flicker and fall
                StartCoroutine(FlickerAndFall(platformToBefall));

                // Remove the platform from the list
                AllPlatforms.RemoveAt(index);
            }
        }
    }

    IEnumerator FlickerAndFall(Transform platform)
    {
        isFlickeringOrFalling = true;

        // Flicker the material of the falling platform to original to red to original for 3 seconds
        Renderer platformRenderer = platform.GetComponent<Renderer>();
        Material originalMat = platformRenderer.material;

        float flickerEndTime = Time.time + flickerDuration;
        while (Time.time < flickerEndTime)
        {
            platformRenderer.material = RedMat;
            yield return new WaitForSeconds(flickerInterval);
            platformRenderer.material = originalMat;
            yield return new WaitForSeconds(flickerInterval);
        }

        Vector3 startPosition = platform.position;
        Vector3 endPosition = startPosition + new Vector3(0, -5, 0);
        float fallDuration = 0.8f; // Duration of the fall

        float elapsed = 0;
        while (elapsed < fallDuration)
        {
            platform.position = Vector3.Lerp(startPosition, endPosition, elapsed / fallDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        platform.position = endPosition;

        isFlickeringOrFalling = false;
    }
}
