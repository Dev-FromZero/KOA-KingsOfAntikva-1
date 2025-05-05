using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMultiplayer : MonoBehaviour
{
    // Start is called before the first frame update    

    public bool Status = false;

    public TextMesh Txt3dForce;   

    public enum ItemType    
    {
        Soldado,
        Antibomba,
        Sargento,
        Tenente,
        Capitao,
        Major,
        Coronel,
        General,
        Ministro,
        Espia,
        Bandeira,
        Bomba

    }

    public ItemType Types;

    public int Force;
    public int Rule;
    public int iFieldLive;
    public float MoveSpeed;
    public bool Attacked = false;

    bool Attack = false;
    int iFields = 0;
    TurnMultiplayer turn;

    Animator anim;

    int iTargetField;

    [SerializeField]
    GameObject gSoldier;

    [SerializeField]
    GameObject AttackEffect;

    [SerializeField]
    GameObject AttackEffectSoldier;

    [SerializeField]
    Transform AttackEffectPos;

    [SerializeField]
    AudioSource auDie;
    [SerializeField]
    AudioSource auDown;

    [SerializeField]
    GameObject gDie;

    [SerializeField]
    GameObject gParticleChest;

    [SerializeField]
    AudioSource auAttackYell;
    
    AudioSource PreAttack;
    AudioSource Steps;
    
    AudioSource Cancel;
    AudioSource Select;

    AudioSource VictoryPeaple;
    AudioSource VictoryConfirm;
    AudioSource auDieSoldier;
    AudioSource auAttackSoldier;
    AudioSource auDownSoldier;

    bool bDown = false;

    string sDebug="";

    DebugToText debugTotext;

    GameObject gEX_Default;

    GameObject gEX_New;

    int iGameMode=0;

    GameObject gChest;

    bool bDie = false;

    CreateBoard board;

    void Start()
    {        

        PreAttack = GameObject.Find("Suspense").GetComponent<AudioSource>();        
        Select = GameObject.Find("Select").GetComponent<AudioSource>();
        Cancel = GameObject.Find("Cancel").GetComponent<AudioSource>();        
        VictoryPeaple = GameObject.Find("VictoryPeaple").GetComponent<AudioSource>();
        VictoryConfirm = GameObject.Find("VictoryConfirm").GetComponent<AudioSource>();
        auDieSoldier = GameObject.Find("DieSoldier").GetComponent<AudioSource>();
        auAttackSoldier = GameObject.Find("AttackSoldier").GetComponent<AudioSource>();
        auDownSoldier = GameObject.Find("Down1345").GetComponent<AudioSource>();

        if (Types == ItemType.Soldado)
        {
            Steps = GameObject.Find("Run").GetComponent<AudioSource>();
        }
        else
        {
            if (iGameMode > 1)
            {
                if (tag == "Enemy")
                {
                    Steps = GameObject.Find("Run").GetComponent<AudioSource>();
                }
                else
                {
                    Steps = GameObject.Find("Steps").GetComponent<AudioSource>();
                }
            }
            else
            {
                Steps = GameObject.Find("Steps").GetComponent<AudioSource>();
            }
        }


        if (Types == ItemType.Bandeira)
        {
            Txt3dForce.text = "";
        }
        else if (Types == ItemType.Bomba)
        {
            Txt3dForce.text = "";
        }
        else if (Types == ItemType.Espia)
        {
            Txt3dForce.text = "S";
        }
        else
        {
            Txt3dForce.text = Force.ToString();
        }

        board = FindObjectOfType<CreateBoard>();

        iFields = board.GetFields();
        iFields = iFields + 1;               

        debugTotext = FindObjectOfType<DebugToText>();        

        iGameMode = FindObjectOfType<GameMode>().GetGameType();        

        if (iGameMode > 1)
        {

            gEX_Default = GetComponentInChildren<Animator>().gameObject;

            if (gSoldier)
            {
                gSoldier.SetActive(false);
                Vector3 vector3 = new Vector3(gEX_Default.transform.position.x, 0, gEX_Default.transform.position.z);
                gEX_New = Instantiate(gSoldier, vector3, transform.rotation);
                gEX_New.transform.parent = transform;

                gEX_Default.SetActive(false);
                gEX_New.SetActive(true);
                gEX_New.transform.rotation = transform.rotation;

                anim = gEX_New.GetComponent<Animator>();

                if (Types == ItemType.Bandeira)
                {
                    gChest = transform.Find("Bau").gameObject;
                    gChest.SetActive(false);
                }
            }
            else
            {
                anim = gEX_Default.GetComponent<Animator>();
            }
        }
        else
        {
            anim = GetComponentInChildren<Animator>();
        }

        turn = FindObjectOfType<TurnMultiplayer>();
    }

    // Update is called once per frame
    void Update()
    {
        StopAttack();        
        CelebrateVitory();
        DownGround();        
    }

    private void OnMouseDown()
    {
        if(turn.TurnPlayer == "Player")
        {
            SelectPeace();
        }
        else
        {
            SelectPeace();
        }
    }

    public void SelectPeace()
    {
        if (!bWinGame)
        {
            if (turn.TurnPlayer == tag.ToString() && turn.Liberate == true)
            {

                if (!Status)
                {
                    sDebug = "Selected piece: " + name + " - " + "Selected Status: " + Status;

                    Debug.Log(sDebug);
                    debugTotext.ShowDebug(sDebug);

                    CancelMovement();

                    Status = true;

                    turn.SetPiece(gameObject);

                    FieldControllerMultiplayer(GetRule());

                    Select.Play();

                }
                else
                {
                    sDebug = "Selected piece: " + name + " - " + "Selected Status: " + Status;

                    Debug.Log(sDebug);
                    debugTotext.ShowDebug(sDebug);

                    Status = false;

                    turn.SetPiece(null);

                    CancelMovement();

                    Cancel.Play();

                }
            }

            if (Attacked == true)
            {
                SetAttack();
            }
        }
    }

    GameObject gEnemyPieace;

    void SetAttack()
    {       
        
        GameObject gpieace = turn.Piece;

        if (gpieace)
        {
            turn.Liberate = false;
            gpieace.GetComponent<PlayerMultiplayer>().AttackRules(gameObject);
            CancelMovement();
        }
    }    

    public void AttackRules(GameObject pieace)
    {
        iTargetField = pieace.GetComponent<PlayerMultiplayer>().iFieldLive;
        GameObject[] fcs = FindObjectOfType<OrdersParts>().Fields;

        gEnemyPieace = pieace;
        LookEnemy(pieace.transform);

        if (pieace.GetComponent<PlayerMultiplayer>().Types != ItemType.Bomba)
        {
            if (pieace.GetComponent<PlayerMultiplayer>().Types != ItemType.Bandeira)
            {
                pieace.GetComponent<PlayerMultiplayer>().LookEnemy(transform);
            }
        }

        if (pieace.GetComponent<PlayerMultiplayer>().Types == ItemType.Bandeira)
        {
            if (Types != ItemType.Soldado)
            {
                sDebug = "Attack pieace.GetComponent<PlayerMultiplayer>().Types = " + pieace.GetComponent<PlayerMultiplayer>().Types;
                Debug.Log(sDebug);
                debugTotext.ShowDebug(sDebug);

                PreAttack.Play();

                IEnumerator enumerator = IEattack(pieace, 1.0f);
                StartCoroutine(enumerator);

            }
            //else if (pieace.GetComponent<PlayerMultiplayer>().Types == ItemType.Soldado)
            else if (Types == ItemType.Soldado)
            {
                sDebug = "Move to " + pieace.GetComponent<PlayerMultiplayer>().iFieldLive + " - " + "Name of house " + fcs[iTargetField].GetComponent<FieldControllerMultiplayer>().gameObject.name;

                Debug.Log(sDebug);
                debugTotext.ShowDebug(sDebug);

                TargetField = fcs[iTargetField].GetComponent<FieldControllerMultiplayer>().gameObject.transform;

                Steps.Play();

                IEnumerator enumerator = MovetoAttack(pieace, true);
                StartCoroutine(enumerator);

            }
        }
        else if (Types == ItemType.Antibomba && pieace.GetComponent<PlayerMultiplayer>().Types == ItemType.Bomba)
        {

            if (pieace.GetComponent<PlayerMultiplayer>().Types != ItemType.Soldado)
            {

                sDebug = "Pieace " + pieace.name + " is dead";
                Debug.Log(sDebug);
                debugTotext.ShowDebug(sDebug);

                PreAttack.Play();

                IEnumerator enumerator = IEattack(pieace, 1.0f);
                StartCoroutine(enumerator);
            }
            else if (pieace.GetComponent<PlayerMultiplayer>().Types == ItemType.Soldado)
            {
                sDebug = "Move to " + pieace.GetComponent<PlayerMultiplayer>().iFieldLive + " - " + "Name of house " + fcs[iTargetField].GetComponent<FieldControllerMultiplayer>().gameObject.name;

                Debug.Log(sDebug);
                debugTotext.ShowDebug(sDebug);

                TargetField = fcs[iTargetField].GetComponent<FieldControllerMultiplayer>().gameObject.transform;

                Steps.Play();

                IEnumerator enumerator = MovetoAttack(pieace, true);
                StartCoroutine(enumerator);
                
            }

        }
        else if (Types != ItemType.Antibomba && pieace.GetComponent<PlayerMultiplayer>().Types == ItemType.Bomba)
        {
            if (Types != ItemType.Soldado)
            {
                PreAttack.Play();
                pieace.GetComponent<PlayerMultiplayer>().CounterAttack(gameObject, 1.0f);
            }
            else if (Types == ItemType.Soldado)
            {
                sDebug = "Move to " + pieace.GetComponent<PlayerMultiplayer>().iFieldLive + " - " + "Name of house " + fcs[iTargetField].GetComponent<FieldControllerMultiplayer>().gameObject.name;

                Debug.Log(sDebug);
                debugTotext.ShowDebug(sDebug);

                TargetField = fcs[iTargetField].GetComponent<FieldControllerMultiplayer>().gameObject.transform;

                Steps.Play();

                IEnumerator enumerator = MovetoAttack(pieace, false);
                StartCoroutine(enumerator);
            }


        }
        else if (Types == ItemType.Espia && pieace.GetComponent<PlayerMultiplayer>().Force == 9)
        {

            PreAttack.Play();

            IEnumerator enumerator = IEattack(pieace, 1.0f);
            StartCoroutine(enumerator);


        } 
        else if (Force >= pieace.GetComponent<PlayerMultiplayer>().Force && pieace.GetComponent<PlayerMultiplayer>().Types != ItemType.Bomba)
        {
            //if (pieace.GetComponent<PlayerMultiplayer>().Types != ItemType.Soldado)
            if (Types != ItemType.Soldado)
            {
                sDebug = "Attack pieace.GetComponent<PlayerMultiplayer>().Types = " + pieace.GetComponent<PlayerMultiplayer>().Types;
                Debug.Log(sDebug);
                debugTotext.ShowDebug(sDebug);

                PreAttack.Play();

                IEnumerator enumerator = IEattack(pieace, 1.0f);
                StartCoroutine(enumerator);
               
            }
            //else if (pieace.GetComponent<PlayerMultiplayer>().Types == ItemType.Soldado)
            else if (Types == ItemType.Soldado)
            {
                sDebug = "Move to " + pieace.GetComponent<PlayerMultiplayer>().iFieldLive + " - " + "Name of house " + fcs[iTargetField].GetComponent<FieldControllerMultiplayer>().gameObject.name;

                Debug.Log(sDebug);
                debugTotext.ShowDebug(sDebug);

                TargetField = fcs[iTargetField].GetComponent<FieldControllerMultiplayer>().gameObject.transform;

                Steps.Play();

                IEnumerator enumerator = MovetoAttack(pieace,true);
                StartCoroutine(enumerator);
                
            }
        }
        else if (Force < pieace.GetComponent<PlayerMultiplayer>().Force)
        {            
            if (Types != ItemType.Soldado)
            {
                PreAttack.Play();
                pieace.GetComponent<PlayerMultiplayer>().CounterAttack(gameObject, 1.0f);
            }
            else if (Types == ItemType.Soldado)
            {
                sDebug = "Move to " + pieace.GetComponent<PlayerMultiplayer>().iFieldLive + " - " + "Name of house " + fcs[iTargetField].GetComponent<FieldControllerMultiplayer>().gameObject.name;

                Debug.Log(sDebug);
                debugTotext.ShowDebug(sDebug);

                TargetField = fcs[iTargetField].GetComponent<FieldControllerMultiplayer>().gameObject.transform;

                Steps.Play();

                IEnumerator enumerator = MovetoAttack(pieace,false);
                StartCoroutine(enumerator);
            }

         }
    }

    public int GetRule()
    {
        int irule = 0;

        if (Types == ItemType.Bandeira || Types == ItemType.Bomba)
        {
            irule = 0;
        }
        else if (Types == ItemType.Soldado)
        {
            irule = 2;
        }
        else
        {
            irule = 1;
        }

        return irule;

    }

    public void ChangeModel()
    {
        if (tag == "Enemy")
        {
            if (iGameMode == 2)
            {
                if (Types != ItemType.Soldado)
                {

                    if (Types == ItemType.Bandeira)
                    {
                        gChest.SetActive(true);
                    }

                    gEX_Default.SetActive(true);
                    gEX_New.SetActive(false);
                    anim = gEX_Default.GetComponent<Animator>();
                }                
            }
            else if (iGameMode == 3)
            {
                if (Types == ItemType.Bandeira || Types == ItemType.Bomba)
                {
                    if (Types == ItemType.Bandeira)
                    {
                        gChest.SetActive(true);
                    }

                    gEX_Default.SetActive(true);
                    gEX_New.SetActive(false);

                    anim = gEX_Default.GetComponent<Animator>();
                }
            }
        }


    }

    public string GetForceType()
    {
        string sforcetype = Force.ToString();

        if (Types == ItemType.Bandeira)
        {
            sforcetype = "F";
        }
        else if (Types == ItemType.Bomba)
        {
            sforcetype = "B";
        }
        else if (Types == ItemType.Espia)
        {
            sforcetype = "S";
        }

        return sforcetype;
    }

    private int IndexHouse(int index)
    {
        int indexfield = 0;
        FieldControllerMultiplayer[] fcs = FindObjectsOfType<FieldControllerMultiplayer>();

        int indexloop = 0;

        foreach (FieldControllerMultiplayer fc in fcs)
        {
            if (fc.Index == index)
            {
                indexfield = indexloop;
            }

            indexloop++;
        }

        return indexfield;
    }

    public int[] HousesFree()
    {

        List<int> lhouses = new List<int>();

        FieldControllerMultiplayer[] fcs = FindObjectsOfType<FieldControllerMultiplayer>();

        int indexfield = 0;

        indexfield = IndexHouse(iFieldLive);

        int iColumnCount = FindObjectOfType<CreateBoard>().GetFields()+1;

        Debug.Log("HousesFree - indexfield = " + indexfield);
        Debug.Log("HousesFree - fcs.Length = " + fcs.Length);
        //Debug.Log("HousesFree - fcs[indexfield].Column = " + fcs[indexfield].Column.ToString() + " row = " + (fcs[indexfield].Row).ToString());

        int lenghtfields = fcs.Length;

        int indexfieldleft = (indexfield-1);
        int indexfieldright = (indexfield + 1);

        int indexfieldtop = (indexfield + iColumnCount);
        int indexfieldbottom = (indexfield - iColumnCount);

        Debug.Log("HousesFree - indexfieldleft = " + indexfieldleft);
        Debug.Log("HousesFree - indexfieldright = " + indexfieldright);

        Debug.Log("HousesFree - indexfieldtop = " + indexfieldtop);
        Debug.Log("HousesFree - indexfieldbottom = " + indexfieldbottom);

        if (indexfieldright < lenghtfields)
        {
            if (fcs[indexfieldright].Row == fcs[indexfield].Row)
            {
                if (fcs[indexfieldright].Busy)
                {
                    if (fcs[indexfieldright].BusyPiece.gameObject.tag == "Player")
                    {
                        lhouses.Add(fcs[indexfieldright].Index);
                    }
                }
                else
                {
                    lhouses.Add(fcs[indexfieldright].Index);
                }
            }
        }

        if (indexfieldleft >= 0)
        {
            if (fcs[indexfieldleft].Row == fcs[indexfield].Row)
            {
                if (fcs[indexfieldleft].Busy)
                {
                    if (fcs[indexfieldleft].BusyPiece.gameObject.tag == "Player")
                    {
                        lhouses.Add(fcs[indexfieldleft].Index);
                    }
                }
                else
                {
                    lhouses.Add(fcs[indexfieldleft].Index);
                }
            }
        }

        if (indexfieldtop < lenghtfields)
        {
            if (fcs[indexfieldtop].Column == fcs[indexfield].Column)
            {
                if (fcs[indexfieldtop].Busy)
                {
                    if (fcs[indexfieldtop].BusyPiece.gameObject.tag == "Player")
                    {
                        lhouses.Add(fcs[indexfieldtop].Index);
                    }
                }
                else
                {
                    lhouses.Add(fcs[indexfieldtop].Index);
                }
            }
        }

        if (indexfieldbottom >= 0)
        {
            if (fcs[indexfieldbottom].Column == fcs[indexfield].Column)
            {
                if (fcs[indexfieldbottom].Busy)
                {
                    if (fcs[indexfieldbottom].BusyPiece.gameObject.tag == "Player")
                    {
                        lhouses.Add(fcs[indexfieldbottom].Index);
                    }
                }
                else
                {
                    lhouses.Add(fcs[indexfieldbottom].Index);
                }
            }
        } 

        int[] ihouses = lhouses.ToArray();

        return ihouses;
    }

    public int[] CheckHouses()
    {

        List<int> lhouses = new List<int>();

        FieldControllerMultiplayer[] fcs = FindObjectsOfType<FieldControllerMultiplayer>();

        int indexfield = 0;

        indexfield = IndexHouse(iFieldLive);

        int iColumnCount = FindObjectOfType<CreateBoard>().GetFields() + 1;

        Debug.Log("HousesFree - indexfield = " + indexfield);
        Debug.Log("HousesFree - fcs.Length = " + fcs.Length);
        //Debug.Log("HousesFree - fcs[indexfield].Column = " + fcs[indexfield].Column.ToString() + " row = " + (fcs[indexfield].Row).ToString());

        int lenghtfields = fcs.Length;

        int indexfieldleft = (indexfield - 1);
        int indexfieldright = (indexfield + 1);

        int indexfieldtop = (indexfield + iColumnCount);
        int indexfieldbottom = (indexfield - iColumnCount);

        Debug.Log("HousesFree - indexfieldleft = " + indexfieldleft);
        Debug.Log("HousesFree - indexfieldright = " + indexfieldright);

        Debug.Log("HousesFree - indexfieldtop = " + indexfieldtop);
        Debug.Log("HousesFree - indexfieldbottom = " + indexfieldbottom);

        if (indexfieldright < lenghtfields)
        {
            if (fcs[indexfieldright].Row == fcs[indexfield].Row)
            {
                if (fcs[indexfieldright].Busy)
                {
                    if (fcs[indexfieldright].BusyPiece.gameObject.tag == "Enemy")
                    {
                        lhouses.Add(fcs[indexfieldright].Index);
                    }
                }
                else
                {
                    lhouses.Add(fcs[indexfieldright].Index);
                }
            }
        }

        if (indexfieldleft >= 0)
        {
            if (fcs[indexfieldleft].Row == fcs[indexfield].Row)
            {
                if (fcs[indexfieldleft].Busy)
                {
                    if (fcs[indexfieldleft].BusyPiece.gameObject.tag == "Enemy")
                    {
                        lhouses.Add(fcs[indexfieldleft].Index);
                    }
                }
                else
                {
                    lhouses.Add(fcs[indexfieldleft].Index);
                }
            }
        }

        if (indexfieldtop < lenghtfields)
        {
            if (fcs[indexfieldtop].Column == fcs[indexfield].Column)
            {
                if (fcs[indexfieldtop].Busy)
                {
                    if (fcs[indexfieldtop].BusyPiece.gameObject.tag == "Enemy")
                    {
                        lhouses.Add(fcs[indexfieldtop].Index);
                    }
                }
                else
                {
                    lhouses.Add(fcs[indexfieldtop].Index);
                }
            }
        }

        if (indexfieldbottom >= 0)
        {
            if (fcs[indexfieldbottom].Column == fcs[indexfield].Column)
            {
                if (fcs[indexfieldbottom].Busy)
                {
                    if (fcs[indexfieldbottom].BusyPiece.gameObject.tag == "Enemy")
                    {
                        lhouses.Add(fcs[indexfieldbottom].Index);
                    }
                }
                else
                {
                    lhouses.Add(fcs[indexfieldbottom].Index);
                }
            }
        }

        int[] ihouses = lhouses.ToArray();

        return ihouses;
    }

    void FieldControllerMultiplayer(int rule)
    {
        TurnMultiplayer turn = FindObjectOfType<TurnMultiplayer>();

        GameObject[] fcs = FindObjectOfType<OrdersPartsMultiplayer>().Fields;

        int TotalFields = fcs.Length;

        int indexField = iFieldLive;

        if (rule == 1)
        {            

            
            if ((indexField + 1) < TotalFields)
            {
                if (SameRow(indexField, indexField + 1))
                {

                    if (!fcs[indexField + 1].GetComponent<FieldControllerMultiplayer>().Busy)
                    {
                        fcs[indexField + 1].GetComponent<FieldControllerMultiplayer>().SetStatus(true);
                    }
                    else
                    {
                        GameObject gpieace = fcs[indexField + 1].GetComponent<FieldControllerMultiplayer>().BusyPiece.gameObject;

                        sDebug = "Field Index = " + fcs[indexField + 1].GetComponent<FieldControllerMultiplayer>().Index + " - " + "Field Busy = " + fcs[indexField + 1].GetComponent<FieldControllerMultiplayer>().Busy;

                        Debug.Log(sDebug);
                        debugTotext.ShowDebug(sDebug);

                        if (tag == "Player" && gpieace.GetComponent<PlayerMultiplayer>().tag == "Enemy")
                        {
                            fcs[indexField + 1].GetComponent<FieldControllerMultiplayer>().SetStatus(true);
                            fcs[indexField + 1].GetComponent<FieldControllerMultiplayer>().AttackMode = true;
                            Attack = true;
                            gpieace.GetComponent<PlayerMultiplayer>().Attacked = true;
                        }
                        if (tag == "Enemy" && gpieace.GetComponent<PlayerMultiplayer>().tag == "Player")
                        {
                            fcs[indexField + 1].GetComponent<FieldControllerMultiplayer>().SetStatus(true);
                            fcs[indexField + 1].GetComponent<FieldControllerMultiplayer>().AttackMode = true;
                            Attack = true;
                            gpieace.GetComponent<PlayerMultiplayer>().Attacked = true;
                        }
                    }
                }
            }

            //if (SameRow(indexField, indexField + 9))
            if ((indexField + iFields) < TotalFields)
            {
                if (!fcs[indexField + iFields].GetComponent<FieldControllerMultiplayer>().Busy)
                {
                    fcs[indexField + iFields].GetComponent<FieldControllerMultiplayer>().SetStatus(true);
                }
                else
                {
                    GameObject gpieace = fcs[indexField + iFields].GetComponent<FieldControllerMultiplayer>().BusyPiece.gameObject;
                    if (tag == "Player" && gpieace.GetComponent<PlayerMultiplayer>().tag == "Enemy")
                    {
                        fcs[indexField + iFields].GetComponent<FieldControllerMultiplayer>().SetStatus(true);
                        Attack = true;
                        gpieace.GetComponent<PlayerMultiplayer>().Attacked = true;
                    }
                    if (tag == "Enemy" && gpieace.GetComponent<PlayerMultiplayer>().tag == "Player")
                    {
                        fcs[indexField + iFields].GetComponent<FieldControllerMultiplayer>().SetStatus(true);
                        Attack = true;
                        gpieace.GetComponent<PlayerMultiplayer>().Attacked = true;
                    }
                }

            }

            
            if ((indexField - 1) >= 0)
            {
                if (SameRow(indexField, indexField - 1))
                {
                    if (!fcs[indexField - 1].GetComponent<FieldControllerMultiplayer>().Busy)
                    {
                        fcs[indexField - 1].GetComponent<FieldControllerMultiplayer>().SetStatus(true);
                    }
                    else
                    {
                        GameObject gpieace = fcs[indexField - 1].GetComponent<FieldControllerMultiplayer>().BusyPiece.gameObject;
                        if (tag == "Player" && gpieace.GetComponent<PlayerMultiplayer>().tag == "Enemy")
                        {
                            fcs[indexField - 1].GetComponent<FieldControllerMultiplayer>().SetStatus(true);
                            fcs[indexField - 1].GetComponent<FieldControllerMultiplayer>().AttackMode = true;
                            Attack = true;
                            gpieace.GetComponent<PlayerMultiplayer>().Attacked = true;
                        }
                        if (tag == "Enemy" && gpieace.GetComponent<PlayerMultiplayer>().tag == "Player")
                        {
                            fcs[indexField - 1].GetComponent<FieldControllerMultiplayer>().SetStatus(true);
                            fcs[indexField - 1].GetComponent<FieldControllerMultiplayer>().AttackMode = true;
                            Attack = true;
                            gpieace.GetComponent<PlayerMultiplayer>().Attacked = true;
                        }
                    }
                }
            }

            //if (SameRow(indexField, indexField - 9))
            if ((indexField - iFields) >= 0)
            {
                if (!fcs[indexField - iFields].GetComponent<FieldControllerMultiplayer>().Busy)
                {
                    fcs[indexField - iFields].GetComponent<FieldControllerMultiplayer>().SetStatus(true);
                }
                else
                {
                    GameObject gpieace = fcs[indexField - iFields].GetComponent<FieldControllerMultiplayer>().BusyPiece.gameObject;
                    if (tag == "Player" && gpieace.GetComponent<PlayerMultiplayer>().tag == "Enemy")
                    {
                        fcs[indexField - iFields].GetComponent<FieldControllerMultiplayer>().SetStatus(true);
                        fcs[indexField - iFields].GetComponent<FieldControllerMultiplayer>().AttackMode = true;
                        Attack = true;
                        gpieace.GetComponent<PlayerMultiplayer>().Attacked = true;
                    }
                    if (tag == "Enemy" && gpieace.GetComponent<PlayerMultiplayer>().tag == "Player")
                    {
                        fcs[indexField - iFields].GetComponent<FieldControllerMultiplayer>().SetStatus(true);
                        fcs[indexField - iFields].GetComponent<FieldControllerMultiplayer>().AttackMode = true;
                        Attack = true;
                        gpieace.GetComponent<PlayerMultiplayer>().Attacked = true;
                    }
                }
            }
        }

        if (rule==2)
        {
            

            string FieldColumm = fcs[indexField].GetComponent<FieldControllerMultiplayer>().ColumnName.ToString();
            //Debug.Log("FieldColumm = " + FieldColumm);

            int FieldRow = fcs[indexField].GetComponent<FieldControllerMultiplayer>().Row;
            //Debug.Log("FieldRow = " + FieldRow);

            bool bColumnLoop = true;
            bool bRowLoop = true;

            int iTotalFields = fcs.Length;

            int iLimitFieldCol = 0;
            int iLimitFieldRow = 0;

            int CountRowFields = 0;

            //Preenche as casas a frente da peça
            for (int i = indexField; i < iTotalFields; i++)
            {
                if (fcs[i].GetComponent<FieldControllerMultiplayer>().ColumnName.ToString() == FieldColumm)
                {
                    if (i > indexField)
                    {
                        if (fcs[i].GetComponent<FieldControllerMultiplayer>().Busy)
                        {
                            if (iLimitFieldCol == 0)
                            {
                                iLimitFieldCol = fcs[i].GetComponent<FieldControllerMultiplayer>().Index;
                            }
                        }
                    }
                }

                CountRowFields++;

            }

            if (iLimitFieldCol == 0)
            {
                iLimitFieldCol = (indexField + CountRowFields);
            }

            //Debug.Log("indexField = " + indexField);
            //Debug.Log("iLimitFieldRow Forward = " + iLimitFieldCol);

            for (int i = indexField; i < iLimitFieldCol; i++)
            {
                if (fcs[i].GetComponent<FieldControllerMultiplayer>().ColumnName.ToString() == FieldColumm)
                {
                    //if (i > indexField)
                    //{
                    if (!fcs[i].GetComponent<FieldControllerMultiplayer>().Busy)
                    {
                        fcs[i].GetComponent<FieldControllerMultiplayer>().SetStatus(true);
                    }
                    //}
                }
            }


            if ((iLimitFieldCol) < TotalFields)
            {
                if (fcs[iLimitFieldCol].GetComponent<FieldControllerMultiplayer>().ColumnName.ToString() == FieldColumm)
                {
                    if (fcs[iLimitFieldCol].GetComponent<FieldControllerMultiplayer>().Busy)
                    {
                        GameObject gpieace = fcs[iLimitFieldCol].GetComponent<FieldControllerMultiplayer>().BusyPiece.gameObject;
                        if (tag == "Player" && gpieace.GetComponent<PlayerMultiplayer>().tag == "Enemy")
                        {
                            fcs[iLimitFieldCol].GetComponent<FieldControllerMultiplayer>().SetStatus(true);
                            fcs[iLimitFieldCol].GetComponent<FieldControllerMultiplayer>().AttackMode = true;
                            Attack = true;
                            gpieace.GetComponent<PlayerMultiplayer>().Attacked = true;
                        }
                        if (tag == "Enemy" && gpieace.GetComponent<PlayerMultiplayer>().tag == "Player")
                        {
                            fcs[iLimitFieldCol].GetComponent<FieldControllerMultiplayer>().SetStatus(true);
                            fcs[iLimitFieldCol].GetComponent<FieldControllerMultiplayer>().AttackMode = true;
                            Attack = true;
                            gpieace.GetComponent<PlayerMultiplayer>().Attacked = true;
                        }
                    }
                }
            }


            //Preenche as casas a atrás da peça
            //Debug.Log("Preenche as casas a atrás da peça");

            iLimitFieldCol = 0;//iTotalFields;

            for (int i = indexField; i >= 0; i--)
            {
                
                if (fcs[i].GetComponent<FieldControllerMultiplayer>().ColumnName.ToString() == FieldColumm)
                {
                    //Debug.Log("FieldColumm - i = " + i);

                    if (i < indexField)
                    {
                        if (fcs[i].GetComponent<FieldControllerMultiplayer>().Busy)
                        {
                            if (iLimitFieldCol == 0)
                            {
                                iLimitFieldCol = fcs[i].GetComponent<FieldControllerMultiplayer>().Index;                                
                            }
                        }
                    }
                }

                CountRowFields++;

            }

           //Debug.Log("Back - iLimitFieldCol = " + iLimitFieldCol);
           //Debug.Log("Back - indexField = " + indexField);

            for (int i = indexField; i >= iLimitFieldCol; i--)
            {
                if (fcs[i].GetComponent<FieldControllerMultiplayer>().ColumnName.ToString() == FieldColumm)
                {
                    //if (i > indexField)
                    //{
                    if (!fcs[i].GetComponent<FieldControllerMultiplayer>().Busy)
                    {
                        fcs[i].GetComponent<FieldControllerMultiplayer>().SetStatus(true);
                    }
                    //}
                }
            }

            Debug.Log("iFields = " + iFields);

            if ((iLimitFieldCol) >= 0)
            {
                if (fcs[iLimitFieldCol].GetComponent<FieldControllerMultiplayer>().Busy)
                {
                    GameObject gpieace = fcs[iLimitFieldCol].GetComponent<FieldControllerMultiplayer>().BusyPiece.gameObject;
                    if (tag == "Player" && gpieace.GetComponent<PlayerMultiplayer>().tag == "Enemy")
                    {
                        fcs[iLimitFieldCol].GetComponent<FieldControllerMultiplayer>().SetStatus(true);
                        fcs[iLimitFieldCol].GetComponent<FieldControllerMultiplayer>().AttackMode = true;
                        Attack = true;
                        gpieace.GetComponent<PlayerMultiplayer>().Attacked = true;
                    }
                    if (tag == "Enemy" && gpieace.GetComponent<PlayerMultiplayer>().tag == "Player")
                    {
                        fcs[iLimitFieldCol].GetComponent<FieldControllerMultiplayer>().SetStatus(true);
                        fcs[iLimitFieldCol].GetComponent<FieldControllerMultiplayer>().AttackMode = true;
                        Attack = true;
                        gpieace.GetComponent<PlayerMultiplayer>().Attacked = true;
                    }
                }
            }


            //Preenche as casas para a direita da peça

            iLimitFieldRow = 0;            

            for (int i = indexField; i < iTotalFields; i++)
            {
                if (fcs[i].GetComponent<FieldControllerMultiplayer>().Row == FieldRow)
                {
                    if (i > indexField)
                    {
                        if (fcs[i].GetComponent<FieldControllerMultiplayer>().Busy)
                        {
                            if (iLimitFieldRow == 0)
                            {
                                iLimitFieldRow = fcs[i].GetComponent<FieldControllerMultiplayer>().Index;
                            }

                        }
                    }

                    CountRowFields++;
                }
            }

            if (iLimitFieldRow == 0)
            {
                iLimitFieldRow = iTotalFields; //CountRowFields;// (indexField + CountRowFields);
            }

            //Debug.Log("indexField = " + indexField);
            //Debug.Log("iLimitFieldRow Right = " + iLimitFieldRow);

            for (int i = 0; i < iTotalFields; i++)
            {                
                if (fcs[i].GetComponent<FieldControllerMultiplayer>().Row == FieldRow)
                {
                    if (i > indexField)
                    {
                        if (fcs[i].GetComponent<FieldControllerMultiplayer>().Index < iLimitFieldRow)
                        {
                            fcs[i].GetComponent<FieldControllerMultiplayer>().SetStatus(bRowLoop);
                        }
                    }
                }
            }

            if (iLimitFieldRow < iTotalFields)
            {
                if (fcs[iLimitFieldRow].GetComponent<FieldControllerMultiplayer>().Row == FieldRow)
                {
                    if (fcs[iLimitFieldRow].GetComponent<FieldControllerMultiplayer>().Busy)
                    {
                        GameObject gpieace = fcs[iLimitFieldRow].GetComponent<FieldControllerMultiplayer>().BusyPiece.gameObject;
                        if (tag == "Player" && gpieace.GetComponent<PlayerMultiplayer>().tag == "Enemy")
                        {
                            fcs[iLimitFieldRow].GetComponent<FieldControllerMultiplayer>().SetStatus(true);
                            fcs[iLimitFieldRow].GetComponent<FieldControllerMultiplayer>().AttackMode = true;
                            Attack = true;
                            gpieace.GetComponent<PlayerMultiplayer>().Attacked = true;
                        }
                        if (tag == "Enemy" && gpieace.GetComponent<PlayerMultiplayer>().tag == "Player")
                        {
                            fcs[iLimitFieldRow].GetComponent<FieldControllerMultiplayer>().SetStatus(true);
                            fcs[iLimitFieldRow].GetComponent<FieldControllerMultiplayer>().AttackMode = true;
                            Attack = true;
                            gpieace.GetComponent<PlayerMultiplayer>().Attacked = true;
                        }
                    }
                }
            }


            iLimitFieldRow = 0;

            CountRowFields = 0;

            //Debug.Log("indexField = " + indexField);
            //Debug.Log("iLimitFieldRow Left = " + iLimitFieldRow);

            //--------------------------------------------------------------------------------------------------------------------------------
            //Preenche as casas para a esquerda da peça

            for (int i = indexField; i >= 0; i--)
            {

                if (fcs[i].GetComponent<FieldControllerMultiplayer>().Row == FieldRow)
                {
                    //Debug.Log("i = " + i);
                    //Debug.Log("indexField = " + indexField);
                    if (i < indexField)
                    {
                        if (fcs[i].GetComponent<FieldControllerMultiplayer>().Busy)
                        {
                            if (iLimitFieldRow == 0)
                            {
                                iLimitFieldRow = fcs[i].GetComponent<FieldControllerMultiplayer>().Index;
                            }

                        }

                        CountRowFields++;
                    }
                }

            }

            //Debug.Log("iLimitFieldRow before Left = " + iLimitFieldRow);

            if (iLimitFieldRow == 0)
            {
                iLimitFieldRow = (CountRowFields);
            }

            //Debug.Log("indexField = " + indexField);
            //Debug.Log("iLimitFieldRow Left = " + iLimitFieldRow);            


            for (int i = indexField; i >= 0; i--)
            {
                if (fcs[i].GetComponent<FieldControllerMultiplayer>().Row == FieldRow)
                {
                    if (i != indexField)
                    {
                        if (fcs[i].GetComponent<FieldControllerMultiplayer>().Index > iLimitFieldRow)
                        {
                            fcs[i].GetComponent<FieldControllerMultiplayer>().SetStatus(bRowLoop);
                        }
                    }
                }
            }

            if (indexField == 1)
            {
                if (!fcs[0].GetComponent<FieldControllerMultiplayer>().Busy)
                {
                    fcs[0].GetComponent<FieldControllerMultiplayer>().SetStatus(bRowLoop);
                }
            }

            //if (iLimitFieldRow < indexField)
            if (fcs[iLimitFieldRow].GetComponent<FieldControllerMultiplayer>().Row == FieldRow)
            {
                if (fcs[iLimitFieldRow].GetComponent<FieldControllerMultiplayer>().Busy)
                {
                    GameObject gpieace = fcs[iLimitFieldRow].GetComponent<FieldControllerMultiplayer>().BusyPiece.gameObject;
                    if (tag == "Player" && gpieace.GetComponent<PlayerMultiplayer>().tag == "Enemy")
                    {
                        fcs[iLimitFieldRow].GetComponent<FieldControllerMultiplayer>().SetStatus(true);
                        fcs[iLimitFieldRow].GetComponent<FieldControllerMultiplayer>().AttackMode = true;
                        Attack = true;
                        gpieace.GetComponent<PlayerMultiplayer>().Attacked = true;
                    }
                    if (tag == "Enemy" && gpieace.GetComponent<PlayerMultiplayer>().tag == "Player")
                    {
                        fcs[iLimitFieldRow].GetComponent<FieldControllerMultiplayer>().SetStatus(true);
                        fcs[iLimitFieldRow].GetComponent<FieldControllerMultiplayer>().AttackMode = true;
                        Attack = true;
                        gpieace.GetComponent<PlayerMultiplayer>().Attacked = true;
                    }
                }
            }
        }

    }

    bool SameRow(int indexpiece, int nextpiece)
    {
        bool bsamerow = false;

        GameObject[] fcs = FindObjectOfType<OrdersPartsMultiplayer>().Fields;

        int rowindex = fcs[indexpiece].GetComponent<FieldControllerMultiplayer>().Row;

        int rownext = fcs[nextpiece].GetComponent<FieldControllerMultiplayer>().Row;

        if(rowindex == rownext)
        {
            bsamerow = true;
        }

        return bsamerow;

    }

    public void CancelMovement()
    {

        FieldControllerMultiplayer[] fcs = FindObjectsOfType<FieldControllerMultiplayer>();

        foreach (FieldControllerMultiplayer fd in fcs)
        {
            fd.SetStatus(false);

            if (fd.BusyPiece)
            {
                fd.BusyPiece.GetComponent<PlayerMultiplayer>().Status = false;

                fd.BusyPiece.GetComponent<PlayerMultiplayer>().Attack = false;

                fd.BusyPiece.GetComponent<PlayerMultiplayer>().Attacked = false;
            }
        }

    }

    Transform TargetField;

    public void MoveStart(Transform target)
    {
        if (!bWinGame)
        {
            if (TargetField)
            {
                TargetField.transform.root.GetComponent<FieldControllerMultiplayer>().Busy = false;
                TargetField.transform.root.GetComponent<FieldControllerMultiplayer>().BusyPiece = null;

                sDebug = "Field name = " + TargetField.transform.root.GetComponent<FieldControllerMultiplayer>().name + " - " + "Field BusyPiece = " + TargetField.transform.root.GetComponent<FieldControllerMultiplayer>().BusyPiece;

                Debug.Log(sDebug);
                debugTotext.ShowDebug(sDebug);
            }

            TargetField = target;

            TargetField.transform.root.GetComponent<FieldControllerMultiplayer>().Busy = true;
            TargetField.transform.root.GetComponent<FieldControllerMultiplayer>().BusyPiece = gameObject;

            transform.LookAt(target);

            Steps.Play();

            StartCoroutine(Moveto());
        }
    }

    public void SetTargetField(Transform target)
    {
        TargetField = target;

        transform.position = TargetField.position;

    }

    IEnumerator Moveto()
    {

        TurnMultiplayer turnnow = FindObjectOfType<TurnMultiplayer>();

        float SpeedPlus = 0;

        if (CanRun(TargetField))
        {
            SpeedPlus = 1.0f;
        }

        while (!DistanceTarget(TargetField))
        {
            SetAnimation("Walk", true);
            transform.Translate(Vector3.forward * Time.deltaTime * (MoveSpeed + SpeedPlus));
            yield return null;
        }

        Debug.Log("Moveto - turn.bChangeTurn = " + turnnow.bChangeTurn);
        if (!turnnow.bChangeTurn)
        {
            Debug.Log("Moveto - turn enter");
            turnnow.ChangeTurn();
        }
        Steps.Stop();
        SetAnimation("Walk", false);
        
        yield return 0; //<<<<Here Added
        //Debug.Log("This code never runs");
    }

    private bool CanRun(Transform target)
    {
        bool bRun = false;
        float dist;

        float MaxDist = (board.GetDistance()*2);

        if (target)
        {
            dist = Vector3.Distance(target.position, transform.position);           

            if (dist >= MaxDist)
            {
                print("CanRun - Distance: " + dist);
                bRun = true;
            }
        }

        return bRun;

    }

    IEnumerator MovetoAttack(GameObject pieace,bool attack)
    {
        if (!bWinGame)
        {
            turn.Liberate = false;
            CancelMovement();

            Debug.Log("MovetoAttack pieace = " + pieace.name + " - attack = " + attack);

            float SpeedPlus = 0;

            if(CanRun(TargetField))
            {
                SpeedPlus = 1.0f;
            }

            while (!DistanceAttack(TargetField))
            {
                SetAnimation("Walk", true);
                transform.Translate(Vector3.forward * Time.deltaTime * (MoveSpeed + SpeedPlus));
                yield return null;
            }
            if (attack)
            {
                PreAttack.Play();
                IEnumerator enumerator = IEattack(pieace, 1.0f);
                StartCoroutine(enumerator);
            }
            else
            {
                PreAttack.Play();
                pieace.GetComponent<PlayerMultiplayer>().CounterAttack(gameObject, 1.0f);
            }

            Steps.Stop();
            SetAnimation("Walk", false);
        }
        yield return 0; //<<<<Here Added
        //                //Debug.Log("This code never runs");
    }


    private bool DistanceTarget(Transform target)
    {
        bool bdistance = false;
        float dist;

        if (target)
        {
            dist = Vector3.Distance(target.position, transform.position);
            //print("Distance to other: " + dist);

            if (dist <= 0.1f)            
            {
                bdistance = true;                
            }
        }

        return bdistance;

    }

    private bool DistanceAttack(Transform target)
    {
        bool bdistance = false;
        float dist;

        if (target)
        {
            dist = Vector3.Distance(target.position, transform.position);
            //print("Distance to other: " + dist);

            if (dist <= 1.5f)
            {
                bdistance = true;                
            }
        }

        return bdistance;

    }

    public void OpenChest()
    {
        AnimatorStateInfo currentBaseState = anim.GetCurrentAnimatorStateInfo(0);
        anim.SetBool("Open", true);

        gParticleChest.SetActive(true);

        VictoryConfirm.Play();

        IEnumerator enumerator = IEWin(1.5f);
        StartCoroutine(enumerator);

    }

    private IEnumerator IEWin(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        WinGame();
    }

    void WinGame()
    {

        //if(tag == "Player")
        if (tag == "Enemy")
        {
            GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");

            foreach(GameObject enemy in Enemies)
            {
                if (enemy.GetComponent<PlayerMultiplayer>().Types != ItemType.Bandeira)
                {
                    enemy.GetComponent<PlayerMultiplayer>().SetDie();
                }
                //Destroy(enemy);
            }            

            GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in Players)
            {
                player.GetComponent<PlayerMultiplayer>().SetVictory();
                //Debug.Log("player = " + player.name);
                //Destroy(player);
            }
        }
        else
        {
            GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in Players)
            {
                if (player.GetComponent<PlayerMultiplayer>().Types != ItemType.Bandeira)
                {
                    player.GetComponent<PlayerMultiplayer>().SetDie();
                }
            }            

            GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject enemy in Enemies)
            {
                enemy.GetComponent<PlayerMultiplayer>().SetVictory();
                //Destroy(enemy);
            }
        }
    }

    bool bWinGame = false;

    public void SetVictory()
    {
        //Debug.Log("SetVictory()");
        TurnMultiplayer turn = FindObjectOfType<TurnMultiplayer>();
        bWinGame = true;        
        turn.SetVictory();
    }

    bool bCelebrate = false;

    void CelebrateVitory()
    {

        if (!bCelebrate)
        {
            if (bWinGame)
            {

                if (tag == "Player")
                {
                    //if (GameObject.FindGameObjectsWithTag("Enemy").Length == 1)
                    //{
                    SetAnimation("Win", true);

                    IEnumerator coVictory = VictoryPlay(1.5f);
                    StartCoroutine(coVictory);

                    bCelebrate = true;

                    //}

                }
                if (tag == "Enemy")
                {
                    //if (GameObject.FindGameObjectsWithTag("Player").Length == 1)
                    //{
                    SetAnimation("Win", true);

                    IEnumerator coVictory = VictoryPlay(1.5f);
                    StartCoroutine(coVictory);

                    bCelebrate = true;
                    //}

                }
            }
        }
    }

    private IEnumerator VictoryPlay(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (!VictoryPeaple.isPlaying)
        {
            VictoryPeaple.Play();
        }
    }

    void SetAnimation(string AnimName, bool bstatus)
    {
        anim.SetBool(AnimName, bstatus);
    }

    void StopAttack()
    {
        if (anim)
        {
            if (anim.GetBool("Attack"))
            {
                AnimatorStateInfo currentBaseState = anim.GetCurrentAnimatorStateInfo(0);

                if (currentBaseState.IsName("Attack"))
                {
                    anim.SetBool("Attack", false);
                }

            }
        }
    }

    bool bAttack = false;

    private bool isWalk()
    {
        bool bWalk = false;

        if (anim)
        {
            AnimatorStateInfo currentBaseState = anim.GetCurrentAnimatorStateInfo(0);

            if (currentBaseState.IsName("Walk"))
            {
                bWalk = true;
            }
        }

        return bWalk;
    }

    private IEnumerator IEattack(GameObject pieace, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        turn.Liberate = false;
        pieace.GetComponent<PlayerMultiplayer>().ChangeModel();
        //Ativa animação de ataque
        SetAnimation("Attack", true);
        bAttack = true;

        if (iGameMode == 2 || iGameMode == 3)
        {
            if (tag == "Enemy")
            {
                auAttackSoldier.Play();
            }
            else
            {
                if (auAttackYell)
                {
                    auAttackYell.Play();
                }
            }
        }
        else
        {
            if (auAttackYell)
            {
                auAttackYell.Play();
            }
        }
        IEnumerator coKillEnemy = KillEnemy(pieace, 0.5f);
        StartCoroutine(coKillEnemy);
    }

    private IEnumerator KillEnemy(GameObject pieace,  float waitTime)
    {
        while (bAttack)
        {            
            print("KillEnemy = " + waitTime);            
            yield return new WaitForSeconds(waitTime);
            StartEffectAttack();
            
            if (pieace.GetComponent<PlayerMultiplayer>().Types == ItemType.Bandeira)
            {
                pieace.GetComponent<PlayerMultiplayer>().OpenChest();
            }
            else
            {
                gEnemyPieace.GetComponent<PlayerMultiplayer>().SetDie();
            }
            bAttack = false;                        
        }
    }

    void StartEffectAttack()
    {
        if(AttackEffect)
        {
            if (iGameMode == 2 || iGameMode == 3)
            {
                if (tag == "Enemy")
                {
                    if (AttackEffectSoldier)
                    {
                        Instantiate(AttackEffectSoldier, AttackEffectPos.position, transform.rotation);
                    }
                }
                else
                {
                    if (auAttackYell)
                    {
                        Instantiate(AttackEffect, AttackEffectPos.position, transform.rotation);
                    }
                }
            }
            else
            {
                if (auAttackYell)
                {
                    Instantiate(AttackEffect, AttackEffectPos.position, transform.rotation);
                }
            }           

        }
    }

    bool bTakeHome = false;    

    public void SetTakeHome(int iGoField)
    {
        if (!bWinGame)
        {
            IEnumerator enumerator = TakeHome(iGoField, 0.5f);
            StartCoroutine(enumerator);
        }
    }

    private IEnumerator TakeHome(int Field, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (!bWinGame)
        {
            sDebug = "Take Home";
            Debug.Log(sDebug);
            debugTotext.ShowDebug(sDebug);

            //Move a peça até a casa do inimigo abatido
            //int iTargetField = pieace.GetComponent<PlayerMultiplayer>().iFieldLive;
            int iHomeField = Field;
            GameObject[] fcs = FindObjectOfType<OrdersPartsMultiplayer>().Fields;
            MoveStart(fcs[iHomeField].gameObject.transform);
            IEnumerator enumerator = EndTurnAfterAttack(iHomeField, 1.5f);
            StartCoroutine(enumerator);
        }

    }

    private IEnumerator EndTurnAfterAttack(int Field, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (turn.Piece)
        {
            //cancela os movimentos
            turn.Piece.GetComponent<PlayerMultiplayer>().CancelMovement();

            //Reseta a casa
            turn.Piece.GetComponent<PlayerMultiplayer>().ReleaseHouses();

            //Atualiza a casa atual após a peça andar
            turn.Piece.GetComponent<PlayerMultiplayer>().iFieldLive = Field;
        }
        sDebug = "EndTurnAfterAttack";
        Debug.Log(sDebug);
        debugTotext.ShowDebug(sDebug);
    }

    private IEnumerator EndTurn(int Field, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        //cancela os movimentos
        turn.Piece.GetComponent<PlayerMultiplayer>().CancelMovement();

        //Reseta a casa
        turn.Piece.GetComponent<PlayerMultiplayer>().ReleaseHouses();

        //Atualiza a casa atual após a peça andar
        turn.Piece.GetComponent<PlayerMultiplayer>().iFieldLive = Field;

        //Troca o turno
        Debug.Log("EndTurn - turn.bChangeTurn = " + turn.bChangeTurn);
        if (!turn.bChangeTurn)
        {            
            turn.ChangeTurn();
        }

        sDebug = "Change Turn";
        Debug.Log(sDebug);
        debugTotext.ShowDebug(sDebug);

    }

    public void CounterAttack(GameObject pieace, float waitTime)
    {        
        if(Types== ItemType.Bomba)
        {
            ChangeModel();
        }
        IEnumerator enumerator = IECounterAttack(pieace, 1.0f);
        StartCoroutine(enumerator);
    }

    private IEnumerator IECounterAttack(GameObject pieace, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        turn.Liberate = false;
        //Ativa animação de ataque
        SetAnimation("Attack", true);
        bAttack = true;

        if (iGameMode == 2 || iGameMode == 3)
        {
            if (tag == "Enemy")
            {
                auAttackSoldier.Play();
            }
            else
            {
                if (auAttackYell)
                {
                    auAttackYell.Play();
                }
            }
        }
        else
        {
            if (auAttackYell)
            {
                auAttackYell.Play();
            }
        }
        IEnumerator coKillEnemy = CounterKillEnemy(pieace, 0.5f);
        StartCoroutine(coKillEnemy);
    }

    bool bDieCounter = false;

    private IEnumerator CounterKillEnemy(GameObject pieace, float waitTime)
    {
        while (bAttack)
        {
            gEnemyPieace = pieace;
            print("KillEnemy = " + waitTime);
            yield return new WaitForSeconds(waitTime);
            StartEffectAttack();
            gEnemyPieace.GetComponent<PlayerMultiplayer>().SetDie();
            bAttack = false;
            gEnemyPieace.GetComponent<PlayerMultiplayer>().EndTurnEnemy();

            if(Types == ItemType.Bomba)
            {
                bDieCounter = true;
                SetDie();
            }

            //IEnumerator enumerator = EndTurn(3.0f);
            //StartCoroutine(enumerator);
        }
    }

    public void EndTurnEnemy()
    {
        IEnumerator enumerator = EndTurnCounter(2.5f);
        StartCoroutine(enumerator);
    }
    

    private IEnumerator EndTurnCounter(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        bDieCounter = true;
        //cancela os movimentos
        CancelMovement();
        ReleaseHouses();

    }

    public void LookEnemy(Transform Target)
    {
        transform.LookAt(Target);
    }    

    public void SetDie()
    {
        
        ChangeModel();

        anim.SetBool("Die", true);        
        bDie = true;

        IEnumerator enumerator1 = ShouPain(0.5f);
        StartCoroutine(enumerator1);

        IEnumerator enumerator2 = ManDown(2.5f);
        StartCoroutine(enumerator2);

        IEnumerator enumerator3 = IEDestroyAfterDying(3.5f);
        StartCoroutine(enumerator3);
    }

    public void SetSuicide()
    {

        ChangeModel();

        anim.SetBool("Die", true);
        //bDie = true;

        IEnumerator enumerator1 = ShouPain(0.5f);
        StartCoroutine(enumerator1);

        IEnumerator enumerator2 = ManDown(2.5f);
        StartCoroutine(enumerator2);

        IEnumerator enumerator3 = IEDestroyAfterSuicide(3.5f);
        StartCoroutine(enumerator3);
    }

    private IEnumerator IEDestroyAfterSuicide(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        CancelMovement();
        //ReleaseHouses();
        turn.ChangeTurn();
        Destroy(gameObject);
    
    }

        public bool isDie()
    {
        return bDie;
    }

    private IEnumerator IEDestroyAfterDying(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        sDebug = "DestroyAfterDie - Name = " + name + " - bDieCounter = " + bDieCounter;
        Debug.Log(sDebug);
        debugTotext.ShowDebug(sDebug);

        if (Types != ItemType.Bomba && bDieCounter)
        {
            if (turn.Piece)
            {
                turn.Piece.GetComponent<PlayerMultiplayer>().SetTakeHome(iFieldLive);
            }
            sDebug = "DestroyAfterDie - Name = " + name + " - bDieCounter = " + bDieCounter;
            Debug.Log(sDebug);
            debugTotext.ShowDebug(sDebug);
            CancelMovement();
            ReleaseHouses();
            turn.ChangeTurn();
        }
        if (Types == ItemType.Bomba && !bDieCounter)
        {
            if (turn.Piece)
            {
                sDebug = "DestroyAfterDie - SetTakeHome iFieldLive = " + iFieldLive;
                Debug.Log(sDebug);
                debugTotext.ShowDebug(sDebug);
                turn.Piece.GetComponent<PlayerMultiplayer>().SetTakeHome(iFieldLive);
            }
            sDebug = "DestroyAfterDie - Name = " + name + " - bDieCounter = " + bDieCounter;
            Debug.Log(sDebug);
            debugTotext.ShowDebug(sDebug);
            CancelMovement();
            ReleaseHouses();
            //turn.ChangeTurn();
        }
        else if (Types != ItemType.Bomba && !bDieCounter)
        {
            if (turn.Piece)
            {
                turn.Piece.GetComponent<PlayerMultiplayer>().SetTakeHome(iFieldLive);
            }
            sDebug = "DestroyAfterDie - Name = " + name + " - bDieCounter = " + bDieCounter;
            Debug.Log(sDebug);
            debugTotext.ShowDebug(sDebug);
            CancelMovement();
            ReleaseHouses();
            turn.ChangeTurn();
        }
        if (Types == ItemType.Bomba && bDieCounter)
        {
            //if (turn.Piece)
            //{
            //    sDebug = "DestroyAfterDie - SetTakeHome iFieldLive = " + iFieldLive;
            //    Debug.Log(sDebug);
            //    debugTotext.ShowDebug(sDebug);
            //    turn.Piece.GetComponent<PlayerMultiplayer>().SetTakeHome(iFieldLive);
            //}
            sDebug = "DestroyAfterDie - Types == ItemType.Bomba = " +  ItemType.Bomba + " - bDieCounter = " + bDieCounter;
            Debug.Log(sDebug);
            debugTotext.ShowDebug(sDebug);
            CancelMovement();
            ReleaseHouses();
            //turn.ChangeTurn();
        }


        //turn.ChangeTurn();
        Destroy(gameObject);
        
    }

    private IEnumerator ShouPain(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (iGameMode == 3)
        {
            if (tag == "Enemy")
            {
                auDieSoldier.Play();
            }
            else
            {
                auDie.Play();
            }
        }
        else
        {
            auDie.Play();
        }
    }

    private IEnumerator ManDown(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);


        if (iGameMode == 3)
        {
            if (tag == "Enemy")
            {
                auDownSoldier.Play();
            }
            else
            {
                if (auDown)
                {
                    auDown.Play();
                }
            }
        }
        else
        {
            if (auDown)
            {
                auDown.Play();
            }
        }
        IEnumerator enumerator = DieEffect(0.5f);
        StartCoroutine(enumerator);

    }
    private IEnumerator DieEffect(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        //Instantiate(gDie, auDie.transform.position, auDie.transform.rotation);
        bDown = true;
        Instantiate(gDie, transform.position, gDie.transform.rotation);
    }

    void DownGround()
    {
        if(bDown)
        {
            transform.Translate(Vector3.down * Time.deltaTime);
        }
    }

    public void ReleaseHouses()
    {
        //Debug.Log("ReleaseHouses iTargetField = " + iTargetField);

        FieldControllerMultiplayer[] fcs = FindObjectsOfType<FieldControllerMultiplayer>();        
        foreach (FieldControllerMultiplayer fd in fcs)
        {
            if (fd.Index == iFieldLive)
            {
                fd.BusyPiece = null;
                fd.Busy = false;
            }
        }
    }

}
