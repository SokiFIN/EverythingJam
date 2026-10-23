using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    [SerializeField]
    private List<InputGetter> Players;
    [SerializeField]
    public GameObject p1ReadyMarker;
    [SerializeField]
    public GameObject p2ReadyMarker;
    [SerializeField]
    private TMP_Text infoText;
    private string[] controllers;

    private List<string> texts = new() { "Choose an action!", "Showing Results!"};

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

    private bool gameEnd = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
    }

    private IEnumerator InfoTextBlink()
    {
        while (!gameEnd)
        {
            infoText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            yield return new WaitForSeconds(0.2f);
            infoText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void Start()
    {
        controllers = Input.GetJoystickNames();
        for (int i = 0; i < controllers.Length; i++)
        {
            Debug.Log("Controller index " + i + ": " + controllers[i]);
        }
        StartCoroutine(Setup());
        StartCoroutine(InfoTextBlink());
        
    }


    private IEnumerator Setup()
    {
        Debug.Log("Setup waiting for actions");
        infoText.text = texts[0];
        p1ReadyMarker.SetActive(false);
        p2ReadyMarker.SetActive(false);
        yield return new WaitUntil(() => p1PlannedAction != Actions.NOT_SELECTED && p2PlannedAction != Actions.NOT_SELECTED);

        Debug.Log("Initiating results!");
        infoText.text = texts[1];
        // Heal.
        if (p1PlannedAction == Actions.HEAL)
        {
            p1PlannedAction = Actions.NOT_SELECTED;
            Players[0].ChangeHealth(2);
            Players[0].PotionCount -= 1;
        }
        if (p2PlannedAction == Actions.HEAL)
        {
            p2PlannedAction = Actions.NOT_SELECTED;
            Players[1].ChangeHealth(2);
            Players[1].PotionCount -= 1;
        }

        // Shield.
        if (p1PlannedAction == Actions.SHIELD && p2PlannedAction == Actions.ATTACK)
        {
            if (p1Direction == p2Direction)
            {
                Players[1].ChangeHealth(-3);
            }
            else
            {
                Players[0].ChangeHealth(-2);
            }

        }
        if (p2PlannedAction == Actions.SHIELD && p1PlannedAction == Actions.ATTACK)
        {
            if (p1Direction == p2Direction)
            {
                Players[0].ChangeHealth(-3);
            }
            else
            {
                Players[1].ChangeHealth(-2);
            }

        }

        if (p1PlannedAction == Actions.ATTACK)
        {
            Players[0].ChangeHealth(-1);
        }
        if (p2PlannedAction == Actions.ATTACK)
        {
            Players[1].ChangeHealth(-1);
        }

        p1Direction = 0;
        p2Direction = 0;
        p1PlannedAction = Actions.NOT_SELECTED;
        p2PlannedAction = Actions.NOT_SELECTED;
        p1ReadyMarker.gameObject.SetActive(false);
        p2ReadyMarker.gameObject.SetActive(false);

        if (Players[0].Health > 0 && Players[1].Health > 0)
        {
            StartCoroutine(Setup());
        }
        else
        {
            if (Players[0].Health <= 0)
            {
                infoText.text = "Player 1 WINS!";
            }
            else
            {
                infoText.text = "Player 2 WINS!";
            }
        }

    }
}
