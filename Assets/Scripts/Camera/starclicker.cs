using UnityEngine;
using UnityEngine.SceneManagement;

public class starclicker : MonoBehaviour
{
    public Material clickMaterial;
    private bool isHovering = false;
    private Renderer objectRenderer;
    public ScoreManager scoremanager;

    // Frame-wide click state (shared across all starclicker instances)
    private static int processedFrame = -1;
    private static bool clickedThisFrame = false;
    private static bool clickHadHit = false;
    private static bool missLoggedThisFrame = false;

    void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        // Reset shared state once per frame
        if (processedFrame != Time.frameCount)
        {
            processedFrame = Time.frameCount;
            clickedThisFrame = false;
            clickHadHit = false;
            missLoggedThisFrame = false;
        }

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
                    clickedThisFrame = true;
                    clickHadHit = true;

                    Debug.Log(transform.localScale.x);
                    if (clickMaterial != null && objectRenderer != null)
                    {
                        objectRenderer.material = clickMaterial;
                        scoremanager.AddScore(transform.localScale.x);
                    }
                }
            }
        }

        // Cache that a click happened even if not hovering
        if (Input.GetMouseButtonDown(0))
        {
            clickedThisFrame = true;
        }
    }

    void LateUpdate()
    {
        // After all Updates run, log a miss only if no star registered a hit
        if (clickedThisFrame && !clickHadHit && !missLoggedThisFrame)
        {
            Debug.Log("Miss");
            missLoggedThisFrame = true;
            scoremanager.CountMiss();
        }
    }
}
