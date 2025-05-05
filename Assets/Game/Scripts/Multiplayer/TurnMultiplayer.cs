using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnMultiplayer : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject Piece;
    public string TurnPlayer="";
    public bool Liberate=false;

    [SerializeField]
    AudioSource auChangeTurn;

    bool bVictory=false;
    bool bWin = false;

    int iPlayerCountVictory = 0;
    int iEnemyCountVictory = 0;

    bool bLoad = false;

    private void Start()
    {
        TurnPlayer = "Player";
        Liberate = true;

    }

    private void Update()
    {
        VictoryVerify();
    }

    public void SetPiece(GameObject piece)
    {
        Piece = piece;
    }

    public GameObject GetPiece()
    {
        return Piece;
    }

    public bool bChangeTurn = false;

    public bool bSuicid = false;

    public void ChangeTurn()
    {

        if (bLoad)
        {
            if (!bWin)
            {
                if (!bChangeTurn)
                {
                    bChangeTurn = true;
                    IEnumerator enumerator = IEChangeTurn(1.5f);
                    StartCoroutine(enumerator);
                    Debug.Log("Turn ChangeTurn - bChangeTurn = " + bChangeTurn);
                }
            }
        }

    }

    private IEnumerator IEChangeTurn(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        //if (Piece.GetComponent<Player>().tag == "Player")        

        Debug.Log("TurnPlayer = " + TurnPlayer);
        PieacesCount();

        if (!bSuicid)
        {
            if (TurnPlayer == "Player")
            {
                TurnPlayer = "Enemy";
            }
            else
            {
                TurnPlayer = "Player";
                auChangeTurn.Play();
            }

            bChangeTurn = false;

            IEnumerator enumerator = IELiberateTurn(0.5f);
            StartCoroutine(enumerator);
        }
    }

    private IEnumerator IELiberateTurn(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        KillStuckPiece();
        Liberate = true;

        
    }

    public void PieacesCount()
    {
        int iPlayerCount = 0;
        int iEnemyCount = 0;


        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in Enemies)
        {
            if (enemy.GetComponent<PlayerMultiplayer>().Types.ToString() != "Bandeira")
            {
                if (enemy.GetComponent<PlayerMultiplayer>().Types.ToString() != "Bomba")
                {
                    //enemy.GetComponent<Player>().SetDie();                
                    iEnemyCount++;
                }
            }

        }        

        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in Players)
        {
            if (player.GetComponent<PlayerMultiplayer>().Types.ToString() != "Bandeira")
            {
                if (player.GetComponent<PlayerMultiplayer>().Types.ToString() != "Bomba")
                {
                    //player.GetComponent<Player>().SetDie();                
                    iPlayerCount++;
                }
            }
        }


        //Debug.Log("player iPlayerCount = " + iPlayerCount);
        //Debug.Log("enemy iEnemyCount = " + iEnemyCount);        

        iPlayerCountVictory = iPlayerCount;
        iEnemyCountVictory = iEnemyCount;

        //Debug.Log("enemy iPlayerCountVictory = " + iPlayerCountVictory);
        //Debug.Log("player iEnemyCountVictory = " + iEnemyCountVictory);

    }

    void VictoryVerify()
    {
        if (bLoad)
        {
            if (!bVictory)
            {
                if (iEnemyCountVictory == 0)
                {

                    bWin = true;
                    Piece = null;

                    GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");

                    foreach (GameObject player in Players)
                    {
                        player.GetComponent<PlayerMultiplayer>().SetVictory();
                    }

                    bVictory = true;
                }

                if (iPlayerCountVictory == 0)
                {
                    bWin = true;
                    Piece = null;

                    GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");

                    foreach (GameObject enemy in Enemies)
                    {
                        enemy.GetComponent<PlayerMultiplayer>().SetVictory();
                    }

                    bVictory = true;
                }
            }
        }
    }

    public void SetVictory()
    {
        bWin = true;
    }

    public void LoadPieces()
    {
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");

        iPlayerCountVictory = Players.Length;
        iEnemyCountVictory = Enemies.Length;

        bLoad = true;
    }

    private void KillStuckPiece()
    {
        if(TurnPlayer == "Player")
        {
            Debug.Log("KillStuckPiece - iPlayerCountVictory = " + iPlayerCountVictory);
            if(iPlayerCountVictory==1)
            {
                GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");

                foreach (GameObject player in Players)
                {
                    if (player.GetComponent<PlayerMultiplayer>().Types.ToString() != "Bandeira")
                    {
                        if (player.GetComponent<PlayerMultiplayer>().Types.ToString() != "Bomba")
                        {
                            int[] iHousesFree = player.GetComponent<PlayerMultiplayer>().CheckHouses();
                            Debug.Log("KillStuckPiece - iHousesFree.Length = " + iHousesFree.Length);

                            if (iHousesFree.Length==0)
                            {
                                bSuicid = true;
                                player.GetComponent<PlayerMultiplayer>().SetSuicide();
                            }
                        }
                    }
                }
            }            
        }

        if (TurnPlayer == "Enemy")
        {
            if (iEnemyCountVictory == 1)
            {
                GameObject[] Players = GameObject.FindGameObjectsWithTag("Enemy");

                foreach (GameObject player in Players)
                {
                    if (player.GetComponent<PlayerMultiplayer>().Types.ToString() != "Bandeira")
                    {
                        if (player.GetComponent<PlayerMultiplayer>().Types.ToString() != "Bomba")
                        {
                            int[] iHousesFree = player.GetComponent<PlayerMultiplayer>().HousesFree();
                            Debug.Log("KillStuckPiece - iHousesFree.Length = " + iHousesFree.Length);

                            if (iHousesFree.Length == 0)
                            {
                                bSuicid = true;
                                player.GetComponent<PlayerMultiplayer>().SetSuicide();
                            }
                        }
                    }
                }
            }
        }

    }
}
