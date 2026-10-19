using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class shopManager : MonoBehaviour
{
    //public int isTheSkinSold;
    public GameObject BackGoToHome;
    public Text gemAmountText;
    public int gemAmount;
   
    //2500
    public Button Button2500;
    int is2500Sold;
    int the2500IsBought;
    public Button boughtButton2500;
    public Button equipedButton2500;
    //5000
    public Button Button5000;
    int is5000Sold;
    int the5000IsBought;
    public Button boughtButton5000;
    public Button equipedButton5000;
    //7500
    public Button Button7500;
    int is7500Sold;
    int the7500IsBought;
    public Button boughtButton7500;
    public Button equipedButton7500;
    //15000
    public Button Button15000;
    int is15000Sold;
    int the15000IsBought;
    public Button boughtButton15000;
    public Button equipedButton15000;
    //50000
    public Button Button50000;
    int is50000Sold;
    int the50000IsBought;
    public Button boughtButton50000;
    public Button equipedButton50000;
    //75000
    public Button Button75000;
    int is75000Sold;
    int the75000IsBought;
    public Button boughtButton75000;
    public Button equipedButton75000;
    //100000
    public Button Button100000;
    int is100000Sold;
    int the100000IsBought;
    public Button boughtButton100000;
    public Button equipedButton100000;
    //150000
    public Button Button150000;
    int is150000Sold;
    int the150000IsBought;
    public Button boughtButton150000;
    public Button equipedButton150000;




    void Start()
    {
        gemAmount = PlayerPrefs.GetInt("GemAmount");
        
    }

    // Update is called once per frame
    void Update()
    {
        gemAmountText.text = gemAmount.ToString();
        //saving if bought 2500
        is2500Sold = PlayerPrefs.GetInt("Is2500Sold");
        the2500IsBought = PlayerPrefs.GetInt("The2500IsBaught");
        //saving if bought 5000
        is5000Sold = PlayerPrefs.GetInt("Is5000Sold");
        the5000IsBought = PlayerPrefs.GetInt("The5000IsBaught");
        //saving if bought 7500
        is7500Sold = PlayerPrefs.GetInt("Is7500Sold");
        the7500IsBought = PlayerPrefs.GetInt("The7500IsBaught");
        //saving if bought 15000
        is15000Sold = PlayerPrefs.GetInt("Is15000Sold");
        the15000IsBought = PlayerPrefs.GetInt("The15000IsBaught");
        //saving if bought 50000
        is50000Sold = PlayerPrefs.GetInt("Is50000Sold");
        the50000IsBought = PlayerPrefs.GetInt("The50000IsBaught");
        //saving if bought 75000
        is75000Sold = PlayerPrefs.GetInt("Is75000Sold");
        the75000IsBought = PlayerPrefs.GetInt("The75000IsBaught");
        //saving if bought 100000
        is100000Sold = PlayerPrefs.GetInt("Is100000Sold");
        the100000IsBought = PlayerPrefs.GetInt("The100000IsBaught");
        //saving if bought 150000
        is150000Sold = PlayerPrefs.GetInt("Is150000Sold");
        the150000IsBought = PlayerPrefs.GetInt("The150000IsBaught");
        //...........................................................

        //if statement func 2500
        For2500IfStatement();
        check2500Bought();
        //if statement func 5000
        For5000IfStatement();
        check5000Bought();
        //if statement func 7500
        For7500IfStatement();
        check7500Bought();
        //if statement func 15000
        For15000IfStatement();
        check15000Bought();
        //if statement func 50000
        For50000IfStatement();
        check50000Bought();
        //if statement func 75000
        For75000IfStatement();
        check75000Bought();
        //if statement func 100000
        For100000IfStatement();
        check100000Bought();
        //if statement func 150000
        For150000IfStatement();
        check150000Bought();
    }


    //For checking if bought.........................................

    //2500
    public void check2500Bought()
    {
        if(the2500IsBought == 1)
        {
            boughtButton2500.gameObject.SetActive(true);
            Button2500.gameObject.SetActive(false);
            
        }
        else
        {
            Button2500.gameObject.SetActive(true);

        }
    }

    //5000
    public void check5000Bought()
    {
        if (the5000IsBought == 1)
        {
            boughtButton5000.gameObject.SetActive(true);
            Button5000.gameObject.SetActive(false);

        }
        else
        {
            Button5000.gameObject.SetActive(true);

        }
    }
    //7500
    public void check7500Bought()
    {
        if (the7500IsBought == 1)
        {
            boughtButton7500.gameObject.SetActive(true);
            Button7500.gameObject.SetActive(false);

        }
        else
        {
            Button7500.gameObject.SetActive(true);

        }
    }
    //15000
    public void check15000Bought()
    {
        if (the15000IsBought == 1)
        {
            boughtButton15000.gameObject.SetActive(true);
            Button15000.gameObject.SetActive(false);

        }
        else
        {
            Button15000.gameObject.SetActive(true);

        }
    }
    //50000
    public void check50000Bought()
    {
        if (the50000IsBought == 1)
        {
            boughtButton50000.gameObject.SetActive(true);
            Button50000.gameObject.SetActive(false);

        }
        else
        {
            Button50000.gameObject.SetActive(true);

        }
    }
    //75000
    public void check75000Bought()
    {
        if (the75000IsBought == 1)
        {
            boughtButton75000.gameObject.SetActive(true);
            Button75000.gameObject.SetActive(false);

        }
        else
        {
            Button75000.gameObject.SetActive(true);

        }
    }
    //100000
    public void check100000Bought()
    {
        if (the100000IsBought == 1)
        {
            boughtButton100000.gameObject.SetActive(true);
            Button100000.gameObject.SetActive(false);

        }
        else
        {
            Button100000.gameObject.SetActive(true);

        }
    }
    //150000
    public void check150000Bought()
    {
        if (the150000IsBought == 1)
        {
            boughtButton150000.gameObject.SetActive(true);
            Button150000.gameObject.SetActive(false);

        }
        else
        {
            Button150000.gameObject.SetActive(true);

        }
    }
    // func with if statements........................................

    //2500
    public void For2500IfStatement()
    {
        if (gemAmount >= 500 && is2500Sold == 0)
        {
            Button2500.interactable = true;
        }
        else
        {
            Button2500.interactable = false;
        }

    }

    //5000
    public void For5000IfStatement()
    {
        if (gemAmount >= 1500 && is5000Sold == 0)
        {
            Button5000.interactable = true;
        }
        else
        {
            Button5000.interactable = false;
        }

    }
    //7500
    public void For7500IfStatement()
    {
        if (gemAmount >= 3000 && is7500Sold == 0)
        {
            Button7500.interactable = true;
        }
        else
        {
            Button7500.interactable = false;
        }

    }
    //15000
    public void For15000IfStatement()
    {
        if (gemAmount >= 6000 && is15000Sold == 0)
        {
            Button15000.interactable = true;
        }
        else
        {
            Button15000.interactable = false;
        }

    }
    //50000
    public void For50000IfStatement()
    {
        if (gemAmount >= 12000 && is50000Sold == 0)
        {
            Button50000.interactable = true;
        }
        else
        {
            Button50000.interactable = false;
        }

    }
    //75000
    public void For75000IfStatement()
    {
        if (gemAmount >= 25000 && is75000Sold == 0)
        {
            Button75000.interactable = true;
        }
        else
        {
            Button75000.interactable = false;
        }

    }
    //100000
    public void For100000IfStatement()
    {
        if (gemAmount >= 50000 && is100000Sold == 0)
        {
            Button100000.interactable = true;
        }
        else
        {
            Button100000.interactable = false;
        }

    }
    //150000
    public void For150000IfStatement()
    {
        if (gemAmount >= 100000 && is150000Sold == 0)
        {
            Button150000.interactable = true;
        }
        else
        {
            Button150000.interactable = false;
        }

    }
    //buy SKIN........................................................

    //2500
    public void buy2500Skin()
    {
        gemAmount -= 500;
        //PlayerPrefs.SetInt("Is2500Sold", 1);
        boughtButton2500.gameObject.SetActive(true);
        Button2500.gameObject.SetActive(false);
        PlayerPrefs.SetInt("GemAmount", gemAmount);
        PlayerPrefs.SetInt("The2500IsBaught", 1);
    }
    //5000
    public void buy5000Skin()
    {
        gemAmount -= 1500;
        boughtButton5000.gameObject.SetActive(true);
        Button5000.gameObject.SetActive(false);
        PlayerPrefs.SetInt("GemAmount", gemAmount);
        PlayerPrefs.SetInt("The5000IsBaught", 1);
    }
    //7500
    public void buy7500Skin()
    {
        gemAmount -= 3000;
        boughtButton7500.gameObject.SetActive(true);
        Button7500.gameObject.SetActive(false);
        PlayerPrefs.SetInt("GemAmount", gemAmount);
        PlayerPrefs.SetInt("The7500IsBaught", 1);
    }
    //15000
    public void buy15000Skin()
    {
        gemAmount -= 6000;
        boughtButton15000.gameObject.SetActive(true);
        Button15000.gameObject.SetActive(false);
        PlayerPrefs.SetInt("GemAmount", gemAmount);
        PlayerPrefs.SetInt("The15000IsBaught", 1);
    }
    //50000
    public void buy50000Skin()
    {
        gemAmount -= 12000;
        boughtButton50000.gameObject.SetActive(true);
        Button50000.gameObject.SetActive(false);
        PlayerPrefs.SetInt("GemAmount", gemAmount);
        PlayerPrefs.SetInt("The50000IsBaught", 1);
    }
    //75000
    public void buy75000Skin()
    {
        gemAmount -= 25000;
        boughtButton75000.gameObject.SetActive(true);
        Button75000.gameObject.SetActive(false);
        PlayerPrefs.SetInt("GemAmount", gemAmount);
        PlayerPrefs.SetInt("The75000IsBaught", 1);
    }
    //100000
    public void buy100000Skin()
    {
        gemAmount -= 50000;
        boughtButton100000.gameObject.SetActive(true);
        Button100000.gameObject.SetActive(false);
        PlayerPrefs.SetInt("GemAmount", gemAmount);
        PlayerPrefs.SetInt("The100000IsBaught", 1);
    }
    //150000
    public void buy150000Skin()
    {
        gemAmount -= 100000;
        boughtButton150000.gameObject.SetActive(true);
        Button150000.gameObject.SetActive(false);
        PlayerPrefs.SetInt("GemAmount", gemAmount);
        PlayerPrefs.SetInt("The150000IsBaught", 1);
    }
    //equip SKIN........................................................

    //2500
    public void equip2500skin()
    {
        PlayerPrefs.SetInt("Is2500Sold", 1);
        boughtButton2500.gameObject.SetActive(false);
        equipedButton2500.gameObject.SetActive(true);
        //PlayerPrefs.SetInt("The2500IsEquiped", 1);
        SceneManager.LoadScene("start");
    }
    //5000
    public void equip5000skin()
    {
        PlayerPrefs.SetInt("Is5000Sold", 1);
        boughtButton5000.gameObject.SetActive(false);
        equipedButton5000.gameObject.SetActive(true);
        SceneManager.LoadScene("start");
    }
    //7500
    public void equip7500skin()
    {
        PlayerPrefs.SetInt("Is7500Sold", 1);
        boughtButton7500.gameObject.SetActive(false);
        equipedButton7500.gameObject.SetActive(true);
        SceneManager.LoadScene("start");
    }
    //15000
    public void equip15000skin()
    {
        PlayerPrefs.SetInt("Is15000Sold", 1);
        boughtButton15000.gameObject.SetActive(false);
        equipedButton15000.gameObject.SetActive(true);
        SceneManager.LoadScene("start");
    }
    //50000
    public void equip50000skin()
    {
        PlayerPrefs.SetInt("Is50000Sold", 1);
        boughtButton50000.gameObject.SetActive(false);
        equipedButton50000.gameObject.SetActive(true);
        SceneManager.LoadScene("start");
    }
    //75000
    public void equip75000skin()
    {
        PlayerPrefs.SetInt("Is75000Sold", 1);
        boughtButton75000.gameObject.SetActive(false);
        equipedButton75000.gameObject.SetActive(true);
        SceneManager.LoadScene("start");
    }
    //100000
    public void equip100000skin()
    {
        PlayerPrefs.SetInt("Is100000Sold", 1);
        boughtButton100000.gameObject.SetActive(false);
        equipedButton100000.gameObject.SetActive(true);
        //boughtButton2500.interactable = false;
        //boughtButton5000.interactable = false;
        //boughtButton7500.interactable = false;
        //boughtButton15000.interactable = false;
        //boughtButton50000.interactable = false;
        //boughtButton75000.interactable = false;
        //boughtButton100000.interactable = false;
        //boughtButton150000.interactable = false;
        //Button2500.interactable = false;
        //Button5000.interactable = false;
        //Button7500.interactable = false;
        //Button15000.interactable = false;
        //Button50000.interactable = false;
        //Button75000.interactable = false;
        //Button100000.interactable = false;
        //Button150000.interactable = false;
        SceneManager.LoadScene("start");
    }
    //150000
    public void equip150000skin()
    {
        PlayerPrefs.SetInt("Is150000Sold", 1);
        boughtButton150000.gameObject.SetActive(false);
        equipedButton150000.gameObject.SetActive(true);
        SceneManager.LoadScene("start");
    }
    //equiped SKIN........................................................

    //2500
    public void equiped2500ToequipSkin()
    {
        boughtButton2500.gameObject.SetActive(true);
        PlayerPrefs.SetInt("Is2500Sold", 0);
        equipedButton2500.gameObject.SetActive(false);
        //PlayerPrefs.SetInt("The2500IsEquiped", 1);
    }
    //5000
    public void equiped5000ToequipSkin()
    {
        boughtButton5000.gameObject.SetActive(true);
        PlayerPrefs.SetInt("Is5000Sold", 0);
        equipedButton5000.gameObject.SetActive(false);
    }
    //7500
    public void equiped7500ToequipSkin()
    {
        boughtButton7500.gameObject.SetActive(true);
        PlayerPrefs.SetInt("Is7500Sold", 0);
        equipedButton7500.gameObject.SetActive(false);
    }
    //15000
    public void equiped15000ToequipSkin()
    {
        boughtButton15000.gameObject.SetActive(true);
        PlayerPrefs.SetInt("Is15000Sold", 0);
        equipedButton15000.gameObject.SetActive(false);
    }
    //50000
    public void equiped50000ToequipSkin()
    {
        boughtButton50000.gameObject.SetActive(true);
        PlayerPrefs.SetInt("Is50000Sold", 0);
        equipedButton50000.gameObject.SetActive(false);
    }
    //75000
    public void equiped75000ToequipSkin()
    {
        boughtButton75000.gameObject.SetActive(true);
        PlayerPrefs.SetInt("Is75000Sold", 0);
        equipedButton75000.gameObject.SetActive(false);
    }
    //100000
    public void equiped100000ToequipSkin()
    {
        boughtButton100000.gameObject.SetActive(true);
        PlayerPrefs.SetInt("Is100000Sold", 0);
        equipedButton100000.gameObject.SetActive(false);
        //boughtButton2500.interactable = true;
        //boughtButton5000.interactable = true;
        //boughtButton7500.interactable = true;
        //boughtButton15000.interactable = true;
        //boughtButton50000.interactable = true;
        //boughtButton75000.interactable = true;
        //boughtButton100000.interactable = true;
        //boughtButton150000.interactable = true;
        //Button2500.interactable = true;
        //Button5000.interactable = true;
        //Button7500.interactable = true;
        //Button15000.interactable = true;
        //Button50000.interactable = true;
        //Button75000.interactable = true;
        //Button100000.interactable = true;
        //Button150000.interactable = true;
    }
    //150000
    public void equiped150000ToequipSkin()
    {
        boughtButton150000.gameObject.SetActive(true);
        PlayerPrefs.SetInt("Is150000Sold", 0);
        equipedButton150000.gameObject.SetActive(false);
    }

    // this to go home END..................................................

    public void backtoTheGame()
    {
        PlayerPrefs.SetInt("GemAmount", gemAmount);
        SceneManager.LoadScene("start");
        
    }

    // Development-only buttons from Hyper Dive (wipe all data / add 1,000,000 gems).
    // Disabled for release; kept so existing button references don't break.
    public void resetPlayerPrefs() { }
    public void add100000Gems() { }
}

