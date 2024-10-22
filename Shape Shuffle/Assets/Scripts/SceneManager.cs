using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{   //store inputType in PLayerPrefs
    [SerializeField] GameManager gm;
    [SerializeField] CameraManager cm;
    //[SerializeField] ShapeMovement sm;
    [SerializeField] WallManager wm;
    [SerializeField] ShapeMovement[] sms;

    [SerializeField] Canvas mainCan;
    [SerializeField] Text completedTxt, levelTxt, tap2playTxt;
    [SerializeField] Image tapImg, swipeImg;
    [SerializeField] Image dimPanel, pauseImg, unpauseImg;
    [SerializeField] InputField nameInpt;
    public Text[] levelLbNames;
    public Button inputBtn, pauseBtn;

    bool playGame;
    bool runOnce = false;
    public bool paused;

    void Awake()
    {
        //PlayerPrefs.SetInt("Level", gm.levelNum);
    }

    void Start()
    {
        //PlayerPrefs.DeleteKey("Level");

        levelTxt.text = PlayerPrefs.GetInt("Level", 1).ToString();
        gm.swipeInput = (bool)(PlayerPrefs.GetInt("InputType", 0) == 1);
        UpdateInput();
    }

    void Update()
    {
        if(sms[0] == null){                                         //assign shapemovements
            sms = GameObject.FindObjectsOfType<ShapeMovement>();
        }

        if(playGame){               //pressed play
            cm.CamMove();

            OnWon();        //check won
            OnLost();
        }
    
        UpdatePause();
    }

    public void Pause()
    {
        for (int i = 0; i < sms.Length; i++)
        {
            if(!paused){    //pause

                paused = true;
                Time.timeScale = 0;
                
            }else{          //unpause

                paused = false;
                Time.timeScale = 1;
            }
        }
    }

    public void PlayButton()
    {
        ActivateScripts();
        print("click");
    }

    void ActivateScripts()  //everythinh to happen once, once pressed play
    {
        playGame = true;

        for (int i = 0; i < gm.shpCount; i++)
        {
            sms[i].enabled = true;
            gm.currShps[i].transform.SetParent(null);
        }
        cm.enabled = true;

        cm.sm = gm.currShps[0].gameObject.GetComponent<ShapeMovement>();
    }

    void OnWon()
    {
        if(gm.won){ 
            completedTxt.enabled = true;

            Invoke("LoadNextSccene", 5);
            
        }
    }

    void OnLost()
    {
        if(gm.lost){
            Invoke("LoadSameSccene", 3);
        }
    }

    void LoadNextSccene()
    {
        
        if(!runOnce){
            gm.won = false;
            playGame = false;

            int levelTemp = 0;
            levelTemp = PlayerPrefs.GetInt("Level", 1);
            levelTemp += 1;
            PlayerPrefs.SetInt("Level", levelTemp);

            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            runOnce = true;
        }
        
    }

    void LoadSameSccene()
    {
        if(!runOnce){
            gm.lost = false;
            playGame = false;

            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            runOnce = true;
        }
    } 
    

    public void ShowSettings()
    {
        if(gm.showSettings){
            inputBtn.gameObject.SetActive(false); 
            gm.showSettings = false;
        }else{
            inputBtn.gameObject.SetActive(true);
            gm.showSettings = true;
        }
    }

    public void SwitchInput()
    {
        if(gm.swipeInput){
            swipeImg.enabled = false;
            tapImg.enabled = true;
            gm.swipeInput = false;
        }else{
            swipeImg.enabled = true;
            tapImg.enabled = false;
            gm.swipeInput = true;
        }

        PlayerPrefs.SetInt("InputType", (gm.swipeInput ? 1 : 0));
        print("saved" + PlayerPrefs.GetInt("InputType"));
    }
    void UpdateInput()
    {
        if(gm.swipeInput){
            swipeImg.enabled = true;
            tapImg.enabled = false;
        }else{
            swipeImg.enabled = false;
            tapImg.enabled = true;
        }
    }

    void UpdatePause()
    {
        if(paused){
            pauseImg.enabled = true;
            unpauseImg.enabled = false;
            dimPanel.enabled = true;
        }else{
            pauseImg.enabled = false;
            unpauseImg.enabled = true;
            dimPanel.enabled = false;
        }

    }
}
