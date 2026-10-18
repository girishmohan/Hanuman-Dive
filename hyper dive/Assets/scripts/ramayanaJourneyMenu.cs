using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Main-menu view of Hanuman's goals: a "Next goal" line under the title and a
/// JOURNEY button that opens the list of chapters, their distances and milestones,
/// marking what the player has already reached or collected.
/// Builds its UI at runtime on the start screen's canvas.
/// </summary>
public class ramayanaJourneyMenu : MonoBehaviour
{
    public Font font;
    [Tooltip("One icon per milestone, in chapter order.")]
    public Sprite[] milestoneIcons;
    [Tooltip("Line under the title that shows the next goal.")]
    public Text nextGoalText;

    private static readonly Color Gold = new Color(1f, 0.82f, 0.30f);
    private static readonly Color Cream = new Color(1f, 0.95f, 0.85f);
    private static readonly Color Dim = new Color(1f, 1f, 1f, 0.45f);
    private static readonly Color Done = new Color(0.55f, 0.95f, 0.55f);

    private GameObject panel;

    private void Start()
    {
        RamayanaChapter[] chapters = chapterManager.DefaultChapters();
        RamayanaMilestone[] milestones = ramayanaMilestones.Defaults();
        float best = PlayerPrefs.GetFloat("Highscore", 0f);

        if (nextGoalText != null)
        {
            int next = -1;
            for (int i = 0; i < chapters.Length; i++)
            {
                if (chapters[i].startDistance > best) { next = i; break; }
            }
            nextGoalText.text = next >= 0
                ? "Next goal: " + chapters[next].title + " · " + chapters[next].startDistance + " m"
                : "Every chapter reached · Jai Shri Ram!";
        }

        BuildButton();
        panel = BuildPanel(chapters, milestones, best);
        panel.SetActive(false);
    }

    public void Open() { panel.SetActive(true); panel.transform.SetAsLastSibling(); }
    public void Close() { panel.SetActive(false); }

    private void BuildButton()
    {
        RectTransform rt = NewRect("JourneyButton", transform);
        rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(28f, -40f);
        rt.sizeDelta = new Vector2(220f, 76f);
        Image bg = rt.gameObject.AddComponent<Image>();
        bg.color = new Color(0.30f, 0.12f, 0.03f, 0.75f);
        Button b = rt.gameObject.AddComponent<Button>();
        b.targetGraphic = bg;
        b.onClick.AddListener(Open);
        Label(rt, "JOURNEY", 40, Gold, TextAnchor.MiddleCenter);
    }

    private GameObject BuildPanel(RamayanaChapter[] chapters, RamayanaMilestone[] milestones, float best)
    {
        RectTransform root = NewRect("JourneyPanel", transform);
        Stretch(root);
        Image shade = root.gameObject.AddComponent<Image>();
        shade.color = new Color(0.08f, 0.04f, 0.02f, 0.97f);

        int done = 0;
        for (int i = 0; i < milestones.Length; i++) if (ramayanaMilestones.IsCompleted(i)) done++;

        Text heading = Label(Place(root, 0f, -120f, 760f, 90f), "THE JOURNEY", 66, Gold, TextAnchor.MiddleCenter);
        heading.fontStyle = FontStyle.Bold;
        Label(Place(root, 0f, -195f, 760f, 50f),
              "Best: " + Mathf.FloorToInt(best) + " m   ·   Milestones " + done + "/" + milestones.Length,
              30, Cream, TextAnchor.MiddleCenter);

        const float rowHeight = 118f;
        for (int i = 0; i < chapters.Length; i++)
        {
            bool reached = best >= chapters[i].startDistance;
            int mIndex = System.Array.FindIndex(milestones, m => m.chapter == i);
            bool collected = mIndex >= 0 && ramayanaMilestones.IsCompleted(mIndex);
            float y = -270f - i * rowHeight;

            RectTransform row = Place(root, 0f, y, 720f, rowHeight - 12f);
            Image rowBg = row.gameObject.AddComponent<Image>();
            rowBg.color = reached ? new Color(1f, 0.85f, 0.5f, 0.14f) : new Color(1f, 1f, 1f, 0.05f);

            if (mIndex >= 0 && milestoneIcons != null && mIndex < milestoneIcons.Length && milestoneIcons[mIndex] != null)
            {
                RectTransform iconRt = NewRect("Icon", row);
                iconRt.anchorMin = iconRt.anchorMax = new Vector2(0f, 0.5f);
                iconRt.pivot = new Vector2(0f, 0.5f);
                iconRt.anchoredPosition = new Vector2(12f, 0f);
                iconRt.sizeDelta = new Vector2(104f, 104f);
                Image icon = iconRt.gameObject.AddComponent<Image>();
                icon.sprite = milestoneIcons[mIndex];
                icon.preserveAspect = true;
                icon.color = collected || reached ? Color.white : new Color(1f, 1f, 1f, 0.35f);
            }

            RectTransform top = NewRect("Chapter", row);
            top.anchorMin = new Vector2(0f, 0.5f); top.anchorMax = new Vector2(1f, 1f);
            top.offsetMin = new Vector2(132f, 0f); top.offsetMax = new Vector2(-16f, -6f);
            Label(top, (i + 1) + ". " + chapters[i].title + "  ·  " + chapters[i].startDistance + " m",
                  36, reached ? Gold : Dim, TextAnchor.MiddleLeft);

            RectTransform bottom = NewRect("Milestone", row);
            bottom.anchorMin = new Vector2(0f, 0f); bottom.anchorMax = new Vector2(1f, 0.5f);
            bottom.offsetMin = new Vector2(132f, 6f); bottom.offsetMax = new Vector2(-16f, 0f);
            string goal = mIndex >= 0 ? milestones[mIndex].title + ": " + milestones[mIndex].story : chapters[i].subtitle;
            Label(bottom, collected ? "DONE  " + goal : goal, 26, collected ? Done : (reached ? Cream : Dim), TextAnchor.MiddleLeft);
        }

        RectTransform close = NewRect("Close", root);
        close.anchorMin = close.anchorMax = new Vector2(0.5f, 0f);
        close.anchoredPosition = new Vector2(0f, 110f);
        close.sizeDelta = new Vector2(260f, 84f);
        Image closeBg = close.gameObject.AddComponent<Image>();
        closeBg.color = new Color(0.95f, 0.55f, 0.15f, 0.95f);
        Button closeButton = close.gameObject.AddComponent<Button>();
        closeButton.targetGraphic = closeBg;
        closeButton.onClick.AddListener(Close);
        Label(close, "CLOSE", 42, Color.white, TextAnchor.MiddleCenter);

        return root.gameObject;
    }

    private RectTransform Place(RectTransform parent, float x, float y, float w, float h)
    {
        RectTransform rt = NewRect("Item", parent);
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(x, y);
        rt.sizeDelta = new Vector2(w, h);
        return rt;
    }

    private static RectTransform NewRect(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return (RectTransform)go.transform;
    }

    private static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
    }

    private Text Label(RectTransform parent, string text, int size, Color color, TextAnchor align)
    {
        RectTransform rt = NewRect("Text", parent);
        Stretch(rt);
        Text t = rt.gameObject.AddComponent<Text>();
        t.text = text;
        t.font = font != null ? font : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = size;
        t.color = color;
        t.alignment = align;
        // Shrink to fit so long chapter names stay on their row.
        t.resizeTextForBestFit = true;
        t.resizeTextMinSize = Mathf.Max(14, size / 2);
        t.resizeTextMaxSize = size;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Truncate;
        t.raycastTarget = false;
        Outline o = rt.gameObject.AddComponent<Outline>();
        o.effectColor = new Color(0.12f, 0.05f, 0.02f, 0.9f);
        o.effectDistance = new Vector2(1.5f, -1.5f);
        return t;
    }
}
