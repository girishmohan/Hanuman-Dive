using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnPoint : MonoBehaviour
{
    public GameObject obstacle;

    private void Start()
    {
        GameObject spawned = Instantiate(obstacle, transform.position, Quaternion.identity);
        // Hazards only appear if they leave Hanuman a way through (pickups are never blocked).
        obstacles hazard = spawned.GetComponent<obstacles>();
        if (hazard != null && hazard.damage > 0 && !ramayanaSpawnGuard.Admit(spawned, hazard.speed * ramayanaRunEvents.SpeedRamp()))
        {
            Destroy(spawned);
        }
    }
}
