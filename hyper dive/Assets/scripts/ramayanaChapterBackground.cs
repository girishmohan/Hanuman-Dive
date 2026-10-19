using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Painted background for each chapter (Kishkindha, the Great Leap, Lanka, Ashoka Vatika...),
/// filling the screen behind the canyon walls and cross-fading when a new chapter begins.
/// Chapters without a painting fade back to the chapter's sky colour.
/// </summary>
public class ramayanaChapterBackground : MonoBehaviour
{
    public Camera gameCamera;
    [Tooltip("One painting per chapter, in chapter order. Leave a slot empty to show the sky colour.")]
    public Sprite[] backgrounds;
    public float fadeSeconds = 1.5f;
    [Tooltip("Dims the painting so obstacles and pickups stay easy to see.")]
    public Color dim = new Color(0.8f, 0.8f, 0.8f, 1f);
    [Tooltip("Behind everything else (canyon walls use -2..0, scenery -3).")]
    public int sortingOrder = -20;
    [Tooltip("While a painting shows, the old canyon walls keep this much of their opacity.")]
    [Range(0f, 1f)] public float wallOpacityWithPainting = 0.3f;

    // Hyper Dive's backdrop art: a solid green back plate and the canyon wall layers.
    private static readonly HashSet<string> BackPlate = new HashSet<string> { "Asset 9" };
    private static readonly HashSet<string> CanyonWalls = new HashSet<string>
        { "Asset 12", "Asset 13", "greyFrontnew", "greynew", "Asset 17", "Asset 19", "red" };
    private readonly Dictionary<int, float> originalAlpha = new Dictionary<int, float>();
    private float nextWallUpdate;

    private SpriteRenderer current, previous;
    private int shownChapter = -2;
    private float fade = 1f;

    private void Start()
    {
        if (gameCamera == null)
        {
            GameObject tagged = GameObject.FindGameObjectWithTag("AniCam");
            gameCamera = tagged != null ? tagged.GetComponent<Camera>() : Camera.main;
        }
        if (gameCamera == null) { enabled = false; return; }
        current = MakeLayer("Chapter background A");
        previous = MakeLayer("Chapter background B");
    }

    private SpriteRenderer MakeLayer(string name)
    {
        // Not parented to the camera: the gameplay camera's transform has zero scale,
        // which would shrink children to nothing. The layer follows it in Follow() instead.
        var go = new GameObject(name);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sortingOrder = sortingOrder;
        sr.color = Clear();
        return sr;
    }

    private void Update()
    {
        int chapter = chapterManager.instance != null ? chapterManager.instance.CurrentChapterIndex : 0;
        if (chapter != shownChapter)
        {
            shownChapter = chapter;
            Sprite next = backgrounds != null && chapter >= 0 && chapter < backgrounds.Length ? backgrounds[chapter] : null;
            if (current.sprite != next)
            {
                // Swap layers: the old painting fades out while the new one fades in.
                SpriteRenderer t = previous; previous = current; current = t;
                current.sprite = next;
                current.sortingOrder = sortingOrder + 1;
                previous.sortingOrder = sortingOrder;
                Fit(current);
                fade = 0f;
            }
        }

        if (fade < 1f)
        {
            fade = Mathf.Min(1f, fade + Time.deltaTime / Mathf.Max(0.01f, fadeSeconds));
            float k = Mathf.SmoothStep(0f, 1f, fade);
            current.color = current.sprite != null ? WithAlpha(dim, k) : Clear();
            previous.color = previous.sprite != null ? WithAlpha(dim, 1f - k) : Clear();
        }
        Fit(current);
        Fit(previous);
        if (Time.unscaledTime >= nextWallUpdate)
        {
            nextWallUpdate = Time.unscaledTime + 0.2f;   // new wall pieces keep spawning
            FadeOldBackdrop();
        }
    }

    // Lets the painting show through: hide the green back plate, make canyon walls translucent.
    private void FadeOldBackdrop()
    {
        float visible = Mathf.Max(current.sprite != null ? current.color.a : 0f,
                                  previous.sprite != null ? previous.color.a : 0f);
        float wallFactor = Mathf.Lerp(1f, wallOpacityWithPainting, visible);
        float plateFactor = 1f - visible;
        foreach (SpriteRenderer sr in FindObjectsOfType<SpriteRenderer>())
        {
            if (sr.sprite == null || sr == current || sr == previous) continue;
            string n = sr.sprite.name;
            bool plate = BackPlate.Contains(n), wall = !plate && CanyonWalls.Contains(n);
            if (!plate && !wall) continue;
            int id = sr.GetInstanceID();
            if (!originalAlpha.TryGetValue(id, out float a0)) { a0 = sr.color.a; originalAlpha[id] = a0; }
            Color c = sr.color;
            c.a = a0 * (plate ? plateFactor : wallFactor);
            sr.color = c;
        }
    }

    // Follow the camera and scale to cover the whole view on any screen shape.
    private void Fit(SpriteRenderer sr)
    {
        Vector3 p = gameCamera.transform.position;
        sr.transform.position = new Vector3(p.x, p.y, p.z + 10f);
        if (sr.sprite == null) return;
        float viewH = gameCamera.orthographicSize * 2f;
        float viewW = viewH * gameCamera.aspect;
        Vector2 size = sr.sprite.bounds.size;
        float s = Mathf.Max(viewW / size.x, viewH / size.y);
        sr.transform.localScale = new Vector3(s, s, 1f);
    }

    private static Color WithAlpha(Color c, float a) { c.a = a; return c; }
    private static Color Clear() { return new Color(1f, 1f, 1f, 0f); }
}
