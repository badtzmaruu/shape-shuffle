using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{   
    CameraManager cm;
    WallManager wm;
    SceneManager sceneManager;
    AdManager adm;
    LeaderBoard lb;

    public GameObject[] shps;
    public ParticleSystem[] psConfetti;
    public GameObject centreShp;

    public List<GameObject> currShps = new List<GameObject>();

    public List<int> shpNum;

    [HideInInspector] public string[,] colourPal;

    public int shpCount;
    public int laneNum = 0;
    public int levelNum;
    public int wallNum;
    public int colourCur;


    float speedMax10, speedMin10, speedMin20, speedMax20, speedMin50, speedMax50;
    float distMin10, distMax10, distMin20, distMax20, distMin50, distMax50; 

    public float speedPhone;
    public float speedLerp, distLerp;            //manipulate this
    public float speed, dist;         //output this

    public bool lost, won;
    bool adShown = true;
    public bool allShpCorrect;
    public bool swipeInput = true;
    public bool showSettings = true;
    
    void Awake()
    {
        cm = (CameraManager)FindObjectOfType(typeof(CameraManager));
        wm = (WallManager)FindObjectOfType(typeof(WallManager));
        sceneManager = (SceneManager)FindObjectOfType(typeof(SceneManager));
        adm = (AdManager)FindObjectOfType(typeof(AdManager));
        lb = (LeaderBoard)FindObjectOfType(typeof(LeaderBoard));
    }

    void Start()
    {
        levelNum = PlayerPrefs.GetInt("Level", 1);

        //levelNum = 9;
        speedMin10 = 3.5f;
        speedMax10 = 4.2f;
        speedMin20 = 4.375f;
        speedMax20 = 5.25f;
        speedMin50 = 6f;
        speedMax50 = 7f;
        distMin10 = 45;
        distMax10 = 35;
        distMin20 = 90;
        distMax20 = 70;
        distMin50 = 85; //20
        distMax50 = 70; //10
        Difficulty();

        ChooseMesh();
        //Invoke("ColourSet", 1);

        lb.SubmitLevel(levelNum);

        adm.RequestInterstitial();
    }

    void Update()
    {
        CheckShapes();
        StartCoroutine(Ads());
    }

    void Difficulty()
    {

        if(levelNum < 11){
            laneNum = 3;

            shpCount = 1;

            wallNum = 5;

            speedLerp = (float)levelNum / 10;
            speed = Mathf.Lerp(speedMin10, speedMax10, speedLerp);

            distLerp = (float)levelNum / 10;
            dist = Mathf.Lerp(distMin10, distMax10, distLerp);
        }
        else if(levelNum > 10 && levelNum < 21){
            laneNum = 5;

            shpCount = 2;

            wallNum = 7;

            speedLerp = (float)levelNum / 20;
            speed = Mathf.Lerp(speedMin20, speedMax20, speedLerp);
        
            distLerp = (float)levelNum / 20;
            dist = Mathf.Lerp(distMin20, distMax20, distLerp);
        }
        else if(levelNum > 20){
            laneNum = 7;

            shpCount = 3;

            wallNum = 10;

            speedLerp = (float)levelNum / 50;// * (levelNum * Random.Range(1, 5));
            speed = Mathf.Lerp(speedMin50, speedMax50, speedLerp);

            distLerp = (float)levelNum / 50;
            dist = Mathf.Lerp(distMin50, distMax50, distLerp);
        }
        
        //cm.CamMove();
    }

    void ChooseMesh()
    {
        for (int i = 0; i < shpCount; i++)
        {
            shpNum.Add(Random.Range(0, shps.Length));
            //currShps.Add()
        }

        //shpNum = Random.Range(0, 3);
        GameObject firstShp = Instantiate(shps[shpNum[0]], new Vector3(7.06f, 38.69789f), Rotate(shpNum[0]));
        firstShp.GetComponent<ShapeMovement>().currentLane = laneNum / 2;
        currShps.Add(firstShp);
        
        if(shpCount > 1){
            int numTemp = 1;
            for (int i = 2; i < shpCount+1; i++)
            {
                if(i % 2 == 0){
                    GameObject shpTemp = Instantiate(shps[shpNum[numTemp]], new Vector3(7.06f, 38.69789f, 1.75f * (1.57142f*(shpCount -1*(shpCount-1)))), Rotate(shpNum[numTemp]));    //spawn shp on left
                    shpTemp.GetComponent<ShapeMovement>().currentLane = (2 * i - (3 - ((laneNum-5) / 2)));   //Lane = 2*SpawnOrder - 3    (3 has adjustments (must decrease by 1 every new 2 lanes))  
                    currShps.Add(shpTemp);
                    // if(shpTemp.transform.childCount == 1){
                    //     for (int k = 0; k < shpTemp.transform.GetChild(0).childCount; k++)
                    //     {
                    //         shpTemp.transform.GetChild(0).GetChild(k).gameObject.SetActive(false);
                    //     }
                    // }else{
                    //     for (int k = 0; k < shpTemp.transform.childCount; k++)
                    //     {   
                    //         shpTemp.transform.GetChild(k).gameObject.SetActive(false);
                    //     }
                    // }
                    numTemp = numTemp + 1;
                }else{
                    GameObject shpTemp = Instantiate(shps[shpNum[numTemp]], new Vector3(7.06f, 38.69789f, -1.75f * (1.57142f*(shpCount -2))), Rotate(shpNum[numTemp]));                 //spawn on right
                    shpTemp.GetComponent<ShapeMovement>().currentLane = (2 * i - (3 - ((laneNum-5) / 2)));          //((2 + ((laneNum-5) / 2)) * i) - (3 + ((laneNum-5) / 2));
                    currShps.Add(shpTemp);
                    // if(shpTemp.transform.childCount == 1){
                    //     for (int k = 0; k < shpTemp.transform.GetChild(0).childCount; k++)
                    //     {
                    //         shpTemp.transform.GetChild(0).GetChild(k).gameObject.SetActive(false);
                    //     }
                    // }else{
                    //     for (int k = 0; k < shpTemp.transform.childCount; k++)
                    //     {   
                    //         shpTemp.transform.GetChild(k).gameObject.SetActive(false);
                    //     }
                    // }
                    numTemp = numTemp + 1;
                }
            }
        }

        //ColourSet();

        //shps[shpNum].SetActive(true);
    }

    void CheckShapes()
    {
        bool tempTrueCheck = false;
        for (int i = 0; i < shpCount; i++)
        {
            if(currShps != null){
                if(currShps[i].GetComponent<ShapeMovement>().correctLane)
                {
                    tempTrueCheck = true;
                }else{
                    tempTrueCheck = false;
                    break;
                }
            }
        }

        if(tempTrueCheck){
           allShpCorrect = true; 
        }else{
            allShpCorrect = false;
        }
    }

    IEnumerator Ads()
    {
        if(lost && adShown){
            yield return new WaitForSeconds(2.5f);
            
            adm.ShowInterstitial();
            adShown = false;
        }

        if(won && adShown){
            yield return new WaitForSeconds(4.5f);

            adm.ShowInterstitial();
            adShown = false;
        }
    }

    Quaternion Rotate(int shpNum)       //for shapes
    {
        Quaternion rot;
        switch (shpNum)
        {
            case 0:
                rot = Quaternion.Euler(0, 0, -15);
                break;
            case 1:
                rot = Quaternion.Euler(-90, 0, 0);
                break;
            case 2:
                rot = Quaternion.Euler(-75, 90, 0);
                break;
            // case 3:
            //     rot = Quaternion.Euler(0, 0, 0);//(-75, 90, 0);
            //     break;
            // case 4:
            //     rot = Quaternion.Euler(-105, -90, 0);   
            //     break;
            case 3:
                rot = Quaternion.Euler(0, 0, 0);
                break;
            case 4:
                rot = Quaternion.Euler(15, 90, 0);
                break;
            case 5:
                rot = Quaternion.Euler(0, 0, 0);
                break;
            default: 
                rot = Quaternion.identity;
                break;
        }
        return rot;
    }

    public void ColourSet()
    {
        colourPal = new string[,] { //road, shape, wall, bg
            {"#FF6969", "#EB9C59", "#FFEFAB", "#BA54E8"},   //red (default)
            //{"#5ABAFF", "#684BEB", "#46E8AC", "#75FF4D"},   //light blue
            {"#44FE4A", "#E1E833", "#FFC738", "#38EBD0"},    //green
            {"#FFD94B", "#E88838", "#FF433D", "#BEEB3D"},    //yellow
            {"#A64812", "#F2913D", "#D97925", "#F2913D"},     //orange
            {"#BF7839", "#F23005", "#F2D338", "#3C3F40"},
            {"#F20505", "#D9D0C1", "#8A00E5", "#63BBF2"},
            {"#D9AA52", "#FAF4EA", "#0D0D0D", "#B2B6B8"},
            {"#24A669", "#59331D", "#025930", "#F8F1E9"},
            {"#986AD9", "#0A0626", "#5632A6", "#413473"},
            {"#BFBEBD", "#F2D16D", "#D96704", "#121212"},
            {"#A60522", "#1BBF15", "#F2B807", "#F2360C"},
            {"#D96690", "#88E8F2", "#F2C9E0", "#89C2D9"},
            {"#734C36", "#F2DEA2", "#D9BD6A", "#BFCDD9"},
            {"#0A6ABF", "#94F250", "#35BDF2", "#0D0D0D"},
            {"#F26D78", "#656D73", "#048ABF", "#827CA6"},
            {"#0D0D0D", "#F2E205", "#84BF04", "#F23005"},
            {"#155259", "#BFD9D9", "#012326", "#F2F2F2"},
            {"#2F3973", "#F2F2F2", "#2F3973", "#111826"},
            {"#BF6586", "#F2C4C4", "#F2AD94", "#73364C"},
            {"#BF7636", "#7787A6", "#733E1F", "#D9B6A3"},
            {"#9CB6D9", "#1E2D40", "#A6786D", "#7897BF"},
            {"#F299A9", "#F2055C", "#F2055C", "#4AD9CA"},
            {"#B3ECF2", "#F277A4", "#732240", "#D9486E"}};  

        colourCur = Random.Range(0, colourPal.Length / 4);

        Color shpColour = new Color();
        Color wallColour = new Color();
        Color roadColour = new Color();
        Color bgColour = new Color();
        ColorUtility.TryParseHtmlString(colourPal[colourCur, 1], out shpColour);
        ColorUtility.TryParseHtmlString(colourPal[colourCur, 2], out wallColour);
        ColorUtility.TryParseHtmlString(colourPal[colourCur, 0], out roadColour);
        ColorUtility.TryParseHtmlString(colourPal[colourCur, 3], out bgColour);

        for (int i = 0; i < currShps.Count; i++)
        {
            if(currShps[i].transform.childCount == 0){
                currShps[i].GetComponent<MeshRenderer>().materials[0].SetColor("shpColour", shpColour);
            }else{
                currShps[i].transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].SetColor("shpColour", shpColour);
            }

            wm.road.GetComponent<MeshRenderer>().materials[0].SetColor("planeColour", roadColour);

            Camera.main.GetComponent<Camera>().backgroundColor = bgColour;
        }

        for (int i = 0; i < wm.totalWalls.Count; i++)
        {
            //;
            for (int j = 0; j < wm.totalWalls[i].transform.childCount; j++)
            {
                
                wm.totalWalls[i].transform.GetChild(j).GetComponent<MeshRenderer>().materials[0].SetColor("wallColour", wallColour);
            }
        }
    }
}
