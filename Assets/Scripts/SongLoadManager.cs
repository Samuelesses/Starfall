using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SongLoadManager : MonoBehaviour
{
    public static SongLoadManager Instance;

    public AudioClip selectedClip;
    public string selectedSongName;
    public float selectedBPM;
    [SerializeField] private float beginDelaySeconds = 5f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void SelectSong(string songName, float bpm, AudioClip clip)
    {
        selectedSongName = songName;
        selectedBPM = bpm;
        selectedClip = clip;

        SceneManager.LoadScene("Main");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Main") return;

        // Try to use the StarManager's music source if available
        StarManager sm = FindObjectOfType<StarManager>();
        AudioSource target = null;
        if (sm != null)
        {
            target = sm.musicSource != null ? sm.musicSource : sm.GetComponent<AudioSource>();
            // Sync BPM on StarManager so spawning matches the selected song
            if (selectedBPM > 0f)
            {
                sm.bpm = selectedBPM;
            }
        }
        // Fallback to any AudioSource in the scene
        if (target == null) target = FindObjectOfType<AudioSource>();

        if (target != null)
        {
            if (selectedClip != null)
            {
                target.clip = selectedClip;
            }
            // Do not auto-play; StarManager.Begin() or other script should start it
            target.playOnAwake = false;
            target.Stop();
            target.time = 0f;
        }

        // Begin StarManager after a delay, if present
        if (sm != null)
        {
            StartCoroutine(BeginAfterDelay(sm));
        }
    }

    private IEnumerator BeginAfterDelay(StarManager sm)
    {
        yield return new WaitForSeconds(beginDelaySeconds);
        sm.Begin();
    }
}
