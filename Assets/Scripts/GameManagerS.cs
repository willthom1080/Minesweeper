using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerS : MonoBehaviour
{

    public GameObject tilePrefab;
    private GameObject newTile;
    float tempX;
    float tempY;

    public static int w = 8;
    public static int h = 8;
    public static MSTile[,] theGridio = new MSTile[w, h];
    public bool[,] empGridio = new bool[w, h];

    void Start()
    {
        //

        for (int i = 0; i < w; i++)
        {
            tempX = i - (w / 2);
            for (int j = 0; j < h; j++)
            {
                tempY = j - (h / 2);
                newTile = Instantiate(tilePrefab, new Vector3(tempX, tempY, 0), Quaternion.identity);
                newTile.GetComponent<MSTile>().mine = Random.value < 0.15;
                newTile.GetComponent<MSTile>().iCoord = i;
                newTile.GetComponent<MSTile>().jCoord = j;

                theGridio[i, j] = newTile.GetComponent<MSTile>();
            }
        }
        showSafeStart();
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
            for (int j = 0; i < h; i++)
            {
                if (theGridio[i, j].mine)
                {
                    if (theGridio[i, j].GetComponent<SpriteRenderer>().sprite.texture.name != "FlagTile")
                    {
                        answer = false;
                    }
                }
            }
        }
        if (answer)
        {
            Debug.Log("you win");
            SceneManager.LoadScene("Overworld");
        }
    }

    public void uncovEmpties(int a, int b)
    {
        if (empGridio[a, b]) { return; }

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