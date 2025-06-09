using System.Data.Common;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
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

}
