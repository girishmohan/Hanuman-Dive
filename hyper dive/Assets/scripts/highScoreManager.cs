using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class highScoreManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Text highscoreText;
    void Start()
    {
        highscoreText.text = "" + PlayerPrefs.GetFloat("Highscore");
    }

    // Update is called once per frame
    // The best distance is saved as the run goes, so this includes the run that just ended.
    float shown = -1f;
    void Update()
    {
        float best = PlayerPrefs.GetFloat("Highscore");
        if (best != shown)
        {
            shown = best;
            highscoreText.text = "" + best;
        }
    }
}
