using UnityEngine;

/// <summary>A friendly vanara who can rise between the obstacles.</summary>
[System.Serializable]
public class VanaraAlly
{
    public string name;
    public string cheer;
    public Sprite sprite;
}

/// <summary>
/// Vanara allies (Rama's monkey army) rising between the obstacles. Touching one
/// gives Hanuman a burst of Rama Naam energy and a short cheer. They appear every
/// so often in every level, as a friendly balance to the rakshasas.
/// </summary>
public class ramayanaVanaras : MonoBehaviour
{
    public Camera gameCamera;
    public VanaraAlly[] allies;

    [Header("When")]
    public float firstAppearSeconds = 8f;
    public Vector2 secondsBetween = new Vector2(15f, 25f);

    [Header("How")]
    public int energyBoost = 10;
    public float riseSpeed = 8f;
    public float height = 2.4f;
    [Tooltip("Half-width of the playable channel (world units).")]
    public float playAreaHalfWidth = 3.8f;
    public int sortingOrder = 6;

    private float timer;

    private void Start()
    {
        if (gameCamera == null)
        {
            GameObject tagged = GameObject.FindGameObjectWithTag("AniCam");
            gameCamera = tagged != null ? tagged.GetComponent<Camera>() : Camera.main;
        }
        timer = firstAppearSeconds;
    }

    private void Update()
    {
        if (gameCamera == null || allies == null || allies.Length == 0) return;
        timer -= Time.deltaTime;
        if (timer > 0f) return;
        timer = Random.Range(secondsBetween.x, secondsBetween.y);
        Spawn(allies[Random.Range(0, allies.Length)]);
    }

    private void Spawn(VanaraAlly ally)
    {
        if (ally.sprite == null) return;
        float halfWidth = Mathf.Min(gameCamera.orthographicSize * gameCamera.aspect, playAreaHalfWidth);
        float x = gameCamera.transform.position.x + Random.Range(-halfWidth, halfWidth) * 0.7f;
        float y = gameCamera.transform.position.y - gameCamera.orthographicSize - height;

        GameObject root = new GameObject("Vanara " + ally.name);
        root.transform.position = new Vector3(x, y, 0f);
        ramayanaHazardMover mover = root.AddComponent<ramayanaHazardMover>();
        mover.riseSpeed = riseSpeed;

        GameObject body = new GameObject("Body");
        body.transform.SetParent(root.transform, false);
        float scale = height / Mathf.Max(0.01f, ally.sprite.bounds.size.y);
        body.transform.localScale = new Vector3(Random.value < 0.5f ? -scale : scale, scale, 1f);
        SpriteRenderer sr = body.AddComponent<SpriteRenderer>();
        sr.sprite = ally.sprite;
        sr.sortingOrder = sortingOrder;
        CircleCollider2D c = body.AddComponent<CircleCollider2D>();
        c.isTrigger = true;
        c.radius = ally.sprite.bounds.size.x * 0.35f;
        ramayanaVanaraTouch touch = body.AddComponent<ramayanaVanaraTouch>();
        touch.owner = this;
        touch.ally = ally;
        touch.root = root;

        float travel = gameCamera.orthographicSize * 2f + height * 3f;
        Destroy(root, travel / riseSpeed + 1f);
    }

    public void Met(VanaraAlly ally)
    {
        move hanuman = FindObjectOfType<move>();
        if (hanuman != null) hanuman.AddEnergy(energyBoost);
        if (chapterManager.instance != null)
        {
            chapterManager.instance.ShowBanner(ally.name, ally.cheer + "  +" + energyBoost + "% Rama Naam", 2f);
        }
    }

    /// <summary>Default allies; sprites are assigned in the scene.</summary>
    public static VanaraAlly[] Defaults()
    {
        return new VanaraAlly[]
        {
            new VanaraAlly { name = "Angada", cheer = "The prince of Kishkindha cheers you on" },
            new VanaraAlly { name = "Vanara Archer", cheer = "Rama's army covers your flight" },
            new VanaraAlly { name = "Jambavan", cheer = "\"Remember your strength, Hanuman!\"" },
        };
    }
}

/// <summary>Hanuman meets a vanara ally.</summary>
public class ramayanaVanaraTouch : MonoBehaviour
{
    public ramayanaVanaras owner;
    public VanaraAlly ally;
    public GameObject root;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (owner != null) owner.Met(ally);
        Destroy(root != null ? root : gameObject);
    }
}
