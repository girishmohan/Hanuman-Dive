using UnityEngine;

/// <summary>
/// Rakshasas (Ravana's demon warriors) that rise toward Hanuman, weaving side
/// to side. Bumping into one costs 5% Rama Naam energy. They start after a short calm
/// opening and then appear every few seconds in every chapter.
/// </summary>
public class ramayanaDemons : MonoBehaviour
{
    public Camera gameCamera;
    public Sprite[] rakshasaSprites;

    [Header("When")]
    public float firstAppearSeconds = 12f;
    public Vector2 secondsBetween = new Vector2(5f, 8f);

    [Header("How")]
    [Tooltip("Rama Naam energy lost per bump (energy is 0-100, so 5 = 5%).")]
    public int damage = 5;
    public float riseSpeed = 11f;
    public float weaveWidth = 1.6f;
    public float weaveSpeed = 1.8f;
    public float height = 2.2f;
    [Tooltip("Half-width of the playable channel (world units).")]
    public float playAreaHalfWidth = 4.2f;
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
        if (gameCamera == null || rakshasaSprites == null || rakshasaSprites.Length == 0) return;
        timer -= Time.deltaTime;
        if (timer > 0f) return;
        timer = Random.Range(secondsBetween.x, secondsBetween.y);
        Spawn();
    }

    private void Spawn()
    {
        float halfWidth = Mathf.Min(gameCamera.orthographicSize * gameCamera.aspect, playAreaHalfWidth) - weaveWidth * 0.5f;
        float centerX = gameCamera.transform.position.x + Random.Range(-halfWidth, halfWidth) * 0.6f;
        float y = gameCamera.transform.position.y - gameCamera.orthographicSize - height;

        GameObject root = new GameObject("Rakshasa");
        root.transform.position = new Vector3(centerX, y, 0f);
        ramayanaDemonMover mover = root.AddComponent<ramayanaDemonMover>();
        mover.riseSpeed = riseSpeed;
        mover.weaveWidth = weaveWidth;
        mover.weaveSpeed = weaveSpeed * Random.Range(0.8f, 1.2f);
        mover.centerX = centerX;
        mover.minX = gameCamera.transform.position.x - halfWidth;
        mover.maxX = gameCamera.transform.position.x + halfWidth;

        GameObject body = new GameObject("Body");
        body.transform.SetParent(root.transform, false);
        Sprite sprite = rakshasaSprites[Random.Range(0, rakshasaSprites.Length)];
        float scale = height / Mathf.Max(0.01f, sprite.bounds.size.y);
        body.transform.localScale = new Vector3(scale, scale, 1f);
        SpriteRenderer sr = body.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = sortingOrder;

        // Hit area: the body, not the club or horns.
        CircleCollider2D hit = body.AddComponent<CircleCollider2D>();
        hit.isTrigger = true;
        hit.radius = sprite.bounds.size.x * 0.3f;
        hit.offset = new Vector2(0f, -sprite.bounds.size.y * 0.05f);
        ramayanaHazardHit h = body.AddComponent<ramayanaHazardHit>();
        h.energyChange = -damage;
        h.removeOnTouch = root;

        float travel = gameCamera.orthographicSize * 2f + height * 3f;
        Destroy(root, travel / riseSpeed + 1f);
    }
}

/// <summary>Rises like other obstacles while weaving left and right.</summary>
public class ramayanaDemonMover : MonoBehaviour
{
    public float riseSpeed;
    public float weaveWidth;
    public float weaveSpeed;
    public float centerX;
    public float minX;
    public float maxX;
    private float t;

    private void Update()
    {
        t += Time.deltaTime;
        Vector3 p = transform.position;
        p.y += riseSpeed * Time.deltaTime;
        p.x = Mathf.Clamp(centerX + Mathf.Sin(t * weaveSpeed) * weaveWidth, minX, maxX);
        transform.position = p;
        // Lean into the direction of travel.
        transform.rotation = Quaternion.Euler(0f, 0f, -Mathf.Cos(t * weaveSpeed) * 8f);
    }
}
