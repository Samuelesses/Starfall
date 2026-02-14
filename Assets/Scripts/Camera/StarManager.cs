using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StarManager : MonoBehaviour
{
    [Header("star Settings")]
    public GameObject starObject;
    public string spawnTag = "spawn";
    public float shrinkDuration = 1f;
    public Material spawnMaterial;
    public GameObject particlePrefab;
    public ScoreManager scoreManager;
    
    [Header("line")]
    public Material lineMaterial;
    public Color lineColor = Color.white;
    public float lineWidth = 0.05f;

    [Header("music settings")]
    public AudioSource musicSource;
    public float bpm = 120f;
    public float startOffset = 0f;

    private Transform[] spawnPoints;
    private Vector3 originalScale;
    private float beatInterval;
    private float nextBeatTime;
    private HashSet<int> occupiedSpawnIndices = new HashSet<int>();
    private int? nextSpawnIndex = null;
    private GameObject lastGuidedStar;

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

        List<int> available = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (!occupiedSpawnIndices.Contains(i))
                available.Add(i);
        }

        if (available.Count == 0) return;

        int index;
        if (nextSpawnIndex.HasValue && available.Contains(nextSpawnIndex.Value))
            index = nextSpawnIndex.Value;
        else
            index = available[Random.Range(0, available.Count)];
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
        occupiedSpawnIndices.Add(index);

        if (lastGuidedStar != null)
        {
            LineRenderer oldLr = lastGuidedStar.GetComponent<LineRenderer>();
            if (oldLr != null) Destroy(oldLr);
        }
        available.Clear();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (!occupiedSpawnIndices.Contains(i))
                available.Add(i);
        }

        if (available.Count > 0)
        {
            nextSpawnIndex = available[Random.Range(0, available.Count)];

            LineRenderer lr = newStar.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.useWorldSpace = true;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            if (lineMaterial != null) lr.material = lineMaterial;
            lr.startColor = lineColor;
            lr.endColor = lineColor;
            Vector3 nextPos = spawnPoints[nextSpawnIndex.Value].position;
            lr.SetPosition(0, newStar.transform.position);
            lr.SetPosition(1, nextPos);
            lastGuidedStar = newStar;
        }
        else
        {
            nextSpawnIndex = null;
            lastGuidedStar = newStar;
        }

        StartCoroutine(ShrinkStar(newStar.transform, index));
    }

    IEnumerator ShrinkStar(Transform target, int spawnIndex)
    {
        float elapsed = 0f;
        Renderer rend = target.GetComponent<Renderer>();

        while (elapsed < shrinkDuration)
        {
            float t = elapsed / shrinkDuration;
            target.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.localScale = Vector3.zero;
        occupiedSpawnIndices.Remove(spawnIndex);

        if (scoreManager != null && rend != null && rend.sharedMaterial == spawnMaterial)
        {
            scoreManager.CountMiss();
        }
    }

}
