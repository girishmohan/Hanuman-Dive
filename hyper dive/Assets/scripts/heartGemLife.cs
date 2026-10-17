using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class heartGemLife : MonoBehaviour
{
    private move player;
    [FormerlySerializedAs("healthPerTouch")]
    public int energyPerTouch;
    public GameObject Effect;
    public float speed;
    private Animator anim;
    //private Animation greenLight_Text;

    // Use this for initialization
    void Start()
    {
        //anim = GetComponent<Animator>();
        player = FindObjectOfType<move>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            
            Instantiate(Effect, transform.position, Quaternion.identity);
            
            player.AddEnergy(energyPerTouch);
            //anim.SetTrigger("green");
            Destroy(gameObject);
            
        }
    }
}
