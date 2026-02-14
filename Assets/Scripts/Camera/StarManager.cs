using UnityEngine;
using System.Collections;

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
    public float bpm = 120f;
    public float startOffset = 0f;

    private Transform[] spawnPoints;
    private Vector3 originalScale;
    private float beatInterval;
    private float nextBeatTime;

    void Start()
    {
        if (starObject == null || musicSource == null || bpm <= 0f) return;
        GameObject[] spawns = GameObject.FindGameObjectsWithTag(spawnTag);
        spawnPoints = new Transform[spawns.Length];
        for (int i = 0; i < spawns.Length; i++)
            spawnPoints[i] = spawns[i].transform;

        originalScale = starObject.transform.localScale;
        beatInterval = 60f / bpm;
        nextBeatTime = startOffset;
        musicSource.Play();
    }

    void Update()
    {
        if (musicSource == null) return;
        while (musicSource.time >= nextBeatTime)
        {
            SpawnStar();
            nextBeatTime += beatInterval;
        }
    }

    void SpawnStar()
    {
        if (spawnPoints.Length == 0) return;
        int index = Random.Range(0, spawnPoints.Length);
        Vector3 pos = spawnPoints[index].position;
        GameObject newStar = Instantiate(starObject, pos, starObject.transform.rotation);
        Renderer r = newStar.GetComponent<Renderer>();
        if (spawnMaterial != null && r != null) r.material = spawnMaterial;
        newStar.transform.localScale = originalScale;
        if (particlePrefab != null)
        {
            GameObject p = Instantiate(particlePrefab, newStar.transform.position, Quaternion.identity);
            ParticleSystem ps = p.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();
        }
        StartCoroutine(ShrinkStar(newStar.transform));
    }

    IEnumerator ShrinkStar(Transform target)
    {
        float elapsed = 0f;
        while (elapsed < shrinkDuration)
        {
            float t = elapsed / shrinkDuration;
            target.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        target.localScale = Vector3.zero;
    }
}
