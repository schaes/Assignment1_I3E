using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using StarterAssets;
using UnityEngine.ProBuilder.Shapes;

public class PlayerBehaviour : MonoBehaviour
{
    public bool Key = false;
    public Transform respawnPoint;
    public GameObject WinText;
    public GameObject NotificationText;
    int score = 0;
    int playerHealth = 10;
    bool canInteract = false;
    int collected = 0; // Number of collectibles collected
    int collectibleCount;

    coinBehaviour currentCoin;
    DoorBehaviour currentDoor;
    ImportantObjectBehaviour currentImportantObject;

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


    void Start()
    {
        WinText.gameObject.SetActive(false);
        NotificationText.gameObject.SetActive(false);
        collectibleCount = GameObject.FindGameObjectsWithTag("Collectible").Length;
        scoreText.text = "Score: " + score.ToString();
        healthText.text = "Health: " + playerHealth.ToString();
        collectibleText.text = "Collected: " + collected.ToString() + "/" + collectibleCount.ToString();

    }

    // Call this method to set the player's position to a specific point in the map
    public void SetPlayerPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    void Update()
    {
        RaycastHit hitInfo;
        Debug.DrawRay(spawnPoint.position, spawnPoint.forward * InteractionDistance, Color.red);

        if (Physics.Raycast(spawnPoint.position, spawnPoint.forward, out hitInfo, InteractionDistance))
        {
            if (hitInfo.collider.CompareTag("Key"))
            {
                canInteract = true;
                currentImportantObject = hitInfo.collider.GetComponent<ImportantObjectBehaviour>();
                currentImportantObject.Highlight(); //Highlights the key
                NotificationText.gameObject.SetActive(true);
                DescriptionText.text = "Press E to Interact"; 
            }
            else if (hitInfo.collider.CompareTag("door"))
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
                {
                    DescriptionText.text = "Press E to unlock";
                }
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
            }
        }
        else if (collision.gameObject.CompareTag("spawn"))
        {
            respawnPoint.position = gameObject.transform.position; //set new respawn point to new spawn point
            Debug.Log("New respawn point set to: " + respawnPoint.position);
        }

        else if (collision.gameObject.CompareTag("hazard"))
        {
            if (playerHealth > 0)
            {
                --playerHealth;
                healthText.text = "Health: " + playerHealth.ToString();

                if (playerHealth <= 0)
                {
                    Debug.Log("Player health reached 0!");
                    // Reset player health and position
                    playerHealth = 10;
                    healthText.text = "Health: " + playerHealth.ToString();
                    score = score - 10;
                    scoreText.text = "Score: " + score.ToString();
                    var controller = GetComponent<CharacterController>();
                    if (controller != null)
                    {
                        controller.enabled = false;
                        transform.position = respawnPoint.position;
                        controller.enabled = true;
                    }
                }
            }
        }

    }
    void OnTriggerEnter(Collider other)
    {
        // Lava logic
        if (other.gameObject.CompareTag("lava"))
        {
            playerHealth = 0; // Set player health to 0 when entering lava
            Debug.Log("Player entered lava!");
            playerHealth = 10;
            healthText.text = "Health: " + playerHealth.ToString();
            score = score - 10;
            scoreText.text = "Score: " + score.ToString();
            var controller = GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
                transform.position = respawnPoint.position;
                controller.enabled = true;
            }
        }

        if (other.gameObject.CompareTag("Win"))
        {
            WinText.gameObject.SetActive(true);
            FinalScoreText.text = "Final Score: " + score.ToString();
            FinalCollectibleText.text = "Total Collected: " + collected.ToString() + "/" + collectibleCount.ToString();
        }
    }


    public void ModifyScore(int amount)
    {
        score += amount;
        scoreText.text = "Score: " + score.ToString();
    }
    public void KeyCollected()
    {
        Key = true;
        Debug.Log("Key collected!");
    }

    void OnInteract()
    {
        // Handle interaction logic here
        if (canInteract)
        {
            if (currentDoor != null)
            {
                if (currentDoor.tag == "door")
                {
                    Debug.Log("Player interacted with a door!");
                    currentDoor.Interact();
                    currentDoor = null;
                }
                else if (currentDoor.tag == "LockedDoor")
                {
                    if (Key)
                    {
                        Debug.Log("Player interacted with a locked door!");
                        currentDoor.Interact();
                        Key = false; // Reset key after using it
                        currentDoor = null;
                    }
                    else
                    {
                        Debug.Log("This door is locked!");
                    }
                }

            }
            else if (currentCoin != null)
            {
                Debug.Log("Player interacted with a coin!");
                currentCoin.Collect(this);
                currentCoin = null; // Reset current coin after collection
            }
            else if (currentImportantObject != null)
            {
                Debug.Log("Player interacted with an important object!");
                currentImportantObject.Collect(this);
                currentImportantObject.Unhighlight(); // Unhighlight the key after collection
                currentImportantObject = null; // Reset current important object after collection
            }

        }
        else
        {
            Debug.Log("Nothing to interact with!");
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            currentCoin = null;
            canInteract = false;
        }
    }


}