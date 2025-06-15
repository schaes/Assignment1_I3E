/*
* Author: Emilie Tee Jing Hui
* Date: 11/6/2025
* Description: PlayerBehaviour script that handles the player's interactions, health, score, and collectibles in the game.
*/

using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using StarterAssets;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.SocialPlatforms.Impl;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour
{
    
    public Transform respawnPoint;
    public GameObject WinText;
    public GameObject NotificationText;
    public GameObject AchievementText;
    int score = 0;
    int playerHealth = 10;
    bool canInteract = false;
    bool Key = false;
    bool GasMask = false; // Variable to track if the player has collected the gas mask
    bool inPoisonGas = false; // Variable to track if the player is in poison gas
    int collected = 0; // Number of collectibles collected
    int collectibleCount;
    Coroutine poisonGasCoroutine; // Coroutine for poison gas effect
    coinBehaviour currentCoin;
    DoorBehaviour currentDoor;
    ImportantObjectBehaviour currentImportantObject;
    AudioSource achievementAudioSource; // Reference to the AudioSource component

    [SerializeField]
    AudioClip hurtSound;
    [SerializeField]
    AudioClip spawnAudioSource; 
    [SerializeField]
    Transform spawnPoint;
    [SerializeField]
    float InteractionDistance = 5f;
    [SerializeField]
    TextMeshProUGUI scoreText;
    [SerializeField]
    TextMeshProUGUI healthText;
    [SerializeField]
    TextMeshProUGUI collectibleText;
    [SerializeField]
    TextMeshProUGUI FinalScoreText;
    [SerializeField]
    TextMeshProUGUI FinalCollectibleText;
    [SerializeField]
    TextMeshProUGUI DescriptionText;
    [SerializeField]
    TextMeshProUGUI AchievText;


    void Start()
    {
        AchievementText.gameObject.SetActive(false);
        WinText.gameObject.SetActive(false);
        NotificationText.gameObject.SetActive(false);
        collectibleCount = GameObject.FindGameObjectsWithTag("Collectible").Length;
        scoreText.text = "Score: " + score.ToString();
        healthText.text = "Health: " + playerHealth.ToString();
        collectibleText.text = "Collected: " + collected.ToString() + "/" + collectibleCount.ToString();
        achievementAudioSource = GetComponent<AudioSource>(); // Get the AudioSource component attached to the door
    }

    void Update()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(spawnPoint.position, spawnPoint.forward, out hitInfo, InteractionDistance))
        {
            if (hitInfo.collider.CompareTag("Key")) /// Checks if object hit tag is key
            {
                canInteract = true;
                currentImportantObject = hitInfo.collider.GetComponent<ImportantObjectBehaviour>();
                currentImportantObject.Highlight(); //Highlights the key
                NotificationText.gameObject.SetActive(true);
                DescriptionText.text = "Press E to pick up the keycard"; 
            }
            else if (hitInfo.collider.CompareTag("gasMask")) /// Checks if object hit tag is gasMask
            {
                canInteract = true;
                currentImportantObject = hitInfo.collider.GetComponent<ImportantObjectBehaviour>();
                currentImportantObject.Highlight(); //Highlights the mask
                NotificationText.gameObject.SetActive(true);
                DescriptionText.text = "Press E to pick up the gas mask"; 
            }
            if (hitInfo.collider.CompareTag("door")) /// Checks if object hit tag is door
            {
                canInteract = true;
                currentDoor = hitInfo.collider.GetComponent<DoorBehaviour>();
                NotificationText.gameObject.SetActive(true);
                DescriptionText.text = "Press E to Interact";
            }
            else if (hitInfo.collider.CompareTag("LockedDoor")) /// Checks if object hit tag is LockedDoor
            {
                canInteract = true;
                currentDoor = hitInfo.collider.GetComponent<DoorBehaviour>();
                NotificationText.gameObject.SetActive(true);
                if (Key) /// Checks if player has key to unlock the door
                    DescriptionText.text = "Press E to unlock";
                else
                    DescriptionText.text = "This door is Locked!";
            }
        }
        else if (currentDoor != null) // makes it so that you can only interact with the door if you are in range
        {
            currentDoor = null; // Reset current door after interaction
            canInteract = false;
            NotificationText.gameObject.SetActive(false);
        }
        else if (currentImportantObject != null) // makes it so that you can only interact with the key if you are in range
        {
            currentImportantObject.Unhighlight(); // Unhighlight the key if not in range
            currentImportantObject = null; // Reset current important object after interaction
            canInteract = false;
            NotificationText.gameObject.SetActive(false);
        }
        else if (currentImportantObject != null) // makes it so that you can only interact with the key/gasmask if you are in range
        {
            currentImportantObject.Unhighlight(); // Unhighlight the key if not in range
            currentImportantObject = null;
            canInteract = false;
            NotificationText.gameObject.SetActive(false);
        }
    }

    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Collectible"))
        {
            currentCoin = collision.gameObject.GetComponent<coinBehaviour>(); // Get the coinBehaviour component from the collided object
            if (currentCoin != null)
            {
                currentCoin.Collect(this);
                currentCoin = null; // Reset current coin after collection
                collected++;
                collectibleText.text = "Collected: " + collected.ToString() + "/" + collectibleCount.ToString(); // Update the collectible text
                if (collected >= collectibleCount) // Check if all collectibles have been collected and awards achievement upon reaching the required amount
                {
                    AchievementText.SetActive(true);
                    AchievText.text = "Achievement Unlocked!\nAll Collectibles Collected!";
                    achievementAudioSource.Play();
                    StartCoroutine(HideAchievementTextAfterDelay(5f)); // Hide after 5 seconds
                }
            }
        }
        else if (collision.gameObject.CompareTag("spawn")) // Checks if object hit tag is spawn
            respawnPoint.position = gameObject.transform.position; //set new respawn point to new spawn point

        else if (collision.gameObject.CompareTag("hazard")) // Checks if object hit tag is hazard
        {
            //respawn stuff below because player collided with hazard and instantly dies upon collision
            playerHealth = 10; // to reset health to full upon respawn
            healthText.text = "Health: " + playerHealth.ToString(); // Update health text when player collides with hazard
            score = score - 10; // penalty for dying
            scoreText.text = "Score: " + score.ToString();
            var controller = GetComponent<CharacterController>();
            // Disable the character controller to prevent movement during respawn because if player moves when respawning the code might not teleport the player to the respawn point
            if (controller != null)
            {
                controller.enabled = false;
                transform.position = respawnPoint.position;
                AudioSource.PlayClipAtPoint(spawnAudioSource, transform.position); // Play the collect sound at the spawn position
                controller.enabled = true;
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        // Lava logic
        if (other.gameObject.CompareTag("lava"))
        {
            // instant death code, same as hazard collision code
            playerHealth = 10; // to reset health to full upon respawn
            healthText.text = "Health: " + playerHealth.ToString();
            score = score - 10; //death penalty
            scoreText.text = "Score: " + score.ToString();
            var controller = GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
                transform.position = respawnPoint.position;
                AudioSource.PlayClipAtPoint(spawnAudioSource, transform.position); // Play the collect sound at the spawn position
                controller.enabled = true;
            }
        }
        //Poison logic which is different from lava and hazard because it does damage over time
        if (other.gameObject.CompareTag("Poison") && !GasMask)
        {
            inPoisonGas = true;
            if (poisonGasCoroutine == null)
                poisonGasCoroutine = StartCoroutine(PoisonDamageOverTime()); // Start the coroutine for poison damage over time (corountine code at line 223)
        }

        if (other.gameObject.CompareTag("Win")) //when player enters the win trigger
        {
            WinText.gameObject.SetActive(true); //pulls up win screen
            FinalScoreText.text = "Final Score: " + score.ToString();
            FinalCollectibleText.text = "Total Collected: " + collected.ToString() + "/" + collectibleCount.ToString();
        }
    }
        void OnTriggerExit(Collider other) // to stop the poison gas damage when player exits the poison gas trigger
    {
        if (other.gameObject.CompareTag("Poison"))
        {
            inPoisonGas = false;
            if (poisonGasCoroutine != null)
            {
                StopCoroutine(poisonGasCoroutine);
                poisonGasCoroutine = null;
            }
        }

    }
    IEnumerator PoisonDamageOverTime() // Coroutine to handle poison gas damage over time
    {
        while (inPoisonGas)
        {
            playerHealth--;
            AudioSource.PlayClipAtPoint(hurtSound, transform.position); // Play the collect sound at the coin's position
            healthText.text = "Health: " + playerHealth.ToString();
            if (playerHealth <= 0) // If player health reaches 0, respawn the player
            {
                playerHealth = 10;
                healthText.text = "Health: " + playerHealth.ToString();
                score = score - 10;
                scoreText.text = "Score: " + score.ToString();
                var controller = GetComponent<CharacterController>();
                if (controller != null)
                {
                    controller.enabled = false;
                    transform.position = respawnPoint.position;
                    AudioSource.PlayClipAtPoint(spawnAudioSource, transform.position); // Play the collect sound at the spawn position
                    controller.enabled = true;
                }
                break;
            }
            yield return new WaitForSeconds(1f); // Damage every 1 second
        }
        poisonGasCoroutine = null;
    }

    IEnumerator HideAchievementTextAfterDelay(float delay) // Coroutine to hide achievement text after a delay
    {
        yield return new WaitForSeconds(delay); // Wait for 'delay' seconds
        AchievementText.SetActive(false); // Hide the achievement text
    }
    public void ModifyScore(int amount) // Method to modify the player's score
    {
        score += amount;
        scoreText.text = "Score: " + score.ToString();
    }
    void OnInteract() // Method to handle player interaction with doors and important objects
    {
        // Interaction logic here
        if (canInteract)
        {
            if (currentDoor != null) // Checks if the player is interacting with a door
            {
                // Checks if the door is a regular door or locked door
                if (currentDoor.tag == "door")
                {
                    //Debug.Log("Player interacted with a door!");
                    currentDoor.Interact();
                    currentDoor = null;
                }
                else if (currentDoor.tag == "LockedDoor")
                {
                    if (Key) //checks if player has key to unlock door
                    {
                        currentDoor.Interact();
                        currentDoor = null;
                    }
                }

            }
            else if (currentImportantObject != null)
            {
                // checks which important object the player is interacting with
                if (currentImportantObject.tag == "Key")
                    Key = true;
                else if (currentImportantObject.tag == "gasMask")
                    GasMask = true;

                currentImportantObject.Collect(this);
                currentImportantObject.Unhighlight(); // Unhighlight the key after collection
                currentImportantObject = null; // Reset current important object after collection
                NotificationText.gameObject.SetActive(false);
                
            }

        }
    }
}