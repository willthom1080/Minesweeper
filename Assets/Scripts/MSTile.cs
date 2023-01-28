using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSTile : MonoBehaviour
{
    public bool mine;
    public GameObject theManager;
    public GameManagerS theAstManager;
    public int iCoord;
    public int jCoord;
    public Sprite[] emptyTextures;
    public Sprite mineTexture;
    public Sprite flagTexture;
    public Sprite[] tileTexture;

    void Start()
    {
        theManager = GameObject.FindGameObjectWithTag("GameController");
        theAstManager = theManager.GetComponent<GameManagerS>();
    }

    void OnMouseUpAsButton()
    {
        if (theAstManager.started)
        {
            if (isCovered())
            {
                if (mine)
                {
                    theAstManager.revealMines();
                }
                else
                {
                    loadTexture(0);
                }
            }
        }
        else
        {
            theAstManager.generateMines(iCoord, jCoord);
            loadTexture(0);
            theAstManager.started = true;
        }
    }

    void OnMouseOver()
    {
        //Use for non left mouse button clicks
        if (Input.GetMouseButtonDown(1))
        {
            if (GetComponent<SpriteRenderer>().sprite == tileTexture[0] || GetComponent<SpriteRenderer>().sprite == flagTexture)
            {
                flagPlant();
            }
        }

    }

    void flagPlant()
    {
        if (GetComponent<SpriteRenderer>().sprite.texture.name == "flagtile")
        {
            GetComponent<SpriteRenderer>().sprite = tileTexture[0];
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = flagTexture;
            theAstManager.checkDone();
        }
    }

    public void loadTexture(int num)
    {
        theManager = GameObject.FindGameObjectWithTag("GameController");
        theAstManager = theManager.GetComponent<GameManagerS>();
        if (mine)
            GetComponent<SpriteRenderer>().sprite = mineTexture;
        else
        {
            
            GetComponent<SpriteRenderer>().sprite = emptyTextures[theAstManager.getAdjCount(iCoord, jCoord)];
            if (GetComponent<SpriteRenderer>().sprite == emptyTextures[0])
            {
                theAstManager.uncovEmpties(iCoord, jCoord);
            }
        }

    }

    public bool isCovered()
    {
        return GetComponent<SpriteRenderer>().sprite == tileTexture[0];
    }
}