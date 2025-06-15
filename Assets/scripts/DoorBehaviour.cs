/*
* Author: Emilie Tee Jing Hui
* Date: 11/6/2025
* Description: Door behaviour script that handles the door's interaction and locking mechanism.
*/
using System.Data.Common;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    [SerializeField]
    bool willItLock;
    AudioSource doorAudioSource; // Reference to the AudioSource component
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        doorAudioSource = GetComponent<AudioSource>(); // Get the AudioSource component attached to the door
    }
    public void Interact()
    {
        doorAudioSource.Play(); // Play the door sound effect
        Vector3 doorRotation = transform.eulerAngles;
        if (doorRotation.y == 0f)
        {
            doorRotation.y = 90f; // Rotate the door by 90 degrees
        }
        else
        {
            doorRotation.y = 0f; // Reset the door rotation to 0 degrees
        }
        transform.eulerAngles = doorRotation; // Apply the rotation to the door
    }
    public Transform player; // Reference to the player transform

    [SerializeField]
    public float closeDistance = 5f; 

    void Update()
    {
        Vector3 doorRotation = transform.eulerAngles;
        if (willItLock)
        {
            if (Mathf.Approximately(doorRotation.y, 90f) && Vector3.Distance(transform.position, player.position) > closeDistance) // checks if door is open and player is far away
            {
                doorRotation.y = 0f; // Reset the door rotation to 0 degrees
                transform.eulerAngles = doorRotation; // Apply the rotation to the door
                doorAudioSource.Play(); // Play the door sound effect
            } 
        }
    }

}
