using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scoreDamage : MonoBehaviour
{
    // Start is called before the first frame update
    public int damage = 1;
    //private shake Shake;
    void Start()
    {
        //Shake = GameObject.FindGameObjectWithTag("ScreenShake").GetComponent<shake>();

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Shake.CamShake();
        if (other.CompareTag("Player"))
        {
            
            //Instantiate(effect, transform.position, Quaternion.identity);
            // the player will take damage
            ramayanaRunEvents.Hurt(ramayanaRunEvents.EnergyRanOut);
            other.GetComponent<move>().ramaNaamEnergy -= damage;
            Debug.Log(other.GetComponent<move>().ramaNaamEnergy);
            Destroy(gameObject);
            
        }
    }
}
