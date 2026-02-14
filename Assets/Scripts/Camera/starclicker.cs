using UnityEngine;

public class starclicker : MonoBehaviour
{
    public Material clickMaterial;
    private Renderer objectRenderer;

    void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit)) return;
        if (hit.collider.gameObject != gameObject) return;
        if (!Input.GetMouseButtonDown(0)) return;
        if (clickMaterial != null && objectRenderer != null)
        {
            objectRenderer.material = clickMaterial;
        }
    }
}
