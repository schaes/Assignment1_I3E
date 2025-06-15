/*
* Author: Emilie Tee Jing Hui
* Date: 11/6/2025
* Description: Coin behaviour script that handles the coin's collection and rotation.
*/
using Unity.VisualScripting;
using UnityEngine;

public class coinBehaviour : MonoBehaviour
{
    [SerializeField]
    int value = 10; // Value of the coin to be added to the player's score that can be changed in unity editor
    [SerializeField]
    AudioClip collectSound; // Sound to play when the coin is collected

    public void Collect(PlayerBehaviour player) // Method to collect the coin used in playerbehaviour as well, which adds its value to the player's score and plays a sound
    {
        player.ModifyScore(value);
        AudioSource.PlayClipAtPoint(collectSound, transform.position); // Play the collect sound at the coin's position
        Destroy(gameObject);
    }

    void Update() // to rotate the coin continuously
    {
        this.transform.Rotate(Vector3.up, 100 * Time.deltaTime, Space.World);
    }
}

