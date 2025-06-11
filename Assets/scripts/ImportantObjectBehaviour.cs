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

    void Start()
    {
        myMeshRenderer = GetComponent<MeshRenderer>();
        originalColor = myMeshRenderer.material;
    }

    public void Collect(PlayerBehaviour player)
    {
        player.KeyCollected();
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


