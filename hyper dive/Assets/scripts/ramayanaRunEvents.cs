using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Small shared helpers for a run: gem rewards, remembering what hurt Hanuman last,
/// and telling the player why the run ended on the Game Over screen.
/// </summary>
public static class ramayanaRunEvents
{
    public const string EnergyRanOut = "Your Rama Naam energy ran out";
    public const string Spikes = "The spikes of Patala Lok got you";
    public const string Asuras = "The Asuras got you";

    private static string lastCause = EnergyRanOut;
    private static GameObject causeLabel;

    /// <summary>Records what just took Hanuman's energy.</summary>
    public static void Hurt(string cause)
    {
        if (!string.IsNullOrEmpty(cause)) lastCause = cause;
    }

    /// <summary>Gives the player gems (saved immediately, shown in the HUD).</summary>
    public static void AwardGems(int amount)
    {
        levelManager lm = Object.FindObjectOfType<levelManager>();
        if (lm != null) lm.AddCoins(amount);
        else PlayerPrefs.SetInt("GemAmount", PlayerPrefs.GetInt("GemAmount") + amount);
    }

    /// <summary>Shows the cause (and a tip) at the top of the Game Over screen.</summary>
    public static void ShowDeathCause(GameObject gameOverPanel)
    {
        if (causeLabel != null) return;
        Font font = chapterManager.instance != null && chapterManager.instance.bannerFont != null
            ? chapterManager.instance.bannerFont
            : (gameOverPanel != null && gameOverPanel.GetComponentInChildren<Text>(true) != null
                ? gameOverPanel.GetComponentInChildren<Text>(true).font
                : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));

        // Its own full-width overlay, above the Game Over panel.
        causeLabel = new GameObject("DeathCause", typeof(Canvas), typeof(CanvasScaler));
        Canvas canvas = causeLabel.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        CanvasScaler scaler = causeLabel.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(800, 600);
        scaler.matchWidthOrHeight = 0f;

        string tip = lastCause == EnergyRanOut
            ? "Tip: catch the orange RAM tablets to refill your energy"
            : "Tip: steer through the gaps and catch RAM tablets for energy";
        // Cause at the top (above METERS), tip along the bottom (below QUIT).
        AddLine(causeLabel.transform, lastCause, font, 30, new Color(1f, 0.82f, 0.35f), top: true);
        AddLine(causeLabel.transform, tip, font, 22, new Color(1f, 0.95f, 0.85f, 0.9f), top: false);
    }

    private static void AddLine(Transform parent, string text, Font font, int size, Color color, bool top)
    {
        GameObject go = new GameObject("Line", typeof(RectTransform));
        RectTransform rt = (RectTransform)go.transform;
        rt.SetParent(parent, false);
        float y = top ? 1f : 0f;
        rt.anchorMin = new Vector2(0f, y); rt.anchorMax = new Vector2(1f, y);
        rt.pivot = new Vector2(0.5f, y);
        rt.anchoredPosition = new Vector2(0f, top ? -14f : 6f);
        rt.sizeDelta = new Vector2(-40f, size * 1.7f);
        Text t = go.AddComponent<Text>();
        t.text = text;
        t.font = font;
        t.fontSize = size;
        t.color = color;
        t.alignment = TextAnchor.MiddleCenter;
        t.resizeTextForBestFit = true;
        t.resizeTextMinSize = 14;
        t.resizeTextMaxSize = size;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Truncate;
        t.raycastTarget = false;
        Outline o = go.AddComponent<Outline>();
        o.effectColor = new Color(0.1f, 0.04f, 0.02f, 0.95f);
        o.effectDistance = new Vector2(2f, -2f);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void ResetForRun()
    {
        lastCause = EnergyRanOut;
        causeLabel = null;
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += (s, m) => { lastCause = EnergyRanOut; causeLabel = null; };
    }
}
