using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// One chapter of Hanuman's journey. Edit these in the Unity Inspector.
/// </summary>
[System.Serializable]
public class RamayanaChapter
{
    public string title;
    public string subtitle;
    [Tooltip("METERS reading at which this chapter begins.")]
    public float startDistance;
    [Tooltip("Sky colour for this chapter.")]
    public Color backgroundColor = Color.white;
}

/// <summary>
/// Follows Hanuman's journey toward Lanka using the METERS counter, announces each
/// chapter with an on-screen banner, and shifts the sky colour per chapter.
/// Title/subtitle Texts are optional: if they are not assigned, a banner is built at runtime.
/// </summary>
public class chapterManager : MonoBehaviour
{
    public static chapterManager instance;

    public RamayanaChapter[] chapters;

    [Tooltip("Only used if there is no scoreManager (METERS) in the scene.")]
    public float distancePerSecond = 3f;
    [Tooltip("How long the chapter banner stays on screen.")]
    public float bannerSeconds = 3f;
    [Tooltip("Seconds the sky takes to blend into the next chapter's colour.")]
    public float skyBlendSeconds = 2.5f;

    public Text chapterTitleText;
    public Text chapterSubtitleText;
    public Text distanceText;
    [Tooltip("Camera whose background colour follows the chapter.")]
    public Camera backgroundCamera;
    [Tooltip("Font for the runtime banner.")]
    public Font bannerFont;

    [Header("Story beats and rewards")]
    [Tooltip("One character per chapter who rises past Hanuman as the chapter begins.")]
    public Sprite[] storyBeatSprites;
    [Tooltip("Gems awarded for reaching each new chapter.")]
    public int chapterGems = 50;
    [Tooltip("Show the 'catch RAM tablets' tip on a player's first few runs.")]
    public int hintRuns = 3;

    public float Distance { get; private set; }
    public int CurrentChapterIndex { get; private set; }

    private float bannerTimer;
    private float fallbackDistance;
    private Color skyTarget;
    private bool hasSkyTarget;

    private void Reset()
    {
        chapters = DefaultChapters();
    }

    private void Awake()
    {
        instance = this;
        if (chapters == null || chapters.Length == 0)
        {
            chapters = DefaultChapters();
        }
    }

    private void Start()
    {
        if (chapterTitleText == null) BuildBanner();
        CurrentChapterIndex = -1;
        ApplyChapter(0);

        // Teach the core loop on the first few runs.
        int runs = PlayerPrefs.GetInt("RunsStarted", 0);
        PlayerPrefs.SetInt("RunsStarted", runs + 1);
        if (runs < hintRuns) Invoke(nameof(ShowEnergyHint), bannerSeconds + 1f);
        if (backgroundCamera != null && hasSkyTarget) backgroundCamera.backgroundColor = skyTarget;
    }

    private void Update()
    {
        // The METERS counter (scoreManager.score) drives the story, so goals match what the player sees.
        if (scoreManager.instance != null)
        {
            Distance = scoreManager.instance.score;
        }
        else
        {
            fallbackDistance += distancePerSecond * Time.deltaTime;
            Distance = fallbackDistance;
        }

        if (distanceText != null)
        {
            distanceText.text = Mathf.FloorToInt(Distance) + " m";
        }

        int target = CurrentChapterIndex;
        while (target + 1 < chapters.Length && Distance >= chapters[target + 1].startDistance)
        {
            target++;
        }
        if (target != CurrentChapterIndex)
        {
            ApplyChapter(target);
        }

        if (backgroundCamera != null && hasSkyTarget)
        {
            backgroundCamera.backgroundColor = Color.Lerp(backgroundCamera.backgroundColor, skyTarget,
                Time.deltaTime / Mathf.Max(0.01f, skyBlendSeconds) * 3f);
        }

        // Game over freezes time; don't leave a banner over the Game Over screen.
        if (Time.timeScale == 0f && bannerTimer > 0f)
        {
            bannerTimer = 0f;
            SetBanner("", "");
        }

        if (bannerTimer > 0f)
        {
            bannerTimer -= Time.deltaTime;
            if (bannerTimer <= 0f)
            {
                SetBanner("", "");
            }
        }
    }

    /// <summary>Shows a two-line banner (used for chapters and milestones).</summary>
    public void ShowBanner(string title, string subtitle, float seconds)
    {
        SetBanner(title, subtitle);
        bannerTimer = seconds;
    }

    /// <summary>Index of the chapter that starts at or before the given METERS reading.</summary>
    public static int ChapterAt(RamayanaChapter[] list, float meters)
    {
        int index = 0;
        while (index + 1 < list.Length && meters >= list[index + 1].startDistance) index++;
        return index;
    }

    private void ApplyChapter(int index)
    {
        if (index < 0 || index >= chapters.Length) return;

        CurrentChapterIndex = index;
        RamayanaChapter chapter = chapters[index];

        string line = index < StoryLines.Length ? StoryLines[index] : chapter.subtitle;
        if (index == 0 && PlayerPrefs.GetInt("RunsStarted", 0) <= hintRuns) line = "Drag left or right to steer Hanuman";
        if (index > 0 && chapterGems > 0)
        {
            ramayanaRunEvents.AwardGems(chapterGems);
            line += "  ·  +" + chapterGems + " gems";
        }
        ShowBanner(chapter.title, line, bannerSeconds);
        SpawnStoryBeat(index);

        skyTarget = chapter.backgroundColor;
        skyTarget.a = 0f;
        hasSkyTarget = true;

        if (PlayerPrefs.GetInt("BestChapter", 0) < index)
        {
            PlayerPrefs.SetInt("BestChapter", index);
        }
    }

    private void ShowEnergyHint()
    {
        if (Time.timeScale == 0f) return;
        ShowBanner("Catch the orange RAM tablets", "They refill your Rama Naam energy", 3.5f);
    }

    /// <summary>What happens as each chapter begins (who speaks or appears).</summary>
    public static readonly string[] StoryLines =
    {
        "Jambavan: \"Remember your strength, Hanuman!\"",
        "The vanaras cheer as you leap the ocean",
        "Lanka's guards keep watch",
        "Sita waits in Ashoka Vatika",
        "Ravana's warriors charge!",
        "Angada: \"Lanka burns! Fly on!\"",
        "Jambavan: \"Bring the Sanjeevani herb for Lakshmana\"",
        "Rama and Sita are reunited. Jai Shri Ram!",
        "The vanara army flies with you",
    };

    // The chapter's character rises past Hanuman on the left (scenery only: no collider).
    private void SpawnStoryBeat(int index)
    {
        if (storyBeatSprites == null || index >= storyBeatSprites.Length || storyBeatSprites[index] == null) return;
        Camera cam = backgroundCamera != null ? backgroundCamera : Camera.main;
        if (cam == null) return;
        Sprite sprite = storyBeatSprites[index];
        const float height = 2.6f;
        float halfW = Mathf.Min(cam.orthographicSize * cam.aspect, 4f);
        GameObject go = new GameObject("Story " + sprite.name);
        go.transform.position = new Vector3(cam.transform.position.x - halfW * 0.55f,
                                            cam.transform.position.y - cam.orthographicSize - height, 0f);
        float scale = height / Mathf.Max(0.01f, sprite.bounds.size.y);
        go.transform.localScale = new Vector3(scale, scale, 1f);
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = 4;
        ramayanaHazardMover mover = go.AddComponent<ramayanaHazardMover>();
        mover.riseSpeed = 5f;
        Destroy(go, (cam.orthographicSize * 2f + height * 2f) / mover.riseSpeed + 1f);
    }

    private void SetBanner(string title, string subtitle)
    {
        if (chapterTitleText != null) chapterTitleText.text = title;
        if (chapterSubtitleText != null) chapterSubtitleText.text = subtitle;
    }

    // Overlay banner just below the METERS heading.
    private void BuildBanner()
    {
        GameObject canvasGo = new GameObject("ChapterBanner", typeof(Canvas), typeof(CanvasScaler));
        Canvas canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;
        CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(800, 600);
        scaler.matchWidthOrHeight = 0f;

        // About 40% down the screen: below Hanuman and the METERS count on any phone shape.
        chapterTitleText = BannerLine(canvasGo.transform, "Title", 64, new Color(1f, 0.85f, 0.35f), 35f);
        chapterSubtitleText = BannerLine(canvasGo.transform, "Subtitle", 34, Color.white, -35f);
    }

    private Text BannerLine(Transform parent, string name, int size, Color color, float y)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        RectTransform rt = (RectTransform)go.transform;
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.6f);
        rt.anchoredPosition = new Vector2(0f, y);
        rt.sizeDelta = new Vector2(760f, size * 1.6f);
        Text t = go.AddComponent<Text>();
        t.font = bannerFont != null ? bannerFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = size;
        t.color = color;
        t.alignment = TextAnchor.MiddleCenter;
        // Shrink long chapter names to fit the screen width.
        t.resizeTextForBestFit = true;
        t.resizeTextMinSize = size / 2;
        t.resizeTextMaxSize = size;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Truncate;
        t.raycastTarget = false;
        Outline o = go.AddComponent<Outline>();
        o.effectColor = new Color(0.16f, 0.07f, 0.03f, 0.95f);
        o.effectDistance = new Vector2(3f, -3f);
        Shadow s = go.AddComponent<Shadow>();
        s.effectColor = new Color(0f, 0f, 0f, 0.5f);
        s.effectDistance = new Vector2(4f, -5f);
        return t;
    }

    /// <summary>
    /// The levels of Hanuman's journey, in METERS. "Infinite Ram" is the endless stage after Victory.
    /// </summary>
    public static RamayanaChapter[] DefaultChapters()
    {
        return new RamayanaChapter[]
        {
            new RamayanaChapter { title = "Hanuman Leaves in Search of Sita", subtitle = "Carry Rama's ring",             startDistance = 0f,    backgroundColor = new Color(0.62f, 0.82f, 0.58f) },
            new RamayanaChapter { title = "The Great Leap",                   subtitle = "Across the ocean to Lanka",     startDistance = 150f,  backgroundColor = new Color(0.45f, 0.70f, 0.90f) },
            new RamayanaChapter { title = "The Golden City of Ravana",        subtitle = "Lanka",                         startDistance = 400f,  backgroundColor = new Color(0.92f, 0.76f, 0.42f) },
            new RamayanaChapter { title = "The Search for Sita",              subtitle = "Ashoka Vatika",                 startDistance = 700f,  backgroundColor = new Color(0.55f, 0.80f, 0.62f) },
            new RamayanaChapter { title = "Rakshasa Battle",                  subtitle = "Ravana's warriors attack",      startDistance = 1000f, backgroundColor = new Color(0.72f, 0.42f, 0.42f) },
            new RamayanaChapter { title = "Lanka Burns",                      subtitle = "Hanuman's tail sets Lanka ablaze", startDistance = 1400f, backgroundColor = new Color(0.95f, 0.55f, 0.30f) },
            new RamayanaChapter { title = "The Mountain of Healing Herbs",    subtitle = "Sanjeevani for Lakshmana",      startDistance = 1800f, backgroundColor = new Color(0.52f, 0.48f, 0.72f) },
            new RamayanaChapter { title = "Victory",                          subtitle = "Jai Shri Ram",                  startDistance = 2300f, backgroundColor = new Color(1.00f, 0.84f, 0.55f) },
            new RamayanaChapter { title = "Infinite Ram",                     subtitle = "Keep chanting, keep flying",    startDistance = 2500f, backgroundColor = new Color(0.98f, 0.70f, 0.40f) },
        };
    }
}
