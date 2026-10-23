using UnityEngine;


public class InputGetter : MonoBehaviour
{
    [SerializeField]
    private int player = 0;

    public int controlledID = -1;

    private bool holding = false;

    private int tilt = -1;
    private int actionInt = -1;


    public int Health = 4;
    public int PotionCount = 3;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }


    void Update()
    {


        //// Green
        if (Input.GetButton($"J{player}_Attack"))
        {
            Debug.Log($"{player} Pressing Attack");
            actionInt = 0;
            holding = true;
            return;
        }
        //// Red
        if (Input.GetButton($"J{player}_Defend"))
        {
            Debug.Log($"{player} Pressing Defend");
            actionInt = 1;
            holding = true;
            return;
        }
        //// Yellow
        if (Input.GetButton($"J{player}_Heal"))
        {
            Debug.Log($"{player} Pressing Heal");
            holding = true;
            actionInt = 2;
            return;
        }
        if (holding)
        {
            CheckTilt();
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

        //// Orange
        //if (Input.GetKeyDown(KeyCode.JoystickButton5))
        //{
        //    Debug.Log($"DPAD upper was pressed!");
        //}


        if (tilt != -1 && !holding && actionInt != -1)
        {
            Debug.Log($"Player {player} applying action!");
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

        Debug.Log($"Player {player} action set! || Tilt {tilt} || Action {action}");
        holding = false;
        tilt = -1;
        actionInt = -1;
    }

    public void ChangeHealth(int newValue)
    {
        Health += newValue;
    }

    private void CheckTilt()
    {

        Debug.Log($"Player {player} selecting direction!");
        // Tilt DOWN
        if (Input.GetButton($"J{player}_DOWN") || Input.GetAxis($"J{player}_Tilt") < 0 )
        {
            tilt = 1;
            Debug.Log($"Player: {player} Selected {tilt}  SHOULD BE 1");
            holding = false;
            return;
        } 
        else if (Input.GetButton($"J{player}_UP") || Input.GetAxis($"J{player}_Tilt") > 0)
        {
            tilt = 2;
            holding = false;
            Debug.Log($"Player: {player} Selected {tilt}  SHOULD BE 2");
            return;
        }
        else
        {

            tilt = 0;
            Debug.Log($"Player: {player} Selected {tilt}  SHOULD BE 0");
        }
    }
}
