using UnityEngine;

/// <summary>
/// First Ramayana obstacle set: "The Great Leap" (Hanuman crossing the ocean to Lanka).
/// Builds placeholder shapes at runtime, so no art is needed yet:
///   Surasa's Jaws    - wall with a narrow gap (Hanuman shrinks to slip through her mouth)
///   Simhika's Shadow - dark orb that drifts toward Hanuman (she seized his shadow)
///   Ocean Rocks      - small scattered rocks
///   Mainaka Peak     - golden pickup that restores Rama Naam energy (the mountain offered rest)
/// Spawns just below the camera; everything rises like the existing obstacles.
/// Swap the shapes for sprites later by replacing BuildPart() calls with prefabs.
/// </summary>
public class ramayanaObstacleSet : MonoBehaviour
{
    [Header("When to spawn")]
    [Tooltip("Chapter index (in chapterManager) during which this set spawns. 1 = The Great Leap.")]
    public int chapterIndex = 1;
    [Tooltip("Ignore chapters and always spawn (handy for playtesting).")]
    public bool alwaysSpawn = false;
    public float secondsBetweenSpawns = 2.5f;

    [Header("Movement")]
    public float riseSpeed = 14f;
    public float lifetime = 6f;

    [Header("Surasa's Jaws")]
    public int surasaDamage = 34;
    public float surasaGapWidth = 2.6f;
    public Color surasaColor = new Color(0.20f, 0.55f, 0.35f);

    [Header("Simhika's Shadow")]
    public int simhikaDamage = 25;
    public float simhikaRadius = 0.9f;
    public float simhikaChaseSpeed = 2.5f;
    public Color simhikaColor = new Color(0.15f, 0.10f, 0.25f);

    [Header("Ocean Rocks")]
    public int rockDamage = 10;
    public int rockCount = 3;
    public Color rockColor = new Color(0.45f, 0.45f, 0.50f);

    [Header("Mainaka Peak")]
    public int mainakaEnergy = 15;
    public Color mainakaColor = new Color(1.00f, 0.80f, 0.20f);

    public int sortingOrder = 5;

    private static Sprite squareSprite;
    private static Sprite circleSprite;
    private float timer;
    private int lastPattern = -1;

    private void Update()
    {
        if (!IsActiveChapter()) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;
        timer = secondsBetweenSpawns;

        Camera cam = Camera.main;
        if (cam == null) return;

        float halfWidth = cam.orthographicSize * cam.aspect;
        float spawnY = cam.transform.position.y - cam.orthographicSize - 1.5f;
        float centerX = cam.transform.position.x;

        // Weighted pick, never the same pattern twice in a row.
        int pattern;
        do
        {
            float r = Random.value;
            pattern = r < 0.35f ? 0 : r < 0.60f ? 1 : r < 0.85f ? 2 : 3;
        } while (pattern == lastPattern);
        lastPattern = pattern;

        switch (pattern)
        {
            case 0: SpawnSurasa(centerX, spawnY, halfWidth); break;
            case 1: SpawnSimhika(centerX, spawnY, halfWidth); break;
            case 2: SpawnRocks(centerX, spawnY, halfWidth); break;
            case 3: SpawnMainaka(centerX, spawnY, halfWidth); break;
        }
    }

    private bool IsActiveChapter()
    {
        if (alwaysSpawn || chapterManager.instance == null) return true;
        return chapterManager.instance.CurrentChapterIndex == chapterIndex;
    }

    private void SpawnSurasa(float centerX, float y, float halfWidth)
    {
        float left = centerX - halfWidth;
        float right = centerX + halfWidth;
        float gapCenter = Random.Range(left + surasaGapWidth, right - surasaGapWidth);
        float gapLeft = gapCenter - surasaGapWidth / 2f;
        float gapRight = gapCenter + surasaGapWidth / 2f;

        GameObject jaws = new GameObject("Surasa's Jaws");
        jaws.transform.position = new Vector3(0f, y, 0f);
        AddMover(jaws, 0f);

        BuildPart(jaws.transform, "Upper Jaw", false,
            new Vector2((left + gapLeft) / 2f, 0f), new Vector2(gapLeft - left, 0.6f),
            surasaColor, -surasaDamage);
        BuildPart(jaws.transform, "Lower Jaw", false,
            new Vector2((gapRight + right) / 2f, 0f), new Vector2(right - gapRight, 0.6f),
            surasaColor, -surasaDamage);
    }

    private void SpawnSimhika(float centerX, float y, float halfWidth)
    {
        float x = Random.Range(centerX - halfWidth + simhikaRadius, centerX + halfWidth - simhikaRadius);
        GameObject root = new GameObject("Simhika's Shadow");
        root.transform.position = new Vector3(x, y, 0f);
        AddMover(root, simhikaChaseSpeed);
        BuildPart(root.transform, "Shadow", true, Vector2.zero,
            Vector2.one * simhikaRadius * 2f, simhikaColor, -simhikaDamage);
    }

    private void SpawnRocks(float centerX, float y, float halfWidth)
    {
        GameObject root = new GameObject("Ocean Rocks");
        root.transform.position = new Vector3(0f, y, 0f);
        AddMover(root, 0f);
        for (int i = 0; i < rockCount; i++)
        {
            float x = Random.Range(centerX - halfWidth + 0.5f, centerX + halfWidth - 0.5f);
            float dy = Random.Range(-1.5f, 1.5f);
            float size = Random.Range(0.6f, 1.1f);
            Transform rock = BuildPart(root.transform, "Rock", false, new Vector2(x, dy),
                Vector2.one * size, rockColor, -rockDamage);
            rock.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 90f));
        }
    }

    private void SpawnMainaka(float centerX, float y, float halfWidth)
    {
        float x = Random.Range(centerX - halfWidth + 1f, centerX + halfWidth - 1f);
        GameObject root = new GameObject("Mainaka Peak");
        root.transform.position = new Vector3(x, y, 0f);
        AddMover(root, 0f);
        Transform peak = BuildPart(root.transform, "Peak", false, Vector2.zero,
            Vector2.one * 0.8f, mainakaColor, mainakaEnergy);
        peak.localRotation = Quaternion.Euler(0f, 0f, 45f);
    }

    private void AddMover(GameObject root, float chaseSpeed)
    {
        ramayanaHazardMover mover = root.AddComponent<ramayanaHazardMover>();
        mover.riseSpeed = riseSpeed;
        mover.chaseSpeed = chaseSpeed;
        Destroy(root, lifetime);
    }

    /// <summary>Creates one placeholder shape. energyChange &lt; 0 hurts, &gt; 0 heals.</summary>
    private Transform BuildPart(Transform parent, string partName, bool circle,
        Vector2 localPos, Vector2 size, Color color, int energyChange)
    {
        GameObject part = new GameObject(partName);
        part.transform.SetParent(parent, false);
        part.transform.localPosition = localPos;
        part.transform.localScale = new Vector3(size.x, size.y, 1f);

        SpriteRenderer sr = part.AddComponent<SpriteRenderer>();
        sr.sprite = circle ? CircleSprite() : SquareSprite();
        sr.color = color;
        sr.sortingOrder = sortingOrder;

        Collider2D col = circle ? (Collider2D)part.AddComponent<CircleCollider2D>()
                                : part.AddComponent<BoxCollider2D>();
        col.isTrigger = true;

        ramayanaHazardHit hit = part.AddComponent<ramayanaHazardHit>();
        hit.energyChange = energyChange;
        // Rocks vanish one at a time; other patterns disappear as a whole (e.g. both jaws).
        hit.removeOnTouch = partName == "Rock" ? part : parent.gameObject;
        return part.transform;
    }

    // 1x1-unit placeholder sprites generated once.
    private static Sprite SquareSprite()
    {
        if (squareSprite == null)
        {
            Texture2D tex = new Texture2D(4, 4);
            Color[] px = new Color[16];
            for (int i = 0; i < px.Length; i++) px[i] = Color.white;
            tex.SetPixels(px);
            tex.Apply();
            squareSprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
        }
        return squareSprite;
    }

    private static Sprite CircleSprite()
    {
        if (circleSprite == null)
        {
            const int n = 64;
            Texture2D tex = new Texture2D(n, n);
            Color[] px = new Color[n * n];
            float r = n / 2f;
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(r, r));
                    px[y * n + x] = d <= r ? Color.white : Color.clear;
                }
            tex.SetPixels(px);
            tex.Apply();
            circleSprite = Sprite.Create(tex, new Rect(0, 0, n, n), new Vector2(0.5f, 0.5f), n);
        }
        return circleSprite;
    }
}

/// <summary>Rises like the original obstacles; optionally drifts toward Hanuman.</summary>
public class ramayanaHazardMover : MonoBehaviour
{
    public float riseSpeed;
    public float chaseSpeed;
    private Transform player;

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    private void Update()
    {
        Vector3 pos = transform.position;
        pos.y += riseSpeed * Time.deltaTime;
        if (chaseSpeed > 0f && player != null)
        {
            pos.x = Mathf.MoveTowards(pos.x, player.position.x, chaseSpeed * Time.deltaTime);
        }
        transform.position = pos;
    }
}

/// <summary>Changes Rama Naam energy when Hanuman touches this part.</summary>
public class ramayanaHazardHit : MonoBehaviour
{
    public int energyChange;
    public GameObject removeOnTouch;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        move hanuman = other.GetComponent<move>();
        if (hanuman == null) return;

        hanuman.AddEnergy(energyChange);
        Destroy(removeOnTouch != null ? removeOnTouch : gameObject);
    }
}
