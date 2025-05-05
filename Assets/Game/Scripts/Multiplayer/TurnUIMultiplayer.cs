using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnUIMultiplayer : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    GameObject gTurnOn;
    [SerializeField]
    GameObject gTurnOff;

    [SerializeField]
    AudioSource auTurnChange;

    TurnMultiplayer turn;
    
    void Start()
    {
        turn = FindObjectOfType<TurnMultiplayer>();
    }

    // Update is called once per frame
    void Update()
    {

        if(turn.Liberate && turn.TurnPlayer == "Player")
        {
            gTurnOn.SetActive(true);
            gTurnOff.SetActive(false);
        }
        else
        {
            gTurnOn.SetActive(false);
            gTurnOff.SetActive(true);
        }
    }


}
