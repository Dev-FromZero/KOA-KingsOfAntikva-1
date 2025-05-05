using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBoardMultiplayer : MonoBehaviour
{
 
    //Script para criação dos campos do tabuleiro
    //Variaveis
    [SerializeField] GameObject Field;
    [SerializeField] int iFields;
    [SerializeField] int iRows;
    [SerializeField] float fDistance;

    [SerializeField] int iCount = 0;
    [SerializeField] int iCountFields = 0;
    [SerializeField] int iCountRows = 0;

    [SerializeField]
    bool bSelectOrder = false;

    string[] sAlfabeto;

    bool bEnded=false;

    // Start is called before the first frame update
    void Start()
    {

        Alfabeto();
        //Chamada ao metodo que cria os campos quando inicia a cena
        if (!bSelectOrder)
        {
            CreateFields(iFields, iRows);
        }
        else
        {
            CreateFieldsSelectOrder(iFields, iRows);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Metodo para criar os campos do tabuleiro
    void CreateFields(int col, int row)
    {

        float fPosX = transform.position.x;
        float fPosZ = transform.position.z;

        for (int i = 0; i <= row; i++)
        {            

            for (int x = 0; x <= col; x++)
            {               
                if (iCountFields > row)
                {
                    fPosX = transform.position.x;
                    iCountFields = 0;
                }

                Vector3 vPos = new Vector3(fPosX, Field.transform.position.y, fPosZ);
                GameObject gFieldClone = Instantiate(Field, vPos, transform.rotation);

                int iFieldCount = iCount + 1;

                gFieldClone.name = "Field" + iFieldCount.ToString();

                FieldControllerMultiplayer fc = gFieldClone.GetComponent<FieldControllerMultiplayer>();

                int iColumn = iCountFields + 1;
                int iRow = iCountRows + 1;

                fc.Index = iCount;
                fc.Row = iRow;
                fc.Column = iColumn;
                fc.ColumnName = sAlfabeto[iCountFields];
                fc.NickName = sAlfabeto[iCountFields] + iColumn.ToString();
                iCountFields++;
                iCount++;

                fPosX = fPosX + fDistance;
            }

            iCountRows++;
            fPosZ = fPosZ + fDistance;
        }

        bEnded = true;
    }

    //Metodo para criar os campos do tabuleiro para ordernar peças
    void CreateFieldsSelectOrder(int col, int row)
    {
        float fPosXInit = transform.position.x;
        float fPosX = transform.position.x;
        float fPosZ = transform.position.z;

        for (int i = 0; i <= row; i++)
        {

            for (int x = 0; x <= col; x++)
            {
                if (iCountFields > iFields)
                {
                    fPosX = fPosXInit;
                    //fPosZ = fPosZ * 2;
                    iCountFields = 0;
                }

                Vector3 vPos = new Vector3(fPosX, transform.position.y, fPosZ);
                GameObject gFieldClone = Instantiate(Field, vPos, transform.rotation);

                int iFieldCount = iCount + 1;

                gFieldClone.name = "Field" + iFieldCount.ToString();

                HousePicker hp = gFieldClone.GetComponent<HousePicker>();

                int iColumn = iCountFields + 1;
                int iRow = iCountRows + 1;

                hp.Index = iCount;
                hp.Row = iRow;
                hp.Column = iColumn;
                hp.ColumnName = sAlfabeto[iCountFields];
                hp.NickName = sAlfabeto[iCountFields] + iColumn.ToString();
                iCountFields++;
                iCount++;

                fPosX = fPosX + fDistance;
            }

            iCountRows++;
            fPosZ = fPosZ + fDistance;
        }
    }

    //Cria array com alfabeto para usar no NickName - Ex: A1
    void Alfabeto()
    {        

        sAlfabeto = new string[26];

        sAlfabeto[0] = "A";
        sAlfabeto[1] = "B";
        sAlfabeto[2] = "C";
        sAlfabeto[3] = "D";
        sAlfabeto[4] = "E";
        sAlfabeto[5] = "F";
        sAlfabeto[6] = "G";
        sAlfabeto[7] = "H";
        sAlfabeto[8] = "I";
        sAlfabeto[9] = "J";
        sAlfabeto[10] = "K";
        sAlfabeto[11] = "L";
        sAlfabeto[12] = "M";
        sAlfabeto[13] = "N";
        sAlfabeto[14] = "O";
        sAlfabeto[15] = "P";
        sAlfabeto[16] = "Q";
        sAlfabeto[17] = "R";
        sAlfabeto[18] = "S";
        sAlfabeto[19] = "T";
        sAlfabeto[20] = "U";
        sAlfabeto[21] = "V";
        sAlfabeto[22] = "X";
        sAlfabeto[23] = "Y";
        sAlfabeto[24] = "Z";
    }

    public int GetFields()
    {
        return iFields;
    }

    public bool isFinished()
    {
        return bEnded;
    }

    public float GetDistance()
    {
        return fDistance;
    }
}
