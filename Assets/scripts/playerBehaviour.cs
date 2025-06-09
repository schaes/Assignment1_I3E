using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    int score = 0;
    int playerHealth = 10;
    int playerMaxHealth = 10;
    bool canInteract = false;
    coinBehaviour currentCoin;
    DoorBehaviour currentDoor;

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Player collided with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("door"))
        {
            canInteract = true;
            currentDoor = collision.gameObject.GetComponent<DoorBehaviour>();
        }

        else if (collision.gameObject.CompareTag("Collectible"))
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
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("hazard"))
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
    
    public void ModifyScore(int amount)
    {
        score += amount;
        Debug.Log("Score: " + score);
    }

    void OnInteract()
    {
        // Handle interaction logic here
        if (canInteract)
        {
            if (currentDoor != null)
            {
                Debug.Log("Player interacted with a door!");
                currentDoor.Interact();
                currentDoor = null;
            }

        }

        Debug.Log("Nothing to interact with!");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            canInteract = true;
            currentDoor = other.GetComponent<DoorBehaviour>();
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