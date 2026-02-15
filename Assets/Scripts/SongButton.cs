using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SongButton : MonoBehaviour
{
    public string songName;
    public float bpm;
    public AudioClip clip;
    public float lengthSeconds;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        lengthSeconds = clip != null ? Mathf.Max(0f, clip.length - 5f) : 0f;
        button.onClick.RemoveListener(OnClick);
        button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        SongLoadManager.Instance.SelectSong(songName, bpm, clip, lengthSeconds);
    }
}
