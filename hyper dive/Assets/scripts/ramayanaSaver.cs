using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Makes progress stick. In a browser, PlayerPrefs only reach storage when saved,
/// so gems, best distance, avatars and milestones could be lost when the tab closed.
/// Saves every few seconds, on every scene change, and when the page is hidden or closed.
/// Installs itself at startup (no scene setup needed).
/// </summary>
public class ramayanaSaver : MonoBehaviour
{
    private const float Interval = 3f;
    private float next;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Install()
    {
        var go = new GameObject("ProgressSaver");
        DontDestroyOnLoad(go);
        go.AddComponent<ramayanaSaver>();
    }

    private void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    private void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }
    private void OnSceneLoaded(Scene s, LoadSceneMode m) { PlayerPrefs.Save(); }

    private void Update()
    {
        if (Time.unscaledTime < next) return;
        next = Time.unscaledTime + Interval;
        PlayerPrefs.Save();
    }

    private void OnApplicationPause(bool paused) { if (paused) PlayerPrefs.Save(); }
    private void OnApplicationFocus(bool focused) { if (!focused) PlayerPrefs.Save(); }
    private void OnApplicationQuit() { PlayerPrefs.Save(); }
}
