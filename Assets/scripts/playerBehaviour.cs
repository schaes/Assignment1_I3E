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
        //Debug.DrawRay(spawnPoint.position, spawnPoint.forward * InteractionDistance, Color.red);

        if (Physics.Raycast(spawnPoint.position, spawnPoint.forward, out hitInfo, InteractionDistance))
        {
            if (hitInfo.collider.CompareTag("Key"))
            {
                canInteract = true;
                currentImportantObject = hitInfo.collider.GetComponent<ImportantObjectBehaviour>();
                currentImportantObject.Highlight(); //Highlights the key
                NotificationText.gameObject.SetActive(true);
                DescriptionText.text = "Press E to pick up the keycard"; 
            }
            else if (hitInfo.collider.CompareTag("gasMask"))
            {
                canInteract = true;
                currentImportantObject = hitInfo.collider.GetComponent<ImportantObjectBehaviour>();
                currentImportantObject.Highlight(); //Highlights the mask
                NotificationText.gameObject.SetActive(true);
                DescriptionText.text = "Press E to pick up the gas mask"; 
            }
            if (hitInfo.collider.CompareTag("door"))
            {
                canInteract = true;
                currentDoor = hitInfo.collider.GetComponent<DoorBehaviour>();
                NotificationText.gameObject.SetActive(true);
                DescriptionText.text = "Press E to Interact";
            }
            else if (hitInfo.collider.CompareTag("LockedDoor"))
            {
                canInteract = true;
                currentDoor = hitInfo.collider.GetComponent<DoorBehaviour>();
                NotificationText.gameObject.SetActive(true);
                if (Key)
                    DescriptionText.text = "Press E to unlock";
                else
                    DescriptionText.text = "This door is Locked!";
            }
        }
        else if (currentDoor != null)
        {
            currentDoor = null;
            canInteract = false;
            NotificationText.gameObject.SetActive(false);
        }
        else if (currentImportantObject != null)
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
            currentCoin = collision.gameObject.GetComponent<coinBehaviour>();
            if (currentCoin != null)
            {
                currentCoin.Collect(this);
                currentCoin = null; // Reset current coin after collection
                collected++;
                collectibleText.text = "Collected: " + collected.ToString() + "/" + collectibleCount.ToString();
                if (collected >= collectibleCount)
                {
                    AchievementText.SetActive(true);
                    AchievText.text = "Achievement Unlocked!\nAll Collectibles Collected!";
                    achievementAudioSource.Play();
                    StartCoroutine(HideAchievementTextAfterDelay(5f)); // Hide after 5 seconds
                }
            }
        }
        else if (collision.gameObject.CompareTag("spawn"))
            respawnPoint.position = gameObject.transform.position; //set new respawn point to new spawn point

        else if (collision.gameObject.CompareTag("hazard"))
        {
            playerHealth=0; // Set player health to 0 when entering lava
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
        }
    }
    void OnTriggerEnter(Collider other)
    {
        // Lava logic
        if (other.gameObject.CompareTag("lava"))
        {
            playerHealth=0; // Set player health to 0 when entering lava
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
        }
        //Poison logic
        if (other.gameObject.CompareTag("Poison") && !GasMask)
        {
            inPoisonGas = true;
            if (poisonGasCoroutine == null)
                poisonGasCoroutine = StartCoroutine(PoisonDamageOverTime());
        }

        if (other.gameObject.CompareTag("Win"))
        {
            WinText.gameObject.SetActive(true);
            FinalScoreText.text = "Final Score: " + score.ToString();
            FinalCollectibleText.text = "Total Collected: " + collected.ToString() + "/" + collectibleCount.ToString();
        }
    }
        void OnTriggerExit(Collider other)
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
    IEnumerator PoisonDamageOverTime()
    {
        while (inPoisonGas)
        {
            playerHealth--;
            AudioSource.PlayClipAtPoint(hurtSound, transform.position); // Play the collect sound at the coin's position
            healthText.text = "Health: " + playerHealth.ToString();
            if (playerHealth <= 0)
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
    public void ModifyScore(int amount)
    {
        score += amount;
        scoreText.text = "Score: " + score.ToString();
    }
    void OnInteract()
    {
        // Interaction logic here
        if (canInteract)
        {
            if (currentDoor != null)
            {
                if (currentDoor.tag == "door")
                {
                    //Debug.Log("Player interacted with a door!");
                    currentDoor.Interact();
                    currentDoor = null;
                }
                else if (currentDoor.tag == "LockedDoor")
                {
                    if (Key)
                    {
                        //Debug.Log("Player interacted with a locked door!");
                        currentDoor.Interact();
                        currentDoor = null;
                    }
                }

            }
            else if (currentImportantObject != null)
            {
                if (currentImportantObject.tag == "Key")
                    Key = true;
                //Debug.Log("Player collected a key!");
                else if (currentImportantObject.tag == "gasMask")
                    GasMask = true;
                //Debug.Log("Player collected a gas mask!");
                //Debug.Log("Player interacted with an important object!");
                currentImportantObject.Collect(this);
                currentImportantObject.Unhighlight(); // Unhighlight the key after collection
                currentImportantObject = null; // Reset current important object after collection
                NotificationText.gameObject.SetActive(false);
                
            }

        }
    }
}