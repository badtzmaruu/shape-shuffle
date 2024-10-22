using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{
    GameManager gm;
    ShapeMovement shapeMovement;

    public List<GameObject> wallShps = new List<GameObject>();  //temp
    public List<GameObject> sortedWallShps = new List<GameObject>();
    public List<GameObject> correctWalls = new List<GameObject>();
    public List<GameObject> totalWalls = new List<GameObject>();
    
    List<GameObject> leftWalls = new List<GameObject>();
    
    public GameObject[] walls;  //main
    GameObject startRoadPos;
    GameObject wallParent;
    GameObject correctWall, wallParentClean;

    [HideInInspector] public GameObject road;

    float currDist;
    
    public int lC = 0;
    public int rC = 0;

    void Start()
    {
        shapeMovement = (ShapeMovement)FindObjectOfType(typeof(ShapeMovement));
        gm = (GameManager)FindObjectOfType(typeof(GameManager));

        wallParentClean = GameObject.FindGameObjectWithTag("wallParentClean");
        startRoadPos = GameObject.Find("startRoadPos");

        Execute(10, gm.dist);

    }

    void Update()
    {
        
    }

    void Execute(int wallNum, float dist) //dist 40
    {       

        for (int i = 1; i <= gm.wallNum; i++)
        {
            Build(dist);    
        }

        // //change width of road
        float scaleXRoad = ((float)gm.dist*gm.wallNum/192.45f) - 0.013404f;     // 1 : 192.45f

        startRoadPos.transform.localScale = new Vector3(scaleXRoad, 1, 1);
        road.transform.localScale = new Vector3(road.transform.localScale.x, 1, (float)(gm.laneNum * 125.066) / 50.0266f);

        gm.ColourSet();
    }

    void Build(float dist)    //leftz -21.56 centrez -13.3278 rightz -27.054 filler1 -22.935 filler2 -25.682
    {
        //build wall
        
        wallParent = Instantiate(wallParentClean, Vector3.zero, Quaternion.identity);

        correctWall = Instantiate(walls[gm.shpNum[0]], Vector3.zero, Rotate(gm.shpNum[0]));
        //correctWall.transform.parent = wallParent.transform;


        float zR = 0;
        float zL = 0;

        

        for (int i = 2; i < gm.laneNum + 1; i++)    //left side
        {
            if(i % 2 == 1){

                bool cWall = false;
                int l;

                lC += 1;
                if(lC < gm.shpCount - 1 * (gm.shpCount-2)){
                    if(gm.shpCount == 1){
                        l = Random.Range(0, walls.Length-1);
                        cWall = false;
                    }else
                    {
                        l = gm.shpNum[lC];
                        cWall = true;   
                    }
                }else{
                    l = Random.Range(0, walls.Length-1);
                    cWall = false;
                }
                
                zL += 1.375f;
                GameObject wallLTemp = Instantiate(walls[l], new Vector3(0,0,zL), Rotate(l));//Instantiate(walls[walls.Length - 1], new Vector3(0,0,zR), Rotate(100));
                //wallLTemp.transform.SetParent(wallParent.transform);
                leftWalls.Add(wallLTemp);

                if(cWall){
                    correctWalls.Add(wallLTemp);
                    cWall = false;
                }

            }else if(i % 2 == 0){
                zL += 1.375f;
                GameObject wallLTemp = Instantiate(walls[walls.Length - 1], new Vector3(0,0,zL), Rotate(100));
                wallLTemp.transform.SetParent(wallParent.transform);
            }
        }

        wallShps.Add(correctWall);              //add middle in list
        correctWalls.Add(correctWall);          //add middle in the list
        
        for (int i = 2; i < gm.laneNum + 1; i++)    //right side
        {
            if(i % 2 == 1){
                
                bool cWall = false;
                int r;
                rC += 1;
                if(rC < gm.shpCount-1){
                    r = gm.shpNum[rC += (gm.shpCount-2)];    //this is questionable, may cause problems later
                    cWall = true;
                }else{
                    r = Random.Range(0, walls.Length-1);
                    cWall = false;
                }
                
                zR -= 1.376f;
                GameObject wallRTemp = Instantiate(walls[r], new Vector3(0,0,zR), Rotate(r));
                //wallRTemp.transform.SetParent(wallParent.transform);
                wallShps.Add(wallRTemp);

                if(cWall){
                    correctWalls.Add(wallRTemp);
                    cWall = false;
                }

            }else if(i % 2 == 0){
                
                zR -= 1.376f;
                GameObject wallRTemp = Instantiate(walls[walls.Length - 1], new Vector3(0,0,zR), Rotate(100));
                wallRTemp.transform.SetParent(wallParent.transform);
            }
        }



        Swop();

        Spawn(dist);
        // spawn new wall
        

    }

    void Swop()
    {

        //add leftWall list
        int iTemp = 0;
        for (int i = leftWalls.Count-1; i > -1 ; i--)
        {
            wallShps.Insert(iTemp, leftWalls[i]);
            ++iTemp;
        }
        
        //detect lane

        int lMoves, rMoves;
        int openLanes = gm.laneNum - gm.shpCount;
        if(openLanes % 2 == 0){
            lMoves = openLanes / 2;
            rMoves = openLanes / 2;
        }else{
            lMoves = openLanes / 2;
            rMoves = openLanes / 2 + 1; 
        }

        //swop
        Vector3[] oWallPos = new Vector3[wallShps.Count];
        Vector3[] cWallPos = new Vector3[correctWalls.Count];
        
        int randShp = Random.Range(-lMoves,rMoves+1);
        //print(randShp);
        
        for (int k = 0; k < wallShps.Count; k++)
        {   
            try
            {
                oWallPos[k] = wallShps[k].transform.position;    
                cWallPos[k] = correctWalls[k].transform.position;    
               
            }
            catch (System.Exception){}
        }
        
        if(randShp > 0){       //left shift
            for (int j = 0; j < gm.shpCount; j++)
            {
                if(gm.shpCount == 1){
                    correctWalls[j].transform.position = oWallPos[0];    
                }else{
                    correctWalls[j].transform.position = oWallPos[j + gm.shpCount-1 + randShp];   //2 and randShp is offset
                }
            
                if(j < randShp){
                    if(gm.shpCount == 1){
                       wallShps[0].transform.position = cWallPos[j];
                    }else{
                        wallShps[gm.shpCount + gm.shpCount-1 + j].transform.position = cWallPos[j];
                    }
                    //print(cWallPos[j]);
                }
            }
            
        }else if(randShp < 0){                  //right shift
            int iLeftSwop = 0;
            for (int l = gm.shpCount-1; l > -1; l--)
            {
                if(gm.shpCount == 1){                                                    //remove randShp oz error thrown when shpCount = 0
                    correctWalls[iLeftSwop].transform.position = oWallPos[2];    
                }else{
                    correctWalls[iLeftSwop].transform.position = oWallPos[iLeftSwop + gm.shpCount-1 + randShp];   //2 and randShp is offset    
                }
                
                if(iLeftSwop < -randShp){
                    if(gm.shpCount != 1){
                        wallShps[gm.laneNum - gm.shpCount - gm.shpCount-(3-gm.shpCount) - iLeftSwop].transform.position = cWallPos[l];
                    }else{
                        wallShps[2].transform.position = cWallPos[l];
                    }
                    
                }
                ++iLeftSwop;
                
            }
        }
        
        Sort();
    }

    void Sort() 
    {
        var descendingComparer = Comparer<float>.Create((x, y) => y.CompareTo(x));
        SortedList<float, float> descSortedList = new SortedList<float, float>(descendingComparer);

        foreach(GameObject go in wallShps){
            descSortedList.Add(go.transform.position.z, go.transform.position.z);
        }
        for (int i = 0; i < descSortedList.Count; i++)
        {
            for (int j = 0; j < wallShps.Count; j++)
            {
                if(descSortedList.Keys[i] == wallShps[j].transform.position.z){
                    sortedWallShps.Add(wallShps[j]);
                }   
            }
        }
    }

    void Spawn(float dist)
    {
        for (int i = 0; i < wallShps.Count; i++)
        {
            sortedWallShps[i].transform.SetParent(wallParent.transform);
        }
        
        totalWalls.Add(wallParent);

        currDist += dist;
        wallParent.transform.position = new Vector3(Mathf.Sin(1.308997f) * currDist, Mathf.Cos(1.308997f) * -currDist + 40.56276f, 0);  //+offset
        correctWalls.Clear();
        leftWalls.Clear();
        wallShps.Clear();
        sortedWallShps.Clear();
        lC = 0;
        rC = 0;

    }

    public void WallExit(GameObject wallPassed)
    {
        wallPassed.GetComponent<AudioSource>().Play();
        wallPassed.LeanMoveX(-10, 3);
    }

    Quaternion Rotate(int shpNum)   //for walls
    {
        Quaternion rot;
        switch (shpNum)
        {
            case 0:
                rot = Quaternion.Euler(15, 90, 0);
                break;
            case 1:
                rot = Quaternion.Euler(15, 90, 0);
                break;
            case 2:
                rot = Quaternion.Euler(15, 90, 90);
                break;
            // case 3:
            //     rot = Quaternion.Euler(15, 90, 0);
            //     break;
            // case 4:
            //     rot = Quaternion.Euler(15, 90, 0);
            //     break;
            case 3:
                rot = Quaternion.Euler(15, 90, -90);
                break;
            case 4:
                rot = Quaternion.Euler(15, 90, 0);
                break;
            case 5:
                rot = Quaternion.Euler(15, 90, -90);
                break;
            default:
                rot = Quaternion.Euler(15,90,0);
                break;
        }

        return rot;
    }

    public int ShpNum(string typeOfShapeOrTag)
    {
        switch (typeOfShapeOrTag)
        {
            case "sqr":
                return 0;
            case "circ":
                return 1;
            case "tri":
                return 2;
            // case "icosph":
            //     return 4;
            case "astro":
                return 3;
            case "donut":
                return 4;
            case "egg":
                return 5;
            default:
                return 0;
        }
    }
}
