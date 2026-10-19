using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class levelManager : MonoBehaviour
{
    public float respawnDelay;
    
    public static levelManager instanceLevel;
    //public spawner spawn;
    public int coins;
    public Vector3 spawnPoint;
    //public GameObject shopWindow;
    public UnityEngine.UI.Text coinText_score;
    public GameObject DefaultPlayer;
    //this the skin INT
    //2500
    int is2500Sold;
    //5000
    int is5000Sold;
    //7500
    int is7500Sold;
    //15000
    int is15000Sold;
    //50000
    int is50000Sold;
    //75000
    int is75000Sold;
    //100000
    int is100000Sold;
    //150000
    int is150000Sold;
    //this is for the skins 2500-150,000 GEMS
    //2500
    public GameObject skin2500;
    //5000
    public GameObject skin5000;
    //7500
    public GameObject skin7500;
    //15000
    public GameObject skin15000;
    //50000
    public GameObject skin50000;
    //75000
    public GameObject skin75000;
    //100000
    public GameObject skin100000;
    //150000
    public GameObject skin150000;
    // Use this for initialization

    private void Awake()
    {
        instanceLevel = this;
    }
    void Start()
    {
        coins = PlayerPrefs.GetInt("GemAmount");
        coinText_score.text = "" + coins;

        //This is for the calling stored items form DATA....
        //2500
        is2500Sold = PlayerPrefs.GetInt("Is2500Sold");
        //5000
        is5000Sold = PlayerPrefs.GetInt("Is5000Sold");
        //7500
        is7500Sold = PlayerPrefs.GetInt("Is7500Sold");
        //15000
        is15000Sold = PlayerPrefs.GetInt("Is15000Sold");
        //50000
        is50000Sold = PlayerPrefs.GetInt("Is50000Sold");
        //75000
        is75000Sold = PlayerPrefs.GetInt("Is75000Sold");
        //100000
        is100000Sold = PlayerPrefs.GetInt("Is100000Sold");
        //150000
        is150000Sold = PlayerPrefs.GetInt("Is150000Sold");

        // Only one avatar can be worn: keep the first equipped one, clear the rest.
        bool chosen = false;
        foreach (string price in new[] { "2500", "5000", "7500", "15000", "50000", "75000", "100000", "150000" })
        {
            if (PlayerPrefs.GetInt("Is" + price + "Sold") != 1) continue;
            if (chosen) PlayerPrefs.SetInt("Is" + price + "Sold", 0);
            chosen = true;
        }
        is2500Sold = PlayerPrefs.GetInt("Is2500Sold");
        is5000Sold = PlayerPrefs.GetInt("Is5000Sold");
        is7500Sold = PlayerPrefs.GetInt("Is7500Sold");
        is15000Sold = PlayerPrefs.GetInt("Is15000Sold");
        is50000Sold = PlayerPrefs.GetInt("Is50000Sold");
        is75000Sold = PlayerPrefs.GetInt("Is75000Sold");
        is100000Sold = PlayerPrefs.GetInt("Is100000Sold");
        is150000Sold = PlayerPrefs.GetInt("Is150000Sold");

        //skin Using Check.................................
        //2500

        check2500skin();
        //5000
        check5000skin();
        //7500
        check7500skin();
        //15000
        check15000skin();
        //50000
        check50000skin();
        //75000
        check75000skin();
        //100000
        check100000skin();
        //150000
        check150000skin();

    }

    //If statements for the skins.............................
    //2500
    public void check2500skin()
    {
        if (is2500Sold == 1)
        {
            skin2500.SetActive(true);
            Destroy(DefaultPlayer);
        }
        else
        {
            DefaultPlayer.SetActive(true);

        }
    }
    //5000
    public void check5000skin()
    {
        if (is5000Sold == 1)
        {
            skin5000.SetActive(true);
            Destroy(DefaultPlayer);
        }
        else
        {
            DefaultPlayer.SetActive(true);

        }
    }
    //7500
    public void check7500skin()
    {
        if (is7500Sold == 1)
        {
            skin7500.SetActive(true);
            Destroy(DefaultPlayer);
        }
        else
        {
            DefaultPlayer.SetActive(true);

        }
    }
    //15000
    public void check15000skin()
    {
        if (is15000Sold == 1)
        {
            skin15000.SetActive(true);
            Destroy(DefaultPlayer);
        }
        else
        {
            DefaultPlayer.SetActive(true);

        }
    }
    //50000
    public void check50000skin()
    {
        if (is50000Sold == 1)
        {
            skin50000.SetActive(true);
            Destroy(DefaultPlayer);
        }
        else
        {
            DefaultPlayer.SetActive(true);

        }
    }
    //75000
    public void check75000skin()
    {
        if (is75000Sold == 1)
        {
            skin75000.SetActive(true);
            Destroy(DefaultPlayer);
        }
        else
        {
            DefaultPlayer.SetActive(true);

        }
    }
    //100000
    public void check100000skin()
    {
        if (is100000Sold == 1)
        {
            skin100000.SetActive(true);
            Destroy(DefaultPlayer);
        }
        else
        {
            DefaultPlayer.SetActive(true);

        }
    }
    //150000
    public void check150000skin()
    {
        if (is150000Sold == 1)
        {
            skin150000.SetActive(true);
            Destroy(DefaultPlayer);
            DefaultPlayer.SetActive(false);
        }
        else
        {
            DefaultPlayer.SetActive(true);

        }
    }

    // Update is called once per frame
    void Update()
    {
        // Keep the HUD in step with the saved total.
        int saved = PlayerPrefs.GetInt("GemAmount");
        if (saved != coins)
        {
            coins = saved;
            coinText_score.text = "" + coins;
        }
    }
    
    
    public void AddCoins(int numberOfCoins)
    {
        // Always add to the saved total, so the HUD, shop and rewards never disagree.
        coins = PlayerPrefs.GetInt("GemAmount") + numberOfCoins;
        PlayerPrefs.SetInt("GemAmount", coins);
        coinText_score.text = "" + coins;
    }
    public void gemsSave()
    {
        PlayerPrefs.SetInt("GemAmount", coins);
    }

    public void gotoShop()
    {
        //PlayerPrefs.SetInt("GemAmount", coins);
        SceneManager.LoadScene("skinShop");
    }
}