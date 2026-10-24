using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    private List<GameObject> p1Arms;
    [SerializeField]
    private GameObject p1DefArms;

    [SerializeField]
    private GameObject p2DefArms;


    [SerializeField]
    private List<GameObject> p2Arms;

    [SerializeField]
    private GameObject endHint;

    private AudioSource aS;

    [SerializeField]
    private List<AudioClip> clips;

    [SerializeField]
    private List<InputGetter> Players;
    [SerializeField]
    public GameObject healthsParent;
    [SerializeField]
    private TMP_Text infoText;
    [SerializeField]
    private List<GameObject> p1DirIndicators;
    [SerializeField]
    private List<GameObject> p2DirIndicators;

    private SpriteRenderer p1SpriteRenderer;
    private SpriteRenderer p2SpriteRenderer;

    private Color damageColor = Color.red;
    private Color healColor = Color.forestGreen;
    private Color shieldColor = Color.darkViolet;
    private string[] controllers;

    private List<string> texts = new() { "Choose an action!", "Showing Results!"};

    private bool results = false;

    public enum Actions
    {
        ATTACK,
        SHIELD,
        HEAL,
        NOT_SELECTED
    }

    public Actions p1PlannedAction = Actions.NOT_SELECTED; 
    public int p1Direction = 0; 
    public Actions p2PlannedAction = Actions.NOT_SELECTED; 
    public int p2Direction = 0; 

    public bool GameEnd = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
 
    }

    private IEnumerator InfoTextBlink()
    {
        while (results)
        {
            if (GameEnd)
            {
                endHint.SetActive(true);
            }

            infoText.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.4f);

            infoText.gameObject.SetActive(false);

            if (GameEnd)
            {
                endHint.SetActive(false);
            }
            yield return new WaitForSeconds(0.3f);
        }
        infoText.gameObject.SetActive(true);
    }

    private void Start()
    {
        controllers = Input.GetJoystickNames();

        if (controllers.Length >= 1) 
        {
            Players[0].controlledID = 1;
            //Debug.Log($"Assigned controller {controllers[0]} to P1");
            if (controllers.Length >= 2)
            {
                Players[1].controlledID = 2;
                //Debug.Log($"Assigned controller {controllers[1]} to P2");
            }
        }


        StartCoroutine(Setup());
        aS = GetComponent<AudioSource>();
        
    }

    private Color GetActionColor(int player)
    {
        Color color = Color.white;

        switch (player)
        {
            case 0:
                if (p1PlannedAction == Actions.ATTACK) color = damageColor;
                if (p1PlannedAction == Actions.SHIELD) color = shieldColor;
                if (p1PlannedAction == Actions.HEAL) color = healColor;
                break;
            case 1:
                if (p2PlannedAction == Actions.ATTACK) color = damageColor;
                if (p2PlannedAction == Actions.SHIELD) color = shieldColor;
                if (p2PlannedAction == Actions.HEAL) color = healColor;
                break;
        }
        return color;
    }

    private IEnumerator Setup()
    {
        var waitTime = 0.5f;

        infoText.text = texts[0];

        // Wait until both are ready
        yield return new WaitUntil(() => p1PlannedAction != Actions.NOT_SELECTED && p2PlannedAction != Actions.NOT_SELECTED);

        results = true;
        infoText.text = texts[1];
        StartCoroutine(InfoTextBlink());


        // Direction indicator color & Direction show active
        var p1SpriteRenderer = p1DirIndicators[p1Direction].GetComponent<SpriteRenderer>();
        var p2SpriteRenderer = p2DirIndicators[p2Direction].GetComponent<SpriteRenderer>();
        yield return new WaitForSeconds(1f);
        p1SpriteRenderer.color = GetActionColor(0);
        p1DirIndicators[p1Direction].SetActive(true);
        p1DefArms.SetActive(false);
        p1Arms[p1Direction].SetActive(true);
        yield return new WaitForSeconds(1f);
        p2SpriteRenderer.color = GetActionColor(1);
        p2DirIndicators[p2Direction].SetActive(true);
        p2DefArms.SetActive(false);
        p2Arms[p2Direction].SetActive(true);
        yield return new WaitForSeconds(1f);


        // Heal actions.
        if (p1PlannedAction == Actions.HEAL)
        {
            yield return new WaitForSeconds(waitTime);
            p1PlannedAction = Actions.NOT_SELECTED;
            Players[0].ChangeHealth(2, false);
            Players[0].PotionCount -= 1;
            PlayAudio(2);

        }
        if (p2PlannedAction == Actions.HEAL)
        {
            yield return new WaitForSeconds(waitTime);
            p2PlannedAction = Actions.NOT_SELECTED;
            Players[1].ChangeHealth(2, false);
            Players[1].PotionCount -= 1;
            PlayAudio(2);
        }

        yield return new WaitUntil(() => !aS.isPlaying);

        // Shield actions.
        if (p1PlannedAction == Actions.SHIELD && p2PlannedAction == Actions.ATTACK)
        {
            yield return new WaitForSeconds(waitTime);
            if (p1Direction == p2Direction)
            {
                Players[1].ChangeHealth(-3, true);
                PlayAudio(1);
            }
            else
            {
                Players[0].ChangeHealth(-2, true);
                PlayAudio(0);
            }

        }
        if (p2PlannedAction == Actions.SHIELD && p1PlannedAction == Actions.ATTACK)
        {
            yield return new WaitForSeconds(waitTime);
            if (p1Direction == p2Direction)
            {
                Players[0].ChangeHealth(-3, true);
                PlayAudio(1);
            }
            else
            {
                Players[1].ChangeHealth(-2, true);
                PlayAudio(0);
            }

        }
        yield return new WaitUntil(() => !aS.isPlaying);

        // Attack actions.
        if (p1PlannedAction == Actions.ATTACK && p2PlannedAction != Actions.SHIELD)
        {
            yield return new WaitForSeconds(waitTime);
            Players[1].ChangeHealth(-1, true);
            PlayAudio(0);
        }
        if (p2PlannedAction == Actions.ATTACK && p1PlannedAction != Actions.SHIELD)
        {
            yield return new WaitForSeconds(waitTime);
            Players[0].ChangeHealth(-1, true);
            PlayAudio(0);
        }
        yield return new WaitUntil(() => !aS.isPlaying);

        // Reset everything
        p1DirIndicators[p1Direction].SetActive(false);
        p2DirIndicators[p2Direction].SetActive(false);
        p1Arms[p1Direction].SetActive(false);
        p2Arms[p2Direction].SetActive(false);
        p1Direction = 0;
        p2Direction = 0;
        p1DefArms.SetActive(true);
        p2DefArms.SetActive(true);
        p1PlannedAction = Actions.NOT_SELECTED;
        p2PlannedAction = Actions.NOT_SELECTED;

        // Check if either player has won otherwise continue.
        if (Players[0].Health > 0 && Players[1].Health > 0)
        {
            StartCoroutine(Setup());
        }
        else
        {

            healthsParent.SetActive(false);

            if (Players[0].Health <= 0 && Players[1].Health <= 0)
            {
                infoText.text = "TIE";
                infoText.color = Color.green;

                Players[0].gameObject.SetActive(false);
                Players[1].gameObject.SetActive(false);

                GameEnd = true;
            }
            else if(Players[0].Health <= 0)
            {
                infoText.text = "Player 2 WINS!";
                Players[0].gameObject.SetActive(false);
                Players[1].transform.position = new Vector2();
                infoText.color = Color.pink;
                GameEnd = true;
            }
            else
            {
                infoText.text = "Player 1 WINS!";
                Players[1].gameObject.SetActive(false);
                Players[0].transform.position = new Vector2();
                infoText.color = Color.lightBlue;
                GameEnd = true;
            }
        }


        if (!GameEnd)
        {
            results = false;
        }
        yield return new WaitForSeconds(3f);


    }

    public void PlayAudio(int audioClip)
    {
        aS.PlayOneShot(clips[audioClip]);
    }
}
