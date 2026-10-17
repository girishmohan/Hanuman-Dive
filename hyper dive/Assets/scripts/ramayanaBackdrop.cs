using UnityEngine;

/// <summary>Scenery for one chapter: which silhouettes drift past, and their tint.</summary>
[System.Serializable]
public class ChapterScenery
{
    public string chapter;
    public Sprite[] silhouettes;
    public Color tint = new Color(1f, 0.95f, 0.85f);
}

/// <summary>
/// Ramayana scenery drifting up behind the canyon walls while Hanuman falls, matched
/// to the current chapter (palms in Kishkindha, waves on the ocean, towers in Lanka,
/// flames when Lanka burns...). Soft light silhouettes, rising slower than the walls.
/// </summary>
public class ramayanaBackdrop : MonoBehaviour
{
    public Camera gameCamera;
    [Tooltip("Used when there is no chapter set for the current chapter.")]
    public Sprite[] silhouettes;
    [Tooltip("One entry per chapter, in chapter order.")]
    public ChapterScenery[] chapterScenery;
    public float secondsBetween = 0.9f;
    public float riseSpeed = 3.5f;
    public Vector2 heightRange = new Vector2(3f, 7f);
    [Range(0f, 1f)] public float opacity = 0.22f;
    public Color tint = new Color(1f, 0.95f, 0.85f);
    [Tooltip("Behind the canyon walls (they use -2..0).")]
    public int sortingOrder = -3;

    private float timer;

    private void Start()
    {
        if (gameCamera == null)
        {
            GameObject tagged = GameObject.FindGameObjectWithTag("AniCam");
            gameCamera = tagged != null ? tagged.GetComponent<Camera>() : Camera.main;
        }
        // Fill the screen at the start instead of waiting for pieces to rise into view.
        if (gameCamera != null)
        {
            float top = gameCamera.transform.position.y + gameCamera.orthographicSize;
            for (float y = BottomY(); y < top; y += riseSpeed * secondsBetween)
            {
                Spawn(y);
            }
        }
    }

    private void Update()
    {
        if (gameCamera == null) return;
        timer -= Time.deltaTime;
        if (timer > 0f) return;
        timer = secondsBetween * Random.Range(0.7f, 1.3f);
        Spawn(BottomY());
    }

    private float BottomY()
    {
        return gameCamera.transform.position.y - gameCamera.orthographicSize - heightRange.y * 0.6f;
    }

    private void Spawn(float y)
    {
        Sprite[] pool = silhouettes;
        Color chapterTint = tint;
        int chapter = chapterManager.instance != null ? Mathf.Max(0, chapterManager.instance.CurrentChapterIndex) : 0;
        if (chapterScenery != null && chapter < chapterScenery.Length
            && chapterScenery[chapter].silhouettes != null && chapterScenery[chapter].silhouettes.Length > 0)
        {
            pool = chapterScenery[chapter].silhouettes;
            chapterTint = chapterScenery[chapter].tint;
        }
        if (pool == null || pool.Length == 0) return;
        Sprite sprite = pool[Random.Range(0, pool.Length)];
        float halfWidth = gameCamera.orthographicSize * gameCamera.aspect;

        GameObject go = new GameObject("Backdrop " + sprite.name);
        go.transform.position = new Vector3(
            gameCamera.transform.position.x + Random.Range(-halfWidth, halfWidth), y, 0f);
        float height = Random.Range(heightRange.x, heightRange.y);
        float scale = height / Mathf.Max(0.01f, sprite.bounds.size.y);
        go.transform.localScale = new Vector3(Random.value < 0.5f ? -scale : scale, scale, 1f);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = sortingOrder;
        Color c = chapterTint;
        c.a = opacity * Random.Range(0.6f, 1f);
        sr.color = c;

        ramayanaHazardMover mover = go.AddComponent<ramayanaHazardMover>();
        mover.riseSpeed = riseSpeed * Random.Range(0.85f, 1.15f);
        float travel = gameCamera.orthographicSize * 2f + heightRange.y * 2f;
        Destroy(go, travel / mover.riseSpeed + 1f);
    }
}
