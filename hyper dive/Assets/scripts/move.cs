using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Serialization;
//using GoogleMobileAds.Api;
public class move : MonoBehaviour
{
    private Vector3 touchPosition;
    private Rigidbody2D rb;
    private Vector3 direction;
    private float moveSpeed_R_L = 10f;
    [SerializeField]
    private float _speed = 5.0f;
    //public float down_speed;
    public Vector3 respawnPoint;
    private Vector3 targetPos;
    // Rama Naam energy (was "oxygen"/health): reaching 0 ends the run.
    [FormerlySerializedAs("health")]
    public int ramaNaamEnergy = 0;
    //private RewardBasedVideoAd rewardBasedVideo;
    public bool publishingApp;
    public Animator animator;
    public scoreManager gameScore;
    [FormerlySerializedAs("healthDisplay")]
    public UnityEngine.UI.Text energyDisplay;
    public string energyLabel = "Rama Naam";
    private const int energyLabelSize = 13;
    public GameObject gameOver;
    public GameObject toxic_Particle_System;

    private Touch touch;
    public float speedModifer = 0.01f;
    [Tooltip("Arrow keys / A-D speed (world units per second), for playtesting in the editor.")]
    public float keyboardSpeed = 10f;
    private Vector3 lastMousePosition;
    [Tooltip("1 = Hanuman follows the finger exactly; higher = faster steering.")]
    public float dragSensitivity = 1f;
    private Vector2 lastTouchPosition;
    private int trackedFinger = -1;
    private Camera gameCamera;
    //public GameObject effect;

    private void Start()
    {
       
        rb = GetComponent<Rigidbody2D>();
        // Two-line energy readout (number + caption) may exceed the original text box.
        energyDisplay.horizontalOverflow = HorizontalWrapMode.Overflow;
        energyDisplay.verticalOverflow = VerticalWrapMode.Overflow;
        energyDisplay.lineSpacing = 0.8f;
        respawnPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {


        ramaNaamEnergy = Mathf.Clamp(ramaNaamEnergy, -10, 100);
        UpdateEnergyDisplay();

        if (ramaNaamEnergy <= 0)
        {
            gameOver.SetActive(true);
            ramayanaRunEvents.ShowDeathCause(gameOver);
            Destroy(gameObject);
            Time.timeScale = 0f;
            //highScore_save();
            //RequestRewardBasedVideo();
            //gameScore.scoreText.gameObject.SetActive(false);
            //gameScore.scoreText.text = ((int)gameScore).ToString();
            //this code below is to change scene and show game over and even ask to play again
            //SceneManager.LoadScene("gameOver");
        }

        if (Input.touchCount > 0)
        {
            move_with_touch_drag();
        }
        else
        {
            // Editor/desktop playtesting: mouse drag works like a finger, arrow keys also steer.
            move_with_mouse_drag();
            move_with_keys();
        }
    }

    public void AddEnergy(int amount)
    {
        ramaNaamEnergy += amount;
        UpdateEnergyDisplay();
    }

    void UpdateEnergyDisplay()
    {
        // Number on top, small caption underneath so it stays clear of the METERS heading.
        energyDisplay.text = Mathf.Max(ramaNaamEnergy, 0) + "%\n<size=" + energyLabelSize + ">" + energyLabel.ToUpper() + "</size>";
    }

    

    void move_keyboard()
    {
        animator.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
        float move = Input.GetAxis("Horizontal");

        rb.velocity = new Vector2(move * _speed * Time.deltaTime, rb.velocity.y);
        //Instantiate(effect, transform.position, Quaternion.identity);
    }
    //this is for touch input move

    void move_with_touch_drag()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            // Measure the finger's movement from its positions: touch.deltaPosition is
            // unreliable in browsers (it comes through reversed on iPhone Safari).
            if (touch.phase == TouchPhase.Began || touch.fingerId != trackedFinger)
            {
                trackedFinger = touch.fingerId;
                lastTouchPosition = touch.position;
                return;
            }
            float dx = touch.position.x - lastTouchPosition.x;
            lastTouchPosition = touch.position;
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) trackedFinger = -1;
            SteerByScreenPixels(dx);
        }
    }

    // Moves Hanuman by a horizontal screen distance, so he follows the finger the same
    // way on every screen size and pixel density.
    void SteerByScreenPixels(float dxPixels)
    {
        if (gameCamera == null)
        {
            GameObject tagged = GameObject.FindGameObjectWithTag("AniCam");
            gameCamera = tagged != null ? tagged.GetComponent<Camera>() : Camera.main;
        }
        float worldPerPixel = gameCamera != null
            ? gameCamera.orthographicSize * 2f * gameCamera.aspect / Mathf.Max(1, Screen.width)
            : speedModifer;
        transform.position = new Vector3(transform.position.x + dxPixels * worldPerPixel * dragSensitivity, transform.position.y);
    }


    void move_with_mouse_drag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            float dx = Input.mousePosition.x - lastMousePosition.x;
            lastMousePosition = Input.mousePosition;
            SteerByScreenPixels(dx);
        }
    }

    void move_with_keys()
    {
        float h = Input.GetAxisRaw("Horizontal");
        if (h != 0f)
        {
            transform.position = new Vector3(transform.position.x + h * keyboardSpeed * Time.deltaTime, transform.position.y);
        }
    }

    void move_touch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            touchPosition.z = 0;
            direction = (touchPosition - transform.position);
            rb.velocity = new Vector2(direction.x, direction.y) * moveSpeed_R_L;

            if (touch.phase == TouchPhase.Ended)
                rb.velocity = Vector2.zero;
        }
    }
    public Vector3 GetPosition()
    {
        return transform.position;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "toxic")
        {
            Instantiate(toxic_Particle_System, transform.position, Quaternion.identity);
        }
    }
    //void OnTriggerEnter2D(Collider2D other)
    //{
    //if (other.tag == "FallDetector")
    //{
    //what will happen if player falls on rhe fall detector part
    //transform.position = respawnPoint;
    //}
    //if(other.tag == "Checkpoint")
    //{
    //respawnPoint = other.transform.position;
    //}
    //}
}

