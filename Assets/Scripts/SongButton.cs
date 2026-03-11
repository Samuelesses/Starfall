using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

[RequireComponent(typeof(Button))]
public class SongButton : MonoBehaviour
{
    [Header("Only set this!")]
    public string songId; // just the id, e.g. "song_001"

    private Button button;
    [SerializeField]
    private TMP_Text titleText;
    [SerializeField]
    private TMP_Text bpmText;
    [SerializeField]
    private RawImage coverImage;

    private AudioClip clip;
    private float bpm;
    private float lengthSeconds;
    private string title;

    [System.Serializable]
    private class SongData
    {
        public string name;
        public float bpm;
        public string audio;
        public string cover;
    }

    private async void Awake()
    {
        button = GetComponent<Button>();

        // Resolve UI references defensively in case prefab hierarchy/names changed.
        if (titleText == null)
        {
            Transform titleTf = transform.Find("title");
            if (titleTf != null)
            {
                titleText = titleTf.GetComponent<TMP_Text>();
            }

            if (titleText == null)
            {
                titleText = FindTextByName("title");
            }
        }

        if (bpmText == null)
        {
            Transform bpmTf = transform.Find("bpm");
            if (bpmTf != null)
            {
                bpmText = bpmTf.GetComponent<TMP_Text>();
            }

            if (bpmText == null)
            {
                bpmText = FindTextByName("bpm");
            }
        }

        if (coverImage == null)
        {
            Transform coverTf = transform.Find("RawImage/cover");
            if (coverTf != null)
            {
                coverImage = coverTf.GetComponent<RawImage>();
            }

            if (coverImage == null)
            {
                RawImage[] allRawImages = GetComponentsInChildren<RawImage>(true);
                foreach (RawImage image in allRawImages)
                {
                    if (string.Equals(image.gameObject.name, "cover", System.StringComparison.OrdinalIgnoreCase))
                    {
                        coverImage = image;
                        break;
                    }
                }
            }
        }

        if (titleText == null || bpmText == null || coverImage == null)
        {
            Debug.LogError($"[{nameof(SongButton)}] Missing UI reference(s) on '{name}'. Assign titleText/bpmText/coverImage in inspector or fix child names.");
            enabled = false;
            return;
        }

        if (string.IsNullOrWhiteSpace(songId))
        {
            Debug.LogError($"[{nameof(SongButton)}] songId is empty on '{name}'.");
            enabled = false;
            return;
        }

        await LoadSong();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private async System.Threading.Tasks.Task LoadSong()
    {
        string songFolder = Path.Combine(Application.streamingAssetsPath, "Songs", songId);
        string jsonPath = Path.Combine(songFolder, "data.json");

        if (!File.Exists(jsonPath))
        {
            Debug.LogError("Missing data.json for " + songId);
            return;
        }

        SongData data = JsonUtility.FromJson<SongData>(File.ReadAllText(jsonPath));

        if (data == null)
        {
            Debug.LogError("Failed to parse data.json for " + songId);
            return;
        }

        // Set text fields
        title = data.name;
        bpm = data.bpm;
        titleText.text = title;
        float starsPerSecond = bpm / 60f;
        bpmText.text = $"{bpm} BPM ({starsPerSecond:F1} Stars / Second)";

        // Load audio
        string audioPath = Path.Combine(songFolder, data.audio);
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(audioPath, AudioType.OGGVORBIS))
        {
            await www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load audio: " + www.error);
            }
            else
            {
                clip = DownloadHandlerAudioClip.GetContent(www);
                lengthSeconds = Mathf.Max(0f, clip.length - 5f);
            }
        }

        // Load cover image (cover.jpg)
        string coverPath = Path.Combine(songFolder, "cover.jpg");
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(coverPath))
        {
            await www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load cover: " + www.error);
            }
            else
            {
                Texture2D tex = DownloadHandlerTexture.GetContent(www);
                coverImage.texture = tex;
            }
        }
    }

    private void OnClick()
    {
        if (SongLoadManager.Instance == null)
        {
            Debug.LogError($"[{nameof(SongButton)}] SongLoadManager.Instance is null.");
            return;
        }

        SongLoadManager.Instance.SelectSong(title, bpm, clip, lengthSeconds);
    }

    private TMP_Text FindTextByName(string objectName)
    {
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text txt in texts)
        {
            if (string.Equals(txt.gameObject.name, objectName, System.StringComparison.OrdinalIgnoreCase))
            {
                return txt;
            }
        }

        return null;
    }
}