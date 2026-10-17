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
    [Tooltip("Distance (in game units) at which this chapter begins.")]
    public float startDistance;
    [Tooltip("Placeholder sky/background tint for this chapter.")]
    public Color backgroundColor = Color.white;
}

/// <summary>
/// Tracks how far Hanuman has travelled toward Lanka and announces chapter changes.
/// Attach to an empty GameObject in the gameplay scene, then assign the UI Text
/// fields (all optional) in the Inspector.
/// </summary>
public class chapterManager : MonoBehaviour
{
    public static chapterManager instance;

    public RamayanaChapter[] chapters;

    [Tooltip("Distance gained per second of play. Tune for pacing.")]
    public float distancePerSecond = 3f;
    [Tooltip("How long the chapter banner stays on screen.")]
    public float bannerSeconds = 3f;

    public Text chapterTitleText;
    public Text chapterSubtitleText;
    public Text distanceText;
    [Tooltip("Optional: camera whose background colour follows the chapter.")]
    public Camera backgroundCamera;

    public float Distance { get; private set; }
    public int CurrentChapterIndex { get; private set; }

    private float bannerTimer;

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
        CurrentChapterIndex = -1;
        ApplyChapter(0);
    }

    private void Update()
    {
        // Time.timeScale is set to 0 on game over, so distance stops automatically.
        Distance += distancePerSecond * Time.deltaTime;

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

        if (bannerTimer > 0f)
        {
            bannerTimer -= Time.deltaTime;
            if (bannerTimer <= 0f)
            {
                SetBanner("", "");
            }
        }
    }

    private void ApplyChapter(int index)
    {
        if (index < 0 || index >= chapters.Length) return;

        CurrentChapterIndex = index;
        RamayanaChapter chapter = chapters[index];

        SetBanner(chapter.title, chapter.subtitle);
        bannerTimer = bannerSeconds;

        if (backgroundCamera != null)
        {
            backgroundCamera.backgroundColor = chapter.backgroundColor;
        }

        if (PlayerPrefs.GetInt("BestChapter", 0) < index)
        {
            PlayerPrefs.SetInt("BestChapter", index);
        }
    }

    private void SetBanner(string title, string subtitle)
    {
        if (chapterTitleText != null) chapterTitleText.text = title;
        if (chapterSubtitleText != null) chapterSubtitleText.text = subtitle;
    }

    /// <summary>
    /// Starting chapter list following the Ramayana. Distances are placeholders to tune.
    /// </summary>
    public static RamayanaChapter[] DefaultChapters()
    {
        return new RamayanaChapter[]
        {
            new RamayanaChapter { title = "Kishkindha",      subtitle = "Hanuman takes Rama's ring",       startDistance = 0f,    backgroundColor = new Color(0.55f, 0.80f, 0.55f) },
            new RamayanaChapter { title = "The Great Leap",  subtitle = "Across the ocean to Lanka",       startDistance = 150f,  backgroundColor = new Color(0.40f, 0.65f, 0.90f) },
            new RamayanaChapter { title = "Lanka",           subtitle = "The golden city of Ravana",       startDistance = 400f,  backgroundColor = new Color(0.85f, 0.65f, 0.30f) },
            new RamayanaChapter { title = "Ashoka Vatika",   subtitle = "The search for Sita",             startDistance = 700f,  backgroundColor = new Color(0.45f, 0.75f, 0.60f) },
            new RamayanaChapter { title = "Rakshasa Battle", subtitle = "Hanuman faces Ravana's warriors", startDistance = 1000f, backgroundColor = new Color(0.60f, 0.25f, 0.25f) },
            new RamayanaChapter { title = "Lanka Burns",     subtitle = "Fire sweeps through the city",    startDistance = 1400f, backgroundColor = new Color(0.90f, 0.40f, 0.15f) },
            new RamayanaChapter { title = "Sanjeevani",      subtitle = "The mountain of healing herbs",   startDistance = 1800f, backgroundColor = new Color(0.35f, 0.30f, 0.55f) },
            new RamayanaChapter { title = "Victory",         subtitle = "Jai Shri Ram",                    startDistance = 2300f, backgroundColor = new Color(1.00f, 0.85f, 0.45f) },
        };
    }
}
