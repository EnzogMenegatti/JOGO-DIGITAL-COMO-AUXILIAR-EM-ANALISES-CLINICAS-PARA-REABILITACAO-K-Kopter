using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO.Ports;

using System.IO;
public class ArduinoManeger : MonoBehaviour
{
    public static ArduinoManeger instance;

    public static string Ard = "COM10"; //Notebook: COM7; PC Lab: 
    public Text Porta_COM;

    public static float S1;
    public static float S2;
    public static float S3;
    public static float S4;

    public static float S5;
    public static float S6;
    public static float S7;
    public static float S8;

    public static float X;
    public static float Y;
    public static float Z;

    public static float ZS1=0;
    public static float ZS2=0;
    public static float ZS3=0;
    public static float ZS4=0;

    public static float ZS5=0;
    public static float ZS6=0;
    public static float ZS7=0;
    public static float ZS8=0;

    public static bool ArdCon=false;

    public float timer = 0f;

    public bool Raw;
    public bool Ace;
    public bool PF;

    public GameObject ICon;
    public GameObject INCon;


    SerialPort porta = new SerialPort(Ard, 115200);

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        timer += Time.deltaTime;
        if (timer > 0.3f)
        {
            if (Raw == true)
            {
                LerRaw();
            }

            if (Ace == true)
            {
                LerAce();
            }

            timer = 0f;

        }

    }

    public void ConectarArd()
    {
        Ard = Porta_COM.text;
        porta = new SerialPort(Ard, 115200);

        porta.Open();
        porta.ReadTimeout = 1000;

        if (porta.IsOpen)
        {
            ICon.SetActive(true);
            INCon.SetActive(false);
            ArdCon = true;
            porta.WriteLine("c");
        }

        
       
    }

    public void FecharArd()
    {
        porta.Close();
        INCon.SetActive(true);
        ICon.SetActive(false);
        ArdCon = false;
    }

    public void LerRaw()
    {
        if (porta.IsOpen)
        {    

            porta.WriteLine("c");

            string value = porta.ReadLine();

            string[] vec3 = value.Split(null);

            S1 = (float.Parse(vec3[0]));
            if (S1 < 0)
            {
                S1 = 655 + (655 - (S1 * -1));
            }

            S2 = (float.Parse(vec3[1]));
            if (S2 < 0)
            {
                S2 = 655 + (655 - (S2 * -1));
            }

            S3 = (float.Parse(vec3[2]));
            if(S3<0)
            {
                S3 = 655 + (655-(S3*-1));
            }

            S4 = (float.Parse(vec3[3]));
            if (S4<0)
            {
                S4 = 655 + (655 - (S4 * -1));
            }

            S5 = (float.Parse(vec3[4]));
            if (S5 < 0)
            {
                S5 = 655 + (655 - (S5 * -1));
            }

            S6 = (float.Parse(vec3[5]));
            if (S6 < 0)
            {
                S6 = 655 + (655 - (S6 * -1));
            }

            S7 = (float.Parse(vec3[6]));
            if (S7 < 0)
            {
                S7 = 655 + (655 - (S7 * -1));
            }

            S8 = (float.Parse(vec3[7]));
            if (S8 < 0)
            {
                S8 = 655 + (655 - (S8 * -1));
            }

            X = (float.Parse(vec3[8]));
            Y = (float.Parse(vec3[9]));
            Z = (float.Parse(vec3[10]));

            
        }
    }

    public void LerAce()
    {
        if (porta.IsOpen)
        {

            porta.WriteLine("a");

            string value = porta.ReadLine();

            string[] vec3 = value.Split(null);

            X = (float.Parse(vec3[0]));
            Y = (float.Parse(vec3[1]));

        }
    }

    public void Calibrar()
    {
        ZS1 = S1;
        ZS2 = S2;
        ZS3 = S3;
        ZS4 = S4;

        ZS5 = S5;
        ZS6 = S6;
        ZS7 = S7;
        ZS8 = S8;
    }

    
}

