using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    public GameObject[] obstaclePatterns;
    private float timeBtwSpawn;
    public float StartTimeBtwSpawn;
    public float decreaseTime;
    public float minTime = 0.65f;
    public static spawner instanceSpawn;
    // Hazard spawners (walls, mines, saws, throwing stars) space out more at the start of a run.
    private bool isHazard;
    private void Awake()
    {
        instanceSpawn = this;
        string n = gameObject.name.ToLower();
        isHazard = n.Contains("wall") || n.Contains("bomb") || n.Contains("ninja") || n.Contains("saw");
    }
    private void Update()
    {
        StartSpawning();
    }
    public void StartSpawning()
    {
        if (timeBtwSpawn <= 0)
        {
            int rand = Random.Range(0, obstaclePatterns.Length);
            Instantiate(obstaclePatterns[rand], transform.position, Quaternion.identity);
            timeBtwSpawn = StartTimeBtwSpawn * (isHazard ? ramayanaRunEvents.OpeningEase() : 1f);
            if (StartTimeBtwSpawn > minTime)
            {
                StartTimeBtwSpawn -= decreaseTime;
            }
        }
        else
        {
            timeBtwSpawn -= Time.deltaTime;
        }
    }
}
