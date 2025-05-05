using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrdersPartsMultiplayer : MonoBehaviour
{
    // Start is called before the first frame update
    public Playsystems.Table table;

    public GameObject[] Pieces;

    public GameObject[] Fields;    

    public bool bPlayer = true;

    [SerializeField]
    bool bEdit = false;

    void Start()
    {
        if (bEdit)
        {
            StartCoroutine(OrderEdit(0.5f));
        }
        else
        {
            //StartCoroutine(Order(0.5f));
            if (bPlayer)
            {
                StartCoroutine(StartLoadPieces());
            }
            else
            {
                StartCoroutine(StartLoadEnemyPieces());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int iPiecesRecord = 0;

    private IEnumerator StartLoadPieces()
    {
        while (iPiecesRecord==0)
        {
            if (table.Loaded())
            {
                iPiecesRecord = table.Count();
                LoadPieces(iPiecesRecord);                
                Debug.Log("table Count = " + table.Count());

            }
            yield return null;
        }
    }

    string[,] aOrderEnemies;

    private IEnumerator StartLoadEnemyPieces()
    {
        aOrderEnemies = OrderEnemy();

        while (iPiecesRecord == 0)        
        {            
            iPiecesRecord = aOrderEnemies.Length;            
            LoadPieces((iPiecesRecord/2)); ;
            Debug.Log("iPiecesRecord = " + iPiecesRecord);
            
            yield return null;
        }
    }

    private void LoadPieces(int length)
    {
        //Pieces = new GameObject[length-1];

        Fields = GameObject.FindGameObjectsWithTag("Field");

        if (bPlayer)
        {
            for (int i = 1; i < length; i++)
            {
                Debug.Log("Name piece = " + table.GetRecord("Piece", i) + " - House = " + table.GetRecord("House", i));

                GameObject piece = transform.Find(table.GetRecord("Piece", i).ToString()).gameObject;
                int house = int.Parse(table.GetRecord("House", i));

                Fields[house].GetComponent<FieldControllerMultiplayer>().Busy = true;
                Fields[house].GetComponent<FieldControllerMultiplayer>().BusyPiece = piece;
                Fields[house].GetComponent<FieldControllerMultiplayer>().SetTextForce(piece.GetComponent<PlayerMultiplayer>().GetForceType());
                piece.GetComponent<PlayerMultiplayer>().iFieldLive = Fields[house].GetComponent<FieldControllerMultiplayer>().Index;
                piece.GetComponent<PlayerMultiplayer>().SetTargetField(Fields[house].transform);
                piece.transform.Rotate(0, 0, 0, Space.Self);
            }
        }
        else
        {            

            for (int i = 0; i < length; i++)
            {

                Debug.Log("LoadPieces aOrderEnemies piece = " + aOrderEnemies[i, 0]);
                GameObject piece = transform.Find(aOrderEnemies[i, 0].ToString()).gameObject;

                Debug.Log("LoadPieces aOrderEnemies field = " + aOrderEnemies[i, 1]);
                int house = int.Parse(aOrderEnemies[i, 1].ToString());

                Fields[house].GetComponent<FieldControllerMultiplayer>().Busy = true;
                Fields[house].GetComponent<FieldControllerMultiplayer>().BusyPiece = piece;
                Fields[house].GetComponent<FieldControllerMultiplayer>().SetTextForce(piece.GetComponent<PlayerMultiplayer>().GetForceType());
                piece.GetComponent<PlayerMultiplayer>().iFieldLive = Fields[house].GetComponent<FieldControllerMultiplayer>().Index;
                piece.GetComponent<PlayerMultiplayer>().SetTargetField(Fields[house].transform);
                piece.transform.Rotate(0, 180, 0, Space.Self);
                
            }
        }

        //StartCoroutine(Order(0.5f));
    }

    private string[,] OrderEnemy()    
    {
        GameObject[] pieces = GameObject.FindGameObjectsWithTag("Enemy");
        Fields = GameObject.FindGameObjectsWithTag("Field");

        string[,] orderpieaces = new string[pieces.Length, 2];

        int PiecesCount = pieces.Length;
        int FieldsCount = Fields.Length-1;

        List<string> lpieces = new List<string>();

        foreach (GameObject piece in pieces)
        {
            lpieces.Add(piece.name);
            Debug.Log("List lpieces = " + piece);
        }

        Debug.Log("PiecesCount = " + PiecesCount);
        Debug.Log("FieldsCount = " + FieldsCount);

        for (int i = 0; i < PiecesCount; i++)
        {
            int index = Random.Range(0, lpieces.Count);
            orderpieaces[i, 0] = lpieces[index];
            orderpieaces[i, 1] = Fields[FieldsCount].GetComponent<FieldControllerMultiplayer>().Index.ToString();
            lpieces.Remove(lpieces[index]);
            FieldsCount--;
            Debug.Log("OrderEnemy piece = " + orderpieaces[i, 0]);
            Debug.Log("OrderEnemy house = " + orderpieaces[i, 1]);
        }
        

        return orderpieaces;
    }

    private IEnumerator Order(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Fields = GameObject.FindGameObjectsWithTag("Field");

        int count = 0;

        if (bPlayer)
        {
            foreach (GameObject Field in Fields)
            {
                Debug.Log(Field.name);

                if (count < (Pieces.Length))
                {
                    Field.GetComponent<FieldControllerMultiplayer>().Busy = true;
                    Field.GetComponent<FieldControllerMultiplayer>().BusyPiece = Pieces[count].gameObject;
                    Field.GetComponent<FieldControllerMultiplayer>().SetTextForce(Pieces[count].GetComponent<PlayerMultiplayer>().Force.ToString());
                    Pieces[count].GetComponent<PlayerMultiplayer>().iFieldLive = Field.GetComponent<FieldControllerMultiplayer>().Index;
                    Pieces[count].GetComponent<PlayerMultiplayer>().SetTargetField(Field.transform);
                    Pieces[count].transform.Rotate(0, 0, 0, Space.Self);
                    count++;
                }
            }
        }
        else
        {
            count = 0;

            GameObject[] fcs = FindObjectOfType<OrdersPartsMultiplayer>().Fields;

            int ifields = fcs.Length-1;

            for (int i = ifields; i > 0; i--)
            {                

                if (count < (Pieces.Length))
                {
                    fcs[i].GetComponent<FieldControllerMultiplayer>().Busy = true;
                    fcs[i].GetComponent<FieldControllerMultiplayer>().BusyPiece = Pieces[count].gameObject;
                    Pieces[count].GetComponent<PlayerMultiplayer>().iFieldLive = fcs[i].GetComponent<FieldControllerMultiplayer>().Index;
                    Pieces[count].GetComponent<PlayerMultiplayer>().SetTargetField(fcs[i].transform);
                    Pieces[count].transform.Rotate(0, 180, 0, Space.Self);                                        
                    count++;
                }
            }
        }


        //Fields[0].GetComponent<FieldController>().Busy = true;
        //Fields[0].GetComponent<FieldController>().BusyPiece = Pieces[0].gameObject;
        //Pieces[0].GetComponent<Player>().iFieldLive = Fields[0].GetComponent<FieldController>().Index;

        print("OrdersParts - WaitAndPrint " + Time.time);            

    }

    private IEnumerator OrderEdit(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Fields = GameObject.FindGameObjectsWithTag("Field");

        int length = table.Count();

        if (length > 1)
        {

            for (int i = 1; i < length; i++)
            {
                Debug.Log("Name piece = " + table.GetRecord("Piece", i) + " - House = " + table.GetRecord("House", i));

                GameObject piece = transform.Find(table.GetRecord("Piece", i).ToString()).gameObject;
                int house = int.Parse(table.GetRecord("House", i));

                Fields[house].GetComponent<HousePicker>().Busy = true;
                Fields[house].GetComponent<HousePicker>().BusyPiece = piece;
                
                piece.GetComponent<ChangePiece>().iHouse = Fields[house].GetComponent<HousePicker>().Index;
                piece.transform.position = Fields[house].transform.position;
                piece.transform.Rotate(0, 0, 0, Space.Self);
                (piece.GetComponent(typeof(BoxCollider)) as Collider).enabled = false;
        }
        }
        else
        {
            int count = 0;

            foreach (GameObject Field in Fields)
            {
                Debug.Log(Field.name);

                if (count < (Pieces.Length))
                {
                    //Field.GetComponent<FieldController>().Busy = true;

                    //Pieces[count].GetComponent<Player>().iFieldLive = Field.GetComponent<FieldController>().Index;
                    //Pieces[count].GetComponent<Player>().SetTargetField(Field.transform);                
                    Field.GetComponent<HousePicker>().BusyPiece = Pieces[count].gameObject;
                    (Pieces[count].GetComponent(typeof(BoxCollider)) as Collider).enabled = false;
                    Pieces[count].transform.position = Field.transform.position;
                    Pieces[count].GetComponent<ChangePiece>().SetHouse(Field.GetComponent<HousePicker>().Index);
                    count++;
                }
            }
        }
        print("OrdersParts - WaitAndPrint " + Time.time);

    }



}
