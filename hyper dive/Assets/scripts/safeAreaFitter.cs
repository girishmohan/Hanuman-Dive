using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Keeps screen-space UI clear of notches, the Dynamic Island and the home bar on any
/// device, while the game itself still draws edge to edge.
/// Installs itself at startup (no scene setup needed): in every scene, UI elements
/// anchored to the top of the screen move down by the top safe-area inset, and
/// elements anchored to the bottom move up by the bottom inset.
/// </summary>
public class safeAreaFitter : MonoBehaviour
{
    private readonly Dictionary<RectTransform, Vector2> originalPositions = new Dictionary<RectTransform, Vector2>();
    private Rect lastSafeArea;
    private Vector2Int lastScreenSize;
    private float lastScale;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Install()
    {
        GameObject go = new GameObject("SafeAreaFitter");
        DontDestroyOnLoad(go);
        go.AddComponent<safeAreaFitter>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        originalPositions.Clear();
        lastScale = -1f; // force a re-apply once canvases have scaled
    }

    // LateUpdate so CanvasScaler has already set this frame's scale.
    private void LateUpdate()
    {
        float scale = FirstCanvasScale();
        Vector2Int size = new Vector2Int(Screen.width, Screen.height);
        if (Screen.safeArea == lastSafeArea && size == lastScreenSize && Mathf.Approximately(scale, lastScale))
        {
            return;
        }
        lastSafeArea = Screen.safeArea;
        lastScreenSize = size;
        lastScale = scale;
        Apply();
    }

    private static float FirstCanvasScale()
    {
        foreach (Canvas c in FindObjectsOfType<Canvas>())
        {
            if (c.isRootCanvas && c.renderMode != RenderMode.WorldSpace) return c.scaleFactor;
        }
        return 1f;
    }

    private void Apply()
    {
        Rect safe = Screen.safeArea;
        float topInsetPx = Screen.height - safe.yMax;
        float bottomInsetPx = safe.yMin;

        foreach (Canvas canvas in FindObjectsOfType<Canvas>(true))
        {
            if (!canvas.isRootCanvas || canvas.renderMode == RenderMode.WorldSpace) continue;
            float scale = canvas.scaleFactor > 0f ? canvas.scaleFactor : 1f;
            FitChildren(canvas.transform, topInsetPx / scale, bottomInsetPx / scale);
        }
    }

    // Walks through full-screen containers and nudges top/bottom-anchored elements.
    private void FitChildren(Transform parent, float topInset, float bottomInset)
    {
        foreach (Transform child in parent)
        {
            RectTransform rt = child as RectTransform;
            if (rt == null) continue;

            if (IsFullScreen(rt))
            {
                FitChildren(rt, topInset, bottomInset);
                continue;
            }

            bool top = rt.anchorMin.y > 0.99f && rt.anchorMax.y > 0.99f;
            bool bottom = rt.anchorMin.y < 0.01f && rt.anchorMax.y < 0.01f;
            if (!top && !bottom) continue;

            Vector2 original;
            if (!originalPositions.TryGetValue(rt, out original))
            {
                original = rt.anchoredPosition;
                originalPositions[rt] = original;
            }
            rt.anchoredPosition = original + new Vector2(0f, top ? -topInset : bottomInset);
        }
    }

    private static bool IsFullScreen(RectTransform rt)
    {
        return rt.anchorMin == Vector2.zero && rt.anchorMax == Vector2.one
            && rt.offsetMin.sqrMagnitude < 1f && rt.offsetMax.sqrMagnitude < 1f;
    }
}
