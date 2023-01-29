using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManagerS : MonoBehaviour
{

    public GameObject tilePrefab;
    private GameObject newTile;
    public GameObject theSlider;
    public TextMeshProUGUI sizeText;
    float tempX;
    float tempY;
    float chance;
    public bool started;

    public int w;
    public int h;
    public MSTile[,] theGridio;
    public bool[,] empGridio;


    public Canvas startCanvas;
    public Canvas endCanvas;

    private void Start()
    {
        
    }

    public void Begin()
    {
        startCanvas.enabled = false;

        if (w % 2 == 0)
        {
            Camera.main.transform.position = new Vector3((float)-0.5, (float)-0.5, (float)-10);
        }

        theGridio = new MSTile[w, h];
        empGridio = new bool[w, h];

        for (int i = 0; i < w; i++)
        {
            tempX = i - (w / 2);
            for (int j = 0; j < h; j++)
            {
                tempY = j - (h / 2);
                newTile = Instantiate(tilePrefab, new Vector3(tempX, tempY, 0), Quaternion.identity);
                newTile.GetComponent<MSTile>().iCoord = i;
                newTile.GetComponent<MSTile>().jCoord = j;

                theGridio[i, j] = newTile.GetComponent<MSTile>();
            }
        }
        
    }

    public void setSize()
    {
        int dimes = (int)theSlider.GetComponent<Slider>().value;
        w = dimes;
        h = w;
        sizeText.text = w + " by " + h + "\n Layout";
    }

    public void generateMines(int safeX, int safeY)
    {
        for (int y = 0; y < 8; y++)
        {
            switch (y)
            {
                case 0: //Right
                    if (safeX + 1 != w) { theGridio[safeX + 1, safeY].mine = true; }
                    break;
                case 1: //Down Right
                    if ((safeX + 1 != w) && (safeY != 0)) { theGridio[safeX + 1, safeY - 1].mine = true; }
                    break;
                case 2: // Down
                    if (safeY != 0) { theGridio[safeX, safeY - 1].mine = true; }
                    break;
                case 3: //Down Left
                    if ((safeX != 0) && (safeY != 0)) { theGridio[safeX - 1, safeY - 1].mine = true; }
                    break;
                case 4: //Left
                    if (safeX != 0) { theGridio[safeX - 1, safeY].mine = true; }
                    break;
                case 5: //Up Left
                    if ((safeX != 0) && (safeY + 1 != h)) { theGridio[safeX - 1, safeY + 1].mine = true; }
                    break;
                case 6: // Up
                    if (safeY + 1 != h) { theGridio[safeX, safeY + 1].mine = true; }
                    break;
                case 7: //Up Right
                    if ((safeX + 1 != w) && (safeY + 1 != h)) { theGridio[safeX + 1, safeY + 1].mine = true; }
                    break;
            }
        }
            for (int i = 0; i < w; i++)
            {
                for(int j = 0; j < h; j++) {

                    if (!(theGridio[i, j].mine) && !(i == safeX && j == safeY))
                    {
                        chance = (float)0.20;
                        theGridio[i, j].mine = (Random.value < chance);
                    }
                    else
                    {
                        theGridio[i, j].mine = false;
                    }

                }
            }
    }

    public void revealMines()
    {
        foreach (MSTile spot in theGridio)
        {
            if (spot.mine) { spot.loadTexture(0); }
        }
    }

    public int getAdjCount(int a, int b)
    {
        int answer = 0;
        for (int i = 0; i < 8; i++)
        {
            switch (i)
            {
                case 0: //Right of tile
                    if (a + 1 != w) { if (theGridio[a + 1, b].mine) { answer++; } }
                    break;
                case 1: //Down Right
                    if ((a + 1 != w) && (b != 0)) { if (theGridio[a + 1, b - 1].mine) { answer++; } }
                    break;
                case 2: // Down
                    if (b != 0) { if (theGridio[a, b - 1].mine) { answer++; } }
                    break;
                case 3: //Down Left
                    if ((a != 0) && (b != 0)) { if (theGridio[a - 1, b - 1].mine) { answer++; } }
                    break;
                case 4: //Left of tile
                    if (a != 0) { if (theGridio[a - 1, b].mine) { answer++; } }
                    break;
                case 5: //Up Left
                    if ((a != 0) && (b + 1 != h)) { if (theGridio[a - 1, b + 1].mine) { answer++; } }
                    break;
                case 6: // Up
                    if (b + 1 != h) { if (theGridio[a, b + 1].mine) { answer++; } }
                    break;
                case 7: //Up Right
                    if ((a + 1 != w) && (b + 1 != h)) { if (theGridio[a + 1, b + 1].mine) { answer++; } }
                    break;
            }
        }
        return answer;
    }

    public void checkDone()
    {
        bool answer = true;
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                if (theGridio[i, j].mine)
                {
                    if (theGridio[i, j].GetComponent<SpriteRenderer>().sprite.texture.name != "flagtile")
                    {
                        answer = false;
                    }
                }
            }
        }
        if (answer)
        {
            endCanvas.enabled = true ;
        }
    }

    public void uncovEmpties(int a, int b)
    {
        if (empGridio[a, b]) { return; }

        if (theGridio[a,b].GetComponent<SpriteRenderer>().sprite.name == "flagtile") { return; }

        empGridio[a, b] = true;

        theGridio[a, b].loadTexture(0);

        if (getAdjCount(a, b) > 0) { return; }

        for (int i = 0; i < 8; i++)
        {
            switch (i)
            {
                case 0: //Right of tile
                    if (a + 1 != w) { uncovEmpties(a + 1, b); }
                    break;
                case 1: //Down Right
                    if ((a + 1 != w) && (b != 0)) { uncovEmpties(a + 1, b - 1); }
                    break;
                case 2: // Down
                    if (b != 0) { uncovEmpties(a, b - 1); }
                    break;
                case 3: //Down Left
                    if ((a != 0) && (b != 0)) { uncovEmpties(a - 1, b - 1); }
                    break;
                case 4: //Left of tile
                    if (a != 0) { uncovEmpties(a - 1, b); }
                    break;
                case 5: //Up Left
                    if ((a != 0) && (b + 1 != h)) { uncovEmpties(a - 1, b + 1); }
                    break;
                case 6: // Up
                    if (b + 1 != h) { uncovEmpties(a, b + 1); }
                    break;
                case 7: //Up Right
                    if ((a + 1 != w) && (b + 1 != h)) { uncovEmpties(a + 1, b + 1); }
                    break;
            }
        }

    }

    public void showSafeStart()
    {
        MSTile safeStart = null;
        foreach (MSTile spot in theGridio)
        { //Finds a safe starting tile close to center
            if (spot.iCoord != 0 && spot.iCoord != w - 1 && spot.jCoord != 0 && spot.jCoord != h - 1)
            {
                if (getAdjCount(spot.iCoord, spot.jCoord) == 0 && spot.mine == false)
                {
                    safeStart = spot;
                }
            }
            if (spot.iCoord > h / 4)
            {
                if (Random.value > .75 && safeStart != null) { break; }
            }
        }
        if (safeStart == null)
        {
            foreach (MSTile spot in theGridio) //Finds any safe starting tile only triggered if previous loop fails
            {
                if (getAdjCount(spot.iCoord, spot.jCoord) == 0 && spot.mine == false)
                {
                    safeStart = spot;
                }
                if (Random.value > .15 && safeStart != null) { break; }
            }
        }
        if (safeStart != null)
        {
            safeStart.loadTexture(1);
        }
    }

}