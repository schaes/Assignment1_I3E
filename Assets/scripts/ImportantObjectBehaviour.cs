/*
* Author: Emilie Tee Jing Hui
* Date: 11/6/2025
* Description: ImportantObjectBehaviour script that handles the collection and highlighting of important objects in the game.
*/
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ImportantObjectBehaviour : MonoBehaviour
{

    MeshRenderer myMeshRenderer;
    [SerializeField]
    Material highlightColor;
    [SerializeField]
    Material originalColor;
    [SerializeField]
    AudioClip collectSound; // Sound to play when the coin is collected

    void Start()
    {
        myMeshRenderer = GetComponent<MeshRenderer>();
        originalColor = myMeshRenderer.material;
    }

    public void Collect(PlayerBehaviour player)
    {
        AudioSource.PlayClipAtPoint(collectSound, transform.position); // Play the collect sound at the coin's position
        Destroy(gameObject);
    }

    public void Highlight()
    {
        myMeshRenderer.material = highlightColor;
    }
    public void Unhighlight()
    {
        myMeshRenderer.material = originalColor;
    }

    void Update()
    {
        this.transform.Rotate(Vector3.up, 100 * Time.deltaTime, Space.World);
    }
}


