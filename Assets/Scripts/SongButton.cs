using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SongButton : MonoBehaviour
{
    public string songName;
    public float bpm;
    public AudioClip clip;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        // Ensure we only add one listener
        button.onClick.RemoveListener(OnClick);
        button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        SongLoadManager.Instance.SelectSong(songName, bpm, clip);
    }
}
