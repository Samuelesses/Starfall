using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SongLoadManager : MonoBehaviour
{
    public static SongLoadManager Instance;

    public AudioClip selectedClip;
    public string selectedSongName;
    public float selectedBPM;
    public float selectedLengthSeconds;
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

    public void SelectSong(string songName, float bpm, AudioClip clip, float selectedseconds)
    {
        selectedSongName = songName;
        selectedBPM = bpm;
        selectedClip = clip;
        selectedLengthSeconds = selectedseconds;

        SceneManager.LoadScene("Main");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Main") return;

        StarManager sm = FindObjectOfType<StarManager>();
        AudioSource target = null;
        sm.songLength = selectedLengthSeconds;
        if (sm != null)
        {
            target = sm.musicSource != null ? sm.musicSource : sm.GetComponent<AudioSource>();
            if (selectedBPM > 0f)
            {
                sm.bpm = selectedBPM;
            }
        }
        if (target == null) target = FindObjectOfType<AudioSource>();

        if (target != null)
        {
            if (selectedClip != null)
            {
                target.clip = selectedClip;
            }
            target.playOnAwake = false;
            target.Stop();
            target.time = 0f;
        }

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
