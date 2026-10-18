using UnityEngine;

/// <summary>A story goal Hanuman completes by catching its pickup during a chapter.</summary>
[System.Serializable]
public class RamayanaMilestone
{
    public string title;
    public string story;
    public int chapter;
    [Tooltip("METERS reading at which the pickup first appears.")]
    public float appearAtMeters;
    [Tooltip("Index of a milestone that must be collected earlier in the same run (-1 = none).")]
    public int requires = -1;
    public Sprite icon;
    [Tooltip("Size multiplier for the pickup: characters (Menaka, Sita, Rama & Sita) are drawn larger than icons.")]
    public float iconScale = 1f;
}

/// <summary>
/// One milestone per chapter (Rama's ring, Menaka's blessing, Lanka's fire, Rama and Sita...).
/// Each appears once its METERS reading is reached; if Hanuman misses it, it comes
/// back a little later while the chapter lasts. Completed milestones are saved and
/// shown on the main menu's Journey panel.
/// </summary>
public class ramayanaMilestones : MonoBehaviour
{
    public static ramayanaMilestones instance;

    public RamayanaMilestone[] milestones;
    public Camera gameCamera;
    public float riseSpeed = 9f;
    public float iconSize = 1.5f;
    public int energyReward = 15;
    public float retryAfterMeters = 60f;
    [Tooltip("Half-width of the playable channel (world units).")]
    public float playAreaHalfWidth = 3.8f;
    public int sortingOrder = 7;

    private float[] nextAppear;
    private bool[] active;
    private bool[] collectedThisRun;

    public static string SaveKey(int index) { return "Milestone_" + index; }
    public static bool IsCompleted(int index) { return PlayerPrefs.GetInt(SaveKey(index), 0) == 1; }

    private void Awake()
    {
        instance = this;
        if (milestones == null || milestones.Length == 0) milestones = Defaults();
    }

    private void Start()
    {
        if (gameCamera == null)
        {
            GameObject tagged = GameObject.FindGameObjectWithTag("AniCam");
            gameCamera = tagged != null ? tagged.GetComponent<Camera>() : Camera.main;
        }
        nextAppear = new float[milestones.Length];
        active = new bool[milestones.Length];
        collectedThisRun = new bool[milestones.Length];
        for (int i = 0; i < milestones.Length; i++) nextAppear[i] = milestones[i].appearAtMeters;
    }

    private void Update()
    {
        if (gameCamera == null || nextAppear == null) return;
        float meters = scoreManager.instance != null ? scoreManager.instance.score
                     : (chapterManager.instance != null ? chapterManager.instance.Distance : 0f);
        RamayanaChapter[] chapters = chapterManager.instance != null ? chapterManager.instance.chapters : chapterManager.DefaultChapters();

        for (int i = 0; i < milestones.Length; i++)
        {
            RamayanaMilestone m = milestones[i];
            if (collectedThisRun[i] || active[i] || meters < nextAppear[i]) continue;
            if (m.requires >= 0 && !collectedThisRun[m.requires]) continue;
            // Only during its own chapter.
            if (chapterManager.ChapterAt(chapters, meters) != m.chapter) continue;
            Spawn(i);
        }
    }

    private void Spawn(int index)
    {
        active[index] = true;
        RamayanaMilestone m = milestones[index];
        float halfWidth = Mathf.Min(gameCamera.orthographicSize * gameCamera.aspect, playAreaHalfWidth);
        float x = gameCamera.transform.position.x + Random.Range(-halfWidth, halfWidth) * 0.7f;
        float size = iconSize * Mathf.Max(0.1f, m.iconScale);
        float y = gameCamera.transform.position.y - gameCamera.orthographicSize - size;

        GameObject root = new GameObject("Milestone " + m.title);
        root.transform.position = new Vector3(x, y, 0f);
        ramayanaHazardMover mover = root.AddComponent<ramayanaHazardMover>();
        mover.riseSpeed = riseSpeed;

        GameObject body = new GameObject("Icon");
        body.transform.SetParent(root.transform, false);
        if (m.icon != null)
        {
            float scale = size / Mathf.Max(0.01f, m.icon.bounds.size.y);
            body.transform.localScale = new Vector3(scale, scale, 1f);
            SpriteRenderer sr = body.AddComponent<SpriteRenderer>();
            sr.sprite = m.icon;
            sr.sortingOrder = sortingOrder;
            CircleCollider2D c = body.AddComponent<CircleCollider2D>();
            c.isTrigger = true;
            c.radius = m.icon.bounds.size.x * 0.45f;
        }
        ramayanaMilestonePickup pickup = body.AddComponent<ramayanaMilestonePickup>();
        pickup.index = index;
        pickup.root = root;

        float travel = gameCamera.orthographicSize * 2f + size * 3f;
        ramayanaMilestoneExpiry expiry = root.AddComponent<ramayanaMilestoneExpiry>();
        expiry.index = index;
        expiry.seconds = travel / riseSpeed + 0.5f;
    }

    public void Collected(int index)
    {
        active[index] = false;
        collectedThisRun[index] = true;
        PlayerPrefs.SetInt(SaveKey(index), 1);
        move hanuman = FindObjectOfType<move>();
        if (hanuman != null) hanuman.AddEnergy(energyReward);
        if (chapterManager.instance != null)
        {
            chapterManager.instance.ShowBanner("Milestone: " + milestones[index].title, milestones[index].story, 3f);
        }
    }

    public void Missed(int index)
    {
        active[index] = false;
        float meters = scoreManager.instance != null ? scoreManager.instance.score : 0f;
        nextAppear[index] = meters + retryAfterMeters;
    }

    /// <summary>The story goals, one per chapter (chapter starts: 0, 150, 400, 700, 1000, 1400, 1800, 2300 m; Infinite Ram has none).</summary>
    public static RamayanaMilestone[] Defaults()
    {
        return new RamayanaMilestone[]
        {
            new RamayanaMilestone { title = "Rama's Ring",        story = "Take the ring to Sita",                 chapter = 0, appearAtMeters = 20 },
            new RamayanaMilestone { title = "Menaka's Blessing",  story = "A blessing for the ocean crossing",     chapter = 1, appearAtMeters = 180, iconScale = 1.6f },
            new RamayanaMilestone { title = "Lankini's Gate",     story = "Slip into the golden city",             chapter = 2, appearAtMeters = 430 },
            new RamayanaMilestone { title = "Ring for Sita",      story = "Give Rama's ring to Sita",              chapter = 3, appearAtMeters = 730, requires = 0, iconScale = 1.6f },
            new RamayanaMilestone { title = "Hanuman's Gada",     story = "Stand against Ravana's army",           chapter = 4, appearAtMeters = 1030 },
            new RamayanaMilestone { title = "Light the Fire",     story = "Set Lanka ablaze with your burning tail", chapter = 5, appearAtMeters = 1430 },
            new RamayanaMilestone { title = "Sanjeevani",         story = "Carry the healing herb to Lakshmana",   chapter = 6, appearAtMeters = 1830 },
            new RamayanaMilestone { title = "Rama and Sita Reunited", story = "Jai Shri Ram",                     chapter = 7, appearAtMeters = 2330, iconScale = 1.8f },
        };
    }
}

/// <summary>Collects the milestone when Hanuman touches it.</summary>
public class ramayanaMilestonePickup : MonoBehaviour
{
    public int index;
    public GameObject root;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (ramayanaMilestones.instance != null) ramayanaMilestones.instance.Collected(index);
        Destroy(root != null ? root : gameObject);
    }
}

/// <summary>Removes a missed milestone off-screen so it can come back later.</summary>
public class ramayanaMilestoneExpiry : MonoBehaviour
{
    public int index;
    public float seconds;

    private void Update()
    {
        seconds -= Time.deltaTime;
        if (seconds > 0f) return;
        if (ramayanaMilestones.instance != null) ramayanaMilestones.instance.Missed(index);
        Destroy(gameObject);
    }
}
