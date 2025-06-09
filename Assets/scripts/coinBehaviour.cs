using Unity.VisualScripting;
using UnityEngine;

public class coinBehaviour : MonoBehaviour
{
    [SerializeField]
    int value = 10;

        public void Collect(PlayerBehaviour player)
    {
        // Example: Increase score and destroy coin
        player.ModifyScore(value);
        Destroy(gameObject);
    }
}

