using UnityEngine;
using UnityEngine.SceneManagement;

public class starclicker : MonoBehaviour
{
    public Material clickMaterial;
    private bool isHovering = false;
    private Renderer objectRenderer;
    public ScoreManager scoremanager;

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

        if (Input.GetMouseButtonDown(0))
        {
            clickedThisFrame = true;
        }
    }

    void LateUpdate()
    {
        if (clickedThisFrame && !clickHadHit && !missLoggedThisFrame)
        {
            Debug.Log("Miss");
            missLoggedThisFrame = true;
            scoremanager.CountMiss();
        }
    }
}
