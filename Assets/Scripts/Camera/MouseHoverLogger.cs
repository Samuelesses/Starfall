using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseHoverLogger : MonoBehaviour
{
    private bool isHovering = false;

    void Update()
    {
        // Cast a ray from the camera to the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        isHovering = false;

        if (Physics.Raycast(ray, out hit))
        {
            // Check if the ray hit this object
            if (hit.collider.gameObject == gameObject)
            {
                Debug.Log("Mouse is hovering over: " + gameObject.name);
                isHovering = true;

                // Check for click while hovering
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log("Clicked on: " + gameObject.name);

                    if (gameObject.name == "Start")
                    {
                        SceneManager.LoadScene("Main");
                    }
                }
            }
        }
    }
}
