using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class StarBeat
{
    public float time; // seconds into the song
}

public class StarManager : MonoBehaviour
{
    [Header("Star Settings")]
    public GameObject starObject;
    public string spawnTag = "spawn";
    public float shrinkDuration = 1f;
    public Material spawnMaterial;
    public GameObject particlePrefab;

    [Header("Music Settings")]
    public AudioSource musicSource;
    public List<StarBeat> beatMap = new List<StarBeat>();

    private Transform[] spawnPoints;
    private int nextBeatIndex = 0;
    private Vector3 originalScale;
    private Renderer objectRenderer;
    private ParticleSystem spawnedParticle;

    void Start()
    {
        if (starObject == null || musicSource == null || beatMap.Count == 0) return;

        // Find spawn points
        GameObject[] spawns = GameObject.FindGameObjectsWithTag(spawnTag);
        spawnPoints = new Transform[spawns.Length];
        for (int i = 0; i < spawns.Length; i++)
            spawnPoints[i] = spawns[i].transform;

        originalScale = starObject.transform.localScale;
        objectRenderer = starObject.GetComponent<Renderer>();

        // Instantiate particle prefab
        if (particlePrefab != null)
        {
            GameObject go = Instantiate(particlePrefab, starObject.transform.position, Quaternion.identity);
            spawnedParticle = go.GetComponent<ParticleSystem>();
            go.SetActive(false);
        }

        // Start the song
        musicSource.Play();
    }

    void Update()
    {
        if (nextBeatIndex >= beatMap.Count) return;

        // Check if current song time reached next beat
        if (musicSource.time >= beatMap[nextBeatIndex].time)
        {
            SpawnStar();
            nextBeatIndex++;
        }
    }

    void SpawnStar()
    {
        if (spawnPoints.Length == 0) return;

        // Pick next spawn point
        int index = Random.Range(0, spawnPoints.Length);
        starObject.transform.position = spawnPoints[index].position;

        // Set material
        if (spawnMaterial != null && objectRenderer != null)
            objectRenderer.material = spawnMaterial;

        // Reset scale
        starObject.transform.localScale = originalScale;

        // Play particle
        if (spawnedParticle != null)
        {
            spawnedParticle.gameObject.SetActive(false); // Reset
            spawnedParticle.transform.position = starObject.transform.position;
            spawnedParticle.gameObject.SetActive(true);
            spawnedParticle.Play();
        }

        // Shrink over time
        StartCoroutine(ShrinkStar());
    }

    IEnumerator ShrinkStar()
    {
        float elapsed = 0f;
        while (elapsed < shrinkDuration)
        {
            float t = elapsed / shrinkDuration;
            starObject.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        starObject.transform.localScale = Vector3.zero;
    }
}
