using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using System.Linq;
using System;

public class Blockbusters : MonoBehaviour {

    // Define the set of 20 answers that will be arranged on the 5x4 grid
    // Each hex cell will contain a clue formed from those letters in CAPITALS
    // in the list below
    private string[] answers = {
        "Sonic The Hedgehog",
        "Duke Nukem",
        "Shadow of the Colossus",
        "Mario",
        "Will White",
        "Nintendo",
        "California Games",
        "Super Mario Odyssey",
        "The Legend Of Zelda",
        "Super Monkey Ball",
        "Bulbasaur",
        "New Zealand",
        "Pikachu",
        "Samus Aran",
        "Sega Dreamcast",
        "Sega Nomad",
        "Duck Hunt",
        "Wild Gunman",
        "Lemmings",
        "James Pond"
    };


    private float flashSpeed = 2.0f;

    public Vector2 Offset;

    public Color solvedColour;

    public GameObject hexCell;
    public GameObject edgeHex;

    public Transform canvas;
    public float width;
    public float height;

    private Transform selectedCell;


    string MakeAcronym(string input) {
        var chars = input.Where(Char.IsUpper).ToArray();
        return new String(chars);
    }

    // Use this for initialization
    void Start () {

        var rectTransform = hexCell.GetComponent<RectTransform>();

        // These are the hexcells in the main game board itself
        for(int h=0; h<4; h++) {
            for(int w=0; w<5; w++) {
                GameObject temp = Instantiate(hexCell, new Vector3(h * height, w * width, 0), Quaternion.identity) as GameObject;
                temp.transform.SetParent(canvas, false);
                temp.transform.SetAsFirstSibling();
                temp.transform.GetComponentInChildren<Text>().text = MakeAcronym(answers[h*5 + w]);
                temp.GetComponent<Button>().onClick.AddListener(
                    () => { OnClick(temp.transform); }
                );
                temp.GetComponent<RectTransform>().localPosition = Offset + new Vector2(w * width, h * height - (w%2==0 ? 0 : height/2f));
            }
        }
        // These are the hex cells at the left and right edges
        for (int h = 0; h < 5; h++)
        {
            // LHS
            GameObject temp = Instantiate(edgeHex, new Vector3(h * height, -1 * width, 0), Quaternion.identity) as GameObject;
            temp.transform.SetParent(canvas, false);
            temp.transform.SetAsFirstSibling();
            temp.GetComponent<RectTransform>().localPosition = Offset + new Vector2(-1 * width, h * height - height / 2f);
            // RHS
            temp = Instantiate(edgeHex, new Vector3(h * height, 5 * width, 0), Quaternion.identity) as GameObject;
            temp.transform.SetParent(canvas, false);
            temp.transform.SetAsFirstSibling();
            temp.GetComponent<RectTransform>().localPosition = Offset + new Vector2(5 * width, h * height - height / 2f);
        }
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}



    public void OnClick(Transform t)
    {
        Debug.Log("You clicked on " + t.name);

        if(selectedCell == null)
        {
            Flash(t);
            selectedCell = t;
        }
        else if(selectedCell == t)
        {
            StopCoroutine(cr);
            t.GetChild(0).gameObject.SetActive(false);
            t.GetComponent<UIPolygon>().color = solvedColour;
            selectedCell.GetComponent<CanvasGroup>().alpha = 1;
            selectedCell = null;
        }
        else
        {
            StopCoroutine(cr);
            selectedCell.GetComponent<CanvasGroup>().alpha = 0;
            selectedCell = t;
            t.GetChild(0).gameObject.SetActive(true);
            t.GetComponent<UIPolygon>().color = new Color32(71,229,96,255);
            t.GetComponent<CanvasGroup>().alpha = 1;
            Flash(t);
        }



    }

    Coroutine cr;

    public void Flash(Transform t) {
        if(cr != null)
            StopCoroutine(cr);
        cr = StartCoroutine(Flash_CR(t));
    }

    private IEnumerator Flash_CR(Transform t) {
        CanvasGroup cG = t.GetComponent<CanvasGroup>();
        while (true) {
            while (cG.alpha > 0) {
                cG.alpha -= flashSpeed * Time.deltaTime;
                yield return null;
            }
            while (cG.alpha <= .9f) {
                cG.alpha += flashSpeed * Time.deltaTime;
                yield return null;
            }
        }   
    }
}
