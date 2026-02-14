using UnityEngine;
using UnityEngine.SceneManagement;

public class starclicker : MonoBehaviour
{
    public Material clickMaterial;
    private bool isHovering = false;
    private Renderer objectRenderer;

    void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        isHovering = false;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                isHovering = true;

                if (Input.GetMouseButtonDown(0))
                {
                    if (clickMaterial != null && objectRenderer != null)
                    {
                        objectRenderer.material = clickMaterial;
                    }
                }
            }
        }
    }
}
