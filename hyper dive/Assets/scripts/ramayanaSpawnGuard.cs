using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps every row of hazards passable. All hazard spawns ask Admit() first: it predicts
/// where each hazard already on screen will be as everything rises (they rise at different
/// speeds, so pieces spawned apart can end up side by side), and refuses a new hazard if it
/// would ever leave Hanuman less than <see cref="minGap"/> of free width in a row.
/// Refused hazards are simply not spawned.
/// </summary>
public static class ramayanaSpawnGuard
{
    // Narrowest free path (world units) a row may leave. Hanuman's body is about 0.9 wide.
    public static float minGap = 1.9f;
    // Extra vertical room between rows (world units) before they count as separate.
    public static float rowClearance = 1.6f;

    private class Hazard
    {
        public Transform transform;
        public float spawnTime, y0, speed, height, xMin, xMax;
    }

    private static readonly List<Hazard> active = new List<Hazard>();
    private static Camera cam;

    /// <summary>
    /// Registers a hazard if it leaves a passable path. Returns false if it would block a row;
    /// the caller should then destroy it. extraHalfWidth widens it for side-to-side movers.
    /// </summary>
    public static bool Admit(GameObject hazard, float riseSpeed, float extraHalfWidth = 0f)
    {
        if (hazard == null) return false;
        if (!TryBounds(hazard, out Bounds b)) return true;   // nothing visible to block with
        Camera c = GameCamera();
        if (c == null) return true;

        Prune(c);
        var h = new Hazard
        {
            transform = hazard.transform,
            spawnTime = Time.time,
            y0 = b.center.y,
            speed = Mathf.Max(0.1f, riseSpeed),
            height = b.size.y,
            xMin = b.min.x - extraHalfWidth,
            xMax = b.max.x + extraHalfWidth,
        };

        float camHalfW = c.orthographicSize * c.aspect;
        float left = c.transform.position.x - camHalfW, right = c.transform.position.x + camHalfW;
        float visibleUntil = h.spawnTime + (c.transform.position.y + c.orthographicSize + h.height - h.y0) / h.speed;

        // Hazards that share a row with the new one at any moment while it is on screen.
        var blocked = new List<Vector2> { new Vector2(h.xMin, h.xMax) };
        foreach (Hazard a in active)
        {
            if (SharesRow(h, a, h.spawnTime, visibleUntil)) blocked.Add(new Vector2(a.xMin, a.xMax));
        }
        if (LargestGap(blocked, left, right) < minGap)
        {
#if UNITY_EDITOR
            Debug.Log("SPAWN_GUARD skipped " + hazard.name + " (would block the row)");
#endif
            return false;
        }

        active.Add(h);
        return true;
    }

    /// <summary>Registers a hazard that must always appear (no check).</summary>
    public static void Register(GameObject hazard, float riseSpeed)
    {
        if (hazard == null || !TryBounds(hazard, out Bounds b)) return;
        active.Add(new Hazard
        {
            transform = hazard.transform, spawnTime = Time.time, y0 = b.center.y,
            speed = Mathf.Max(0.1f, riseSpeed), height = b.size.y, xMin = b.min.x, xMax = b.max.x,
        });
    }

    private static bool SharesRow(Hazard n, Hazard a, float from, float to)
    {
        // Vertical distance between the two is linear in time: d(t) = p + q t.
        float q = n.speed - a.speed;
        float p = (n.y0 - n.speed * n.spawnTime) - (a.y0 - a.speed * a.spawnTime);
        float limit = (n.height + a.height) * 0.5f + rowClearance;
        float d0 = p + q * from, d1 = p + q * to;
        if (Mathf.Sign(d0) != Mathf.Sign(d1)) return true;     // they pass each other
        return Mathf.Min(Mathf.Abs(d0), Mathf.Abs(d1)) < limit;
    }

    private static float LargestGap(List<Vector2> spans, float left, float right)
    {
        spans.Sort((u, v) => u.x.CompareTo(v.x));
        float best = 0f, cursor = left;
        foreach (Vector2 s in spans)
        {
            if (s.x > cursor) best = Mathf.Max(best, Mathf.Min(s.x, right) - cursor);
            cursor = Mathf.Max(cursor, s.y);
            if (cursor >= right) break;
        }
        if (cursor < right) best = Mathf.Max(best, right - cursor);
        return best;
    }

    private static void Prune(Camera c)
    {
        float top = c.transform.position.y + c.orthographicSize;
        active.RemoveAll(a => a.transform == null
            || a.y0 + a.speed * (Time.time - a.spawnTime) - a.height > top);
    }

    private static bool TryBounds(GameObject go, out Bounds bounds)
    {
        bounds = new Bounds();
        bool any = false;
        foreach (Collider2D col in go.GetComponentsInChildren<Collider2D>())
        {
            if (!col.enabled) continue;
            Bounds cb = col.bounds;
            if (cb.size == Vector3.zero) continue;
            if (!any) { bounds = cb; any = true; } else bounds.Encapsulate(cb);
        }
        if (any) return true;
        foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
        {
            if (!any) { bounds = r.bounds; any = true; } else bounds.Encapsulate(r.bounds);
        }
        return any;
    }

    private static Camera GameCamera()
    {
        if (cam == null)
        {
            GameObject tagged = GameObject.FindGameObjectWithTag("AniCam");
            cam = tagged != null ? tagged.GetComponent<Camera>() : Camera.main;
        }
        return cam;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void ResetForScene()
    {
        active.Clear();
        cam = null;
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += (s, m) => { active.Clear(); cam = null; };
    }
}
