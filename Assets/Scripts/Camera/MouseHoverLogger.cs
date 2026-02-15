using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseHoverLogger : MonoBehaviour
{
    public AudioSource hoversound;
    public GameObject SongSelectionUI;
    public GameObject Credits;
    private bool isHovering;

    void Awake()
    {
        if (hoversound == null) hoversound = GetComponent<AudioSource>();
        if (hoversound == null) hoversound = gameObject.AddComponent<AudioSource>();
        hoversound.playOnAwake = false;
        hoversound.loop = false;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hovering = Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject;
        if (hovering && !isHovering && hoversound != null)
        {
            if (hoversound.clip != null) hoversound.PlayOneShot(hoversound.clip);
            else hoversound.Play();
        }
        isHovering = hovering;
        if (hovering && Input.GetMouseButtonDown(0))
        {
            if (gameObject.name == "Start")
            {
                SongSelectionUI.SetActive(true);
            }

            if (gameObject.name == "Credits")
            {
                Credits.SetActive(true);
            }
        }
    }
}
