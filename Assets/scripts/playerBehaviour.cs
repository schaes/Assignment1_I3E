using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    int score = 0;
    int playerHealth = 10;
    int playerMaxHealth = 10;
    bool canInteract = false;
    public bool Key = false;

    coinBehaviour currentCoin;
    DoorBehaviour currentDoor;
    ImportantObjectBehaviour currentImportantObject;

    [SerializeField]
    Transform spawnPoint;

    [SerializeField]
    float InteractionDistance = 5f;

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

            }
            else if (hitInfo.collider.CompareTag("door") || hitInfo.collider.CompareTag("LockedDoor"))
            {
                canInteract = true;
                currentDoor = hitInfo.collider.GetComponent<DoorBehaviour>();
            }
        }
        else if (currentCoin != null)
        {
            currentCoin = null;
            canInteract = false;
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
            }
        }


        else if (collision.gameObject.CompareTag("hazard"))
        {
            if (playerHealth > 0)
            {
                --playerHealth;
                Debug.Log("Player damaged! Player Health: " + playerHealth);
            }

            else if (playerHealth <= 0)
            {
                Debug.Log("Game Over!");
            }
        }

    }
    void OnTriggerEnter(Collider other)
    {
        // Lava logic
        if (other.gameObject.CompareTag("lava"))
        {
            playerHealth = 0; // Set player health to 0 when entering lava
            Debug.Log("Player entered lava! Game Over!");
        }
    }
    
    
    public void ModifyScore(int amount)
    {
        score += amount;
        Debug.Log("Score: " + score);
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