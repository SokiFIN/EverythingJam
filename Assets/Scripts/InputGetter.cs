using UnityEngine;
using static GameManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class InputGetter : MonoBehaviour
{
    [SerializeField]
    private int player = 0;

    [SerializeField]
    private GameObject ownCursorSprite;
    [SerializeField]
    private GameObject ownSprite;

    [SerializeField]
    private List<GameObject> healthSprites = new();

    private Color ogColor;
    private Color healColor = Color.forestGreen;
    private Color damageColor = Color.indianRed;

    public int controlledID = -1;

    private bool holding = false;

    private int tilt = -1;
    private int actionInt = -1;


    public int Health = 4;
    private int maxHealth = 4;
    public int PotionCount = 3;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(TurnIndicator());
        ogColor = ownSprite.GetComponent<SpriteRenderer>().color;
    }

    private IEnumerator TurnIndicator()
    {
        while (!GameManager.Instance.GameEnd)
        {

            if (GameManager.Instance.p1PlannedAction == Actions.NOT_SELECTED && player == 0)
            {
                ownCursorSprite.SetActive(true);
            }

            if (GameManager.Instance.p2PlannedAction == Actions.NOT_SELECTED && player == 1)
            {

                ownCursorSprite.SetActive(true);
            }
            yield return new WaitForSeconds(0.4f);

            if (player == 0)
            {
                ownCursorSprite.SetActive(false);
            }

            if (player == 1)
            {

                ownCursorSprite.SetActive(false);
            }

            yield return new WaitForSeconds(0.2f);
        }


        // Fail safe if they stay on
        if (player == 0)
        {
            ownCursorSprite.SetActive(false);
        }

        if (player == 1)
        {

            ownCursorSprite.SetActive(false);
        }
    }



    void Update()
    {

        // Dont know the button yet.
        if (Input.GetKeyDown(KeyCode.JoystickButton7) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SceneManager.LoadScene(0);
        }

        if (GameManager.Instance.GameEnd)
        {
            return;
        }

        //// Green
        if (Input.GetButton($"J{player}_Attack"))
        {
            actionInt = 0;
            CheckTilt();
            return;
        }
        //// Red
        if (Input.GetButton($"J{player}_Defend"))
        {
            actionInt = 1;
            CheckTilt();
            return;
        }
        //// Yellow
        if (Input.GetButton($"J{player}_Heal"))
        {
            tilt = 1;
            actionInt = 2;
            CheckTilt();
            return;
        }

        //// Blue
        //if (Input.GetKeyDown(KeyCode.JoystickButton2))
        //{
        //    Debug.Log($"Blue was pressed!");
        //}
        //// Orange
        //if (Input.GetKeyDown(KeyCode.JoystickButton4))
        //{
        //    Debug.Log($"Orange was pressed!");
        //}




        if (tilt != -1 && actionInt != -1)
        {
            ApplyAction();
        }


    }

    private void ApplyAction()
    {
        GameManager.Actions action = GameManager.Actions.ATTACK;

        switch (actionInt)
        {
            case 0:
                action = GameManager.Actions.ATTACK;
                break;
            case 1:
                action = GameManager.Actions.SHIELD;
                break;
            case 2:
                if (PotionCount > 0)
                {
                    action = GameManager.Actions.HEAL;
                }
                else
                {
                    actionInt = -1;
                    return;
                }

                break;
        }

        switch (player)
        {
            case 0:
                GameManager.Instance.p1ReadyMarker.SetActive(true);
                GameManager.Instance.p1PlannedAction = action;
                GameManager.Instance.p1Direction = tilt;
                break;
            case 1:
                GameManager.Instance.p2ReadyMarker.SetActive(true);
                GameManager.Instance.p2PlannedAction = action;
                GameManager.Instance.p2Direction = tilt;
                break;
        }

        holding = false;
        tilt = -1;
        actionInt = -1;
    }

    public void ChangeHealth(int newValue, bool damage)
    {

        Health = Mathf.Clamp(Health + newValue, 0, maxHealth);

        if (damage)
        {
            for (int i = Health; i < Health - newValue; i++)
            {
                if (i >= 0 && i < healthSprites.Count)
                {
                    healthSprites[i].SetActive(false);
                }
            }
        }
        else
        {
            
            for (int i = 0; i < healthSprites.Count; i++)
            {
                healthSprites[i].SetActive(i < Health);
            }
        }

        StartCoroutine(VisualizeHealth(damage));

    }

    private IEnumerator VisualizeHealth(bool damage)
    {
        if (damage)
        {
            ownSprite.GetComponent<SpriteRenderer>().color = damageColor;

        }
        else
        {
            ownSprite.GetComponent<SpriteRenderer>().color = healColor;
        }
        yield return new WaitForSeconds(0.2f);
        ownSprite.GetComponent<SpriteRenderer>().color = ogColor;
    }

    private void CheckTilt()
    {

        // Tilt DOWN
        if (Input.GetButton($"J{player}_DOWN") || Input.GetAxis($"J{player}_Tilt") < 0 )
        {
            tilt = 0;
            holding = false;
            return;
        } 
        else if (Input.GetButton($"J{player}_UP") || Input.GetAxis($"J{player}_Tilt") > 0)
        {
            tilt = 2;
            holding = false;
            return;
        }
        else
        {
            tilt = 1;
        }
    }
}
