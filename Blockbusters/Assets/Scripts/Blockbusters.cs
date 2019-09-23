using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using System.Linq;
using System;

public class Blockbusters : MonoBehaviour {

    private enum GameState { StartScreen, Playing, Ended };
    GameState gameState = GameState.StartScreen;

    [SerializeField]
    private Text timerText;
    private float timeRemaining = 60f;


    /*What is the subtitle of the second South Park game, sequel to The Stick of Truth?
TFBW
South Park : The Fractured But Whole


In Ocarina of Time, what item can be found in the Spirit Temple?
Mirror Shield

Lavos is the final boss from which game?
Chrono Trigger

In what fictional city did Resident Evil 1,2 and 3 take place?
Raccoon City

Who is the main character of the Half Life series?
Gordon Freeman

TDBM In Overwatch, what does the character Genji say when using their ultimate? The Dragon Becomes Me

GF What is the adventure game set in the Land of the Dead, published by LucasArts in 1998 Grim Fandango

AT What is the subtitle of the Uncharted 2?
Among Thieves*/

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

    [Header("Audio")]
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip successMusic;
    [SerializeField]
    private AudioClip inGameMusic;
    [SerializeField]
    private AudioClip failureMusic;


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
            temp.GetComponent<Button>().onClick.AddListener(
                    () => { OnSuccess(); }
                );
        }
        
    }
	
	// Update is called once per frame
	void Update () {
		if(gameState == GameState.Playing) {
            timeRemaining -= Time.deltaTime;
        }
        if(timeRemaining <0 ) {
            timeRemaining = 0;
            OnFailure();
        }
        timerText.text = timeRemaining.ToString("F0"); 

    }


    public void OnStartGame()
    {
        gameState = GameState.Playing;
        audioSource.clip = inGameMusic;
        audioSource.Play();
    }

    public void OnSuccess()
    {
        gameState = GameState.Ended;
        audioSource.clip = successMusic;
        audioSource.Play();
    }

    public void OnFailure()
    {
        gameState = GameState.Ended;
        audioSource.clip = failureMusic;
        audioSource.Play();
    }


    public void OnClick(Transform t)
    {
        Debug.Log("You clicked on " + t.name);

        // Clicking on a new cell
        if(selectedCell == null) {

            if(gameState == GameState.StartScreen)
            {
                OnStartGame();
            }

            Flash(t);
            selectedCell = t;
        }
        else if(selectedCell == t) {
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
