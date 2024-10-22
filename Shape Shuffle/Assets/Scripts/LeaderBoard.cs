using System.Collections;
using System.Collections.Generic;
using LootLocker.Requests;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{

    SceneManager sm;

    string playerName;

    public int levelLbId;

    public bool hasName;

    void Awake()
    {
        if(PlayerPrefs.HasKey("PlayerID")){
            hasName = true;
        }else{
            hasName = false;
        }
    }

    void Start()
    {
        sm = (SceneManager)FindObjectOfType(typeof(SceneManager));



        
        LootLockerSDKManager.StartSession("Player", (response) =>
        {    
        
            if(response.success){

                print("y");

            }else{

                print("n");

            }
        });

    
    }

    
    void Update()
    {
        
    }

    public void ShowScores()
    {
        LootLockerSDKManager.GetScoreList(levelLbId, 10, (response) => 
        {
            if(response.success){

                LootLocker.Requests.LootLockerLeaderboardMember[] levels = response.items;

                for (int i = 0; i < levels.Length; i++)
                {
                    sm.levelLbNames[i].text = levels[i].rank + ". " + levels[i].member_id + "\t"+"\t"+"\t"+"\t"+ levels[i].score;
                }

            }else{

                print("n");

            }
        });
    }

    public void SubmitLevel(int levelNum)
    {
        if(!PlayerPrefs.HasKey("PlayerID")){ return; }
        LootLockerSDKManager.SubmitScore(PlayerPrefs.GetString("PlayerID", "000000"), levelNum, levelLbId, (response) =>
        {
            if(response.success){

                print("y");

            }else{

                print("n");

            }
        });
    }

    public void GetNameInput(InputField nameInpt)
    {
        playerName = nameInpt.text;
        PlayerPrefs.SetString("PlayerID", playerName);
        hasName = true;
        nameInpt.gameObject.SetActive(false);
    }

    public void EnterName(InputField nameInpt)
    {
        if(hasName){ return; }
        nameInpt.gameObject.SetActive(true);
    }
}
