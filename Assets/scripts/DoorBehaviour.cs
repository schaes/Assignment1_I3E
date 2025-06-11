using System.Data.Common;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    [SerializeField]
    bool willItLock;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Interact()
    {
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
            if (Mathf.Approximately(doorRotation.y, 90f) && Vector3.Distance(transform.position, player.position) > closeDistance)
            {
                doorRotation.y = 0f; // Reset the door rotation to 0 degrees
                transform.eulerAngles = doorRotation; // Apply the rotation to the door
            } 
        }
    }

}
