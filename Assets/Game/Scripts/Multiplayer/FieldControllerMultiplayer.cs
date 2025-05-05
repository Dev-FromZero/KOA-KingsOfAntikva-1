using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldControllerMultiplayer : MonoBehaviour
{
    // Start is called before the first frame update
    public string NickName;
    public string ColumnName;
    public int Index;
    public int Row;
    public int Column;
    public Transform Target;

    public bool Status=false;
    public bool Busy;

    public bool AttackMode = false;

    public GameObject BusyPiece;

    [SerializeField]
    TextMesh TxtForce;

    GameObject VisualActive;

    TurnMultiplayer turn;

    AudioSource Confirm;

    GameMode gamemode;

    int iGameMode = 0;

    bool bPlayer = false;

    void Start()
    {
        if (GameObject.Find("Confirm"))
        {
            Confirm = GameObject.Find("Confirm").GetComponent<AudioSource>();
        }

        if (transform.Find("Preview"))
        {
            VisualActive = transform.Find("Preview").gameObject;
            VisualActive.SetActive(false);
        }       

        turn = FindObjectOfType<TurnMultiplayer>();

        gamemode = FindObjectOfType<GameMode>();
        iGameMode = gamemode.GetGameType();
    }

    public void SetTextForce(string force)
    {
        //TxtForce.gameObject.SetActive(true);
        TxtForce.text = force;
    }

    // Update is called once per frame
    void Update()
    {
        if (Busy)
        {
            if (BusyPiece)
            {
                if (BusyPiece.tag == "Player")
                {
                    bPlayer = true;
                }
                else
                {
                    bPlayer = false;
                }
            }
        }

        if (bPlayer)
        {           

            if (Busy)
            {
                if (BusyPiece)
                {
                    string sForceType = BusyPiece.GetComponent<PlayerMultiplayer>().GetForceType();

                    if (sForceType != "F")
                    {
                        if (sForceType != "B")
                        {
                            if (!TxtForce.gameObject.activeSelf)// && turn.Liberate)
                            {
                                TxtForce.gameObject.SetActive(true);
                                TxtForce.text = BusyPiece.GetComponent<PlayerMultiplayer>().GetForceType();
                            }
                        }
                    }
                }
            }
            else
            {
                TxtForce.gameObject.SetActive(false);
            }
                
        }
        else
        {
            if(iGameMode == 1)
            {                
                if (Busy)
                {
                    if (!TxtForce.gameObject.activeSelf)// && turn.Liberate)
                    {
                        TxtForce.gameObject.SetActive(true);
                        TxtForce.text = BusyPiece.GetComponent<PlayerMultiplayer>().GetForceType();
                    }
                }
                else
                {
                    TxtForce.gameObject.SetActive(false);
                }
            }
            else if (iGameMode == 2)
            {
                if (Busy)
                {
                    //if (!TxtForce.gameObject.activeSelf)
                    //{
                        if (BusyPiece.GetComponent<PlayerMultiplayer>().isDie())
                        {
                            TxtForce.gameObject.SetActive(true);
                            TxtForce.text = BusyPiece.GetComponent<PlayerMultiplayer>().GetForceType();
                        }
                    //}
                }
                else
                {
                    TxtForce.gameObject.SetActive(false);
                }
            }
            else
            {
                TxtForce.gameObject.SetActive(false);
            }

        }

    }

    private void OnMouseDown()
    {
        if (turn.TurnPlayer == "Player")
        {
            if (Status && !AttackMode)
            {
                Debug.Log("Selected field: " + name);
                Debug.Log("Selected NickName: " + NickName);
                Debug.Log("Index field: " + Index);
                Debug.Log("Column field: " + Column);
                Debug.Log("Row field: " + Row);
                Debug.Log("Piece Type: " + turn.Piece.GetComponent<PlayerMultiplayer>().Types.ToString());

                Selection();

            }
        }

        if (turn.TurnPlayer == "Enemy")
        {
            if (Status && !AttackMode)
            {
                Debug.Log("Selected field: " + name);
                Debug.Log("Selected NickName: " + NickName);
                Debug.Log("Index field: " + Index);
                Debug.Log("Column field: " + Column);
                Debug.Log("Row field: " + Row);
                Debug.Log("Piece Type: " + turn.Piece.GetComponent<PlayerMultiplayer>().Types.ToString());

                Selection();

            }
        }

    }

    public void Selection()
    {
        turn.Liberate = false;

        turn.Piece.GetComponent<PlayerMultiplayer>().MoveStart(Target);

        turn.Piece.GetComponent<PlayerMultiplayer>().CancelMovement();

        turn.Piece.GetComponent<PlayerMultiplayer>().iFieldLive = Index;
    }

    public void SetStatus(bool status)
    {
        Status = status;

        if(Status)
        {
            if (turn.TurnPlayer != "Enemy")
            {
                VisualActive.SetActive(true);
            }
        }
        else
        {
            VisualActive.SetActive(false);
            AttackMode = false;
        }

        if (Status)
        {
            if (turn.TurnPlayer != "Player")
            {
                VisualActive.SetActive(true);
            }
        }
        else
        {
            VisualActive.SetActive(false);
            AttackMode = false;
        }

    }



}
