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

    public void Highlight() // Highlights the object by changing its material to the highlight color
    {
        myMeshRenderer.material = highlightColor;
        foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material = highlightColor;
        }
    }
    public void Unhighlight() // Unhighlights the object by changing its material back to the original color
    {
        myMeshRenderer.material = originalColor;
            foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material = originalColor;
        }
    }

    void Update() // to rotate the important object continuously
    {
        this.transform.Rotate(Vector3.up, 100 * Time.deltaTime, Space.World);
    }
}


