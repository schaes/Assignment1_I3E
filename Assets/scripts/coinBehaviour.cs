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
    int value = 10;

    MeshRenderer myMeshRenderer;
    [SerializeField]
    Material highlightColor;
    [SerializeField]
    Material originalColor;

    void Start()
    {
        myMeshRenderer = GetComponent<MeshRenderer>();
        originalColor = myMeshRenderer.material;
    }

    public void Collect(PlayerBehaviour player)
    {
        player.ModifyScore(value);
        Destroy(gameObject);
    }

    void Update()
    {
        this.transform.Rotate(Vector3.up, 100 * Time.deltaTime, Space.World);
    }
}

