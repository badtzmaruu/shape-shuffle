using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeMovement : MonoBehaviour
{
    GameManager gm;
    WallManager wm;
    SceneManager scm;

    public float xVec, yVec;
    float startTime;
    float swipeCooldn = 0.2f;
    //float tapCooldn = 0.2f;

    public int lMoves, rMoves;
    public int currentLane, currentWall;


    public Vector3 startTouchPos;
    public Vector3 touchStartClick;
    public Vector2 touchTempPos;

    public bool clicked, leftSwiped, rightSwiped, tempRSwipe, tempRTap;
    public bool correctLane;
    public bool accel;
    bool tapCooldn = true;

    public GameObject touchTemp;

    Rigidbody shpR;
    Transform shpT;

    void Awake()
    {
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
        wm = (WallManager)FindObjectOfType(typeof(WallManager));
        scm = (SceneManager)FindObjectOfType(typeof(SceneManager));

        touchTemp = GameObject.FindGameObjectWithTag("mouseTemp");

        // if(transform.position.z == 0){
        //     gm.centreShp = gameObject;
        // }
    }

    void Start()
    {
        shpR = GetComponent<Rigidbody>();
        shpT = GetComponent<Transform>();

        int openLanes = gm.laneNum - gm.shpCount;
        if(openLanes % 2 == 0){
            lMoves = openLanes / 2;
            rMoves = openLanes / 2;
        }else{
            lMoves = openLanes / 2;
            rMoves = openLanes / 2 + 1; 
        }
    }

    void Update()
    {
        StartCoroutine(Test());
        Shuffle();
        LaneDetect();
        Wind();
    }

    public void Move(Rigidbody r, Transform wallTrans)
    {
        xVec = Mathf.Sin(1.308997f);             //75 * (Mathf.PI/180)
        yVec = Mathf.Cos(1.308997f);             //75 * (Mathf.PI/180)

        if(currentWall == 0){               //first wall
            //r.AddForce(xVec * gm.speed * gm.speedPhone, -yVec * gm.speed * gm.speedPhone, 0);
            r.AddForce(xVec * gm.speed * 0.9f * gm.speedPhone, -yVec * gm.speed * 0.9f * gm.speedPhone, 0);
        }
        else if(wallTrans.position.x - transform.position.x < gm.dist * 0.75f
                && currentWall != 0){       //first speed up
            accel = true;
            r.AddForce(xVec * gm.speed * 0.9f * gm.speedPhone, -yVec * gm.speed * 0.9f * gm.speedPhone, 0);

        }else{                              //slow down
            accel = false;
            r.AddForce(-xVec * gm.speed * 3f * gm.speedPhone, yVec * gm.speed * 3 * gm.speedPhone, 0);

        }
    }

    void Shuffle()
    {
        // if(gm.lost)
        //     yield return null;

        //swipe input
        
        Vector3 shapePos = transform.position;
        //const float MAX_SWIPE_TIME = 3f;
        //const float MIN_SWIPE_TIME = 0.1f;
        //const float MIN_SWIPE_DISTANCE = 0f;

        if(gm.swipeInput){
            swipeCooldn += Time.deltaTime;

            //if(swipeCooldn < 0.1f)
               // return;

            if(Input.touchCount == 1){

                    Touch t = Input.GetTouch(0);
                    if(t.phase == TouchPhase.Began){
                        startTouchPos = new Vector2(t.position.x/(float)Screen.width, t.position.y/(float)Screen.width);
                        startTime = Time.time;
                    }
                    if(t.phase == TouchPhase.Ended){
                        // if (Time.time - startTime > MAX_SWIPE_TIME) // press too long
                        //     yield return null;

                        Vector2 endPos = new Vector2(t.position.x/(float)Screen.width, t.position.y/(float)Screen.width);

		    		    Vector2 swipe = new Vector2(endPos.x - startTouchPos.x, endPos.y - startTouchPos.y);

                        // if (swipe.magnitude < MIN_SWIPE_DISTANCE) // Too short swipe
                        //     yield return null;


                        if (Mathf.Abs (swipe.x) > Mathf.Abs (swipe.y)) { // Horizontal swipe
                            if (swipe.x > 0) {
                                if(rMoves > 0){
                                    rightSwiped = true;
                                }
		    			    }
                            else {
                                if(lMoves > 0){
                                    leftSwiped = true;
                                }    
		    			    }
		    		    }
    
                    // else { // Vertical swipe                 //future use
		    		// 	if (swipe.y > 0) {
		    		// 		swipedUp = true;
		    		// 	}
		    		// 	else {
		    		// 		swipedDown = true;
		    		// 	}
		    		// }

                    }


                    // if(touchStartClick.x < startTouchPos.x){

                    //     if(rMoves > 0){       //shapePos.z > -1.75f * ((gm.laneNum -1) / 2
                    //         rightSwiped = true;
                    //     }

                    // }else if(touchStartClick.x > startTouchPos.x){

                    //     if(lMoves > 0){       //shapePos.z < 1.75f * ((gm.laneNum -1) / 2)
                    //         leftSwiped = true;
                    //     }

                    // }
                //}

            }

            if(leftSwiped){
                shapePos.z = shapePos.z + 2.75f;
                LeanTween.cancel(gameObject);
                LeanTween.moveZ(gameObject, shapePos.z, 0.1f).setEaseOutBack();
                --lMoves;
                ++rMoves;
                --currentLane;
                if(tempRSwipe)
                    swipeCooldn = 0;
                tempRSwipe = false;
                leftSwiped = false;

            }

            if(rightSwiped){
                shapePos.z = shapePos.z - 2.75f;
                LeanTween.cancel(gameObject);
                LeanTween.moveZ(gameObject, shapePos.z, 0.1f).setEaseOutBack();
                ++lMoves;
                --rMoves;
                ++currentLane;
                if(!tempRSwipe)
                    swipeCooldn = 0;
                tempRSwipe = true;
                rightSwiped = false;

            }
        }
        else{

            if(Input.GetKeyDown(KeyCode.A)){
                    if(lMoves > 0){          //shapePos.z < 1.75f * ((gm.laneNum -1) / 2)
                        shapePos.z = shapePos.z + 2.75f;
                        LeanTween.cancel(gameObject);
                        LeanTween.moveZ(gameObject, shapePos.z, 0.1f).setEaseOutBack();
                        // transform.position = shapePos;
                        --lMoves;
                        ++rMoves;
                        --currentLane;
                        // if(tempRTap)
                        //     tapCooldn = 0;
                    }
                }else if(Input.GetKeyDown(KeyCode.D)){
                    if(rMoves > 0){       //shapePos.z > -1.75f * ((gm.laneNum -1) / 2
                        shapePos.z = shapePos.z - 2.75f;
                        LeanTween.cancel(gameObject);
                        LeanTween.moveZ(gameObject, shapePos.z, 0.1f).setEaseOutBack();
                        //transform.position = shapePos;
                        ++lMoves;
                        --rMoves;
                        ++currentLane;
                        // if(!tempRTap)
                        //     tapCooldn = 0;
                    }
                }
            if(Input.touchCount > 0){

                //tapCooldn += Time.deltaTime;

                //if(tapCooldn < 0.1f)
                    //return;

                if(!tapCooldn){
                    return;
                }

                StartCoroutine(SpamTap());

                Touch t = Input.GetTouch(0);
                if(t.phase == TouchPhase.Began){

                    if(t.position.x < Screen.width/2){    //lefr side

                        if(lMoves > 0){          //shapePos.z < 1.75f * ((gm.laneNum -1) / 2)
                            shapePos.z = shapePos.z + 2.75f;
                            LeanTween.cancel(gameObject);
                            LeanTween.moveZ(gameObject, shapePos.z, 0.1f).setEaseOutBack();
                            // transform.position = shapePos;
                            --lMoves;
                            ++rMoves;
                            --currentLane;
                            // if(tempRTap)
                            //     tapCooldn = 0;
                        }
                    }else if(t.position.x > Screen.width/2){  //rigjt side

                        if(rMoves > 0){       //shapePos.z > -1.75f * ((gm.laneNum -1) / 2
                            shapePos.z = shapePos.z - 2.75f;
                            LeanTween.cancel(gameObject);
                            LeanTween.moveZ(gameObject, shapePos.z, 0.1f).setEaseOutBack();
                            //transform.position = shapePos;
                            ++lMoves;
                            --rMoves;
                            ++currentLane;
                            // if(!tempRTap)
                            //     tapCooldn = 0;
                        }
                    }
                
                }
            }
        }
    }

    IEnumerator SpamTap()
    {
        tapCooldn = false;
        yield return new WaitForSeconds(0.1f);
        tapCooldn = true;
    }


    IEnumerator Test()
    {
        if(Input.GetKeyDown(KeyCode.A)){
            
            print("left");yield return new WaitForSeconds(3);
        }
        if(Input.GetKeyDown(KeyCode.D)){
            
            print("right");yield return new WaitForSeconds(3);
        }
    }

    void LaneDetect()
    {
        if(scm.paused)
            return;

        if(currentWall >= gm.wallNum){
            gm.won = true;
            if(transform.childCount == 0){
                gameObject.GetComponent<MeshRenderer>().enabled = false;
            }else{
                gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            }
            //GetComponent<MeshRenderer>().enabled = false;
            return;
        }
        

        Transform wallTransform = wm.totalWalls[currentWall].transform;

        if(gm.lost)
            return;

        Move(shpR, wallTransform);
        
        if(transform.position.x > wallTransform.position.x){        //make offset to make delay or quicker reaction (+pos.x)
            if(gm.allShpCorrect){
                wm.WallExit(wallTransform.gameObject);
                ++currentWall;
            }
            else{
                //print("dead");
                return;
            }
        }

        Transform singleWall = wallTransform.GetChild((gm.laneNum-1) + currentLane);
        string wallName = singleWall.gameObject.tag;

        if(tag == wallName){
            singleWall.GetChild(0).gameObject.GetComponent<BoxCollider>().enabled = false;
            correctLane = true;
        }else{
            singleWall.GetChild(0).gameObject.GetComponent<BoxCollider>().enabled = true;
            correctLane = false;
        }
    }
    
    void OnCollisionEnter(Collision col)
    {
        if(col != null){
            gm.lost = true;
            shpR.AddForce(new Vector3(-700, Random.Range(300, 500), Random.Range(-700, 700)));
        }
    }

    void Wind()
    {
        TrailRenderer tr;
            
        tr = GetComponent<TrailRenderer>();
        
        if(gm.won){
            //tr.enabled = false;
            gameObject.SetActive(false);
        }

        if(accel){
            if(tr.time < 0.42f * gm.levelNum*0.5f)
                tr.time += 0.001f;
        }else{
            if(tr.time > 0)
                tr.time -= 0.01f;
        }
         
    }

}
