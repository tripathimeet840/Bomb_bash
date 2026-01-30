using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f; // Movement speed of the car
    [SerializeField] private float rotationSpeed = 5f; // Speed at which the car rotates towards waypoints
    [SerializeField] private bool isLooping = true; // Whether the car should loop back to the first waypoint after reaching the last one
    [SerializeField] private bool startAtCurrentIndex = true; // Whether the car should start at the current waypoint index

    public Transform[] waypoints; // Array to store waypoint Transforms
    public int currentWaypointIndex = 0; // Index of the current waypoint

    void Start()
    {
        if (waypoints.Length == 0)
        {
            Debug.LogError("No waypoints found! Please assign waypoints to the 'waypoints' array.");
            return;
        }

        // If startAtCurrentIndex is true, set the initial position and rotation
        if (startAtCurrentIndex)
        {
            SetPositionAndRotationAtWaypoint(currentWaypointIndex);
        }
    }

    void Update()
    {
        if (waypoints.Length == 0)
        {
            return;
        }

        // Move the car towards the current waypoint
        Vector3 targetPosition = new Vector3(waypoints[currentWaypointIndex].position.x, transform.position.y, waypoints[currentWaypointIndex].position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Look towards the current waypoint (smooth rotation)
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Check if the car has reached the current waypoint
        if (Vector3.Distance(transform.position, targetPosition) <= 0.1f)
        {
            // Move to the next waypoint or loop back if necessary
            if (currentWaypointIndex == waypoints.Length - 1)
            {
                if (isLooping)
                {
                    currentWaypointIndex = 0;
                }
            }
            else
            {
                currentWaypointIndex++;
            }
        }
    }

    // Helper method to set the car's position and rotation based on a waypoint index
    private void SetPositionAndRotationAtWaypoint(int index)
    {
        Vector3 startPosition = new Vector3(waypoints[index].position.x, transform.position.y, waypoints[index].position.z);
        transform.position = startPosition;

        Vector3 nextWaypointPosition = waypoints[(index + 1) % waypoints.Length].position;
        Quaternion startRotation = Quaternion.LookRotation(new Vector3(nextWaypointPosition.x, transform.position.y, nextWaypointPosition.z) - startPosition);
        transform.rotation = startRotation;
    }
}
