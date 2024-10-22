using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public _Sounds[] sounds;

    WallManager wm;
    GameManager gm;
    CameraManager cm;

    void Start()
    {
        Invoke("SetSounds", 0.2f);
        
    }

    void SetSounds()
    {
        wm = (WallManager)FindObjectOfType(typeof(WallManager));
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
        cm = (CameraManager)FindObjectOfType(typeof(CameraManager));

        foreach(_Sounds s in sounds)
        {
            if(s.name == "wallPass"){
                for (int i = 0; i < gm.wallNum; i++)
                {
                    s.source = wm.totalWalls[i].AddComponent<AudioSource>();
                    
                    s.source.clip = s.clip;
                    s.source.playOnAwake = s.playWhenAwake;
                    
                    s.source.volume = s.volume;
                    s.source.pitch = s.pitch;
                }
            }

            if(s.name == "wallCrash"){
                s.source = cm.gameObject.AddComponent<AudioSource>();

                s.source.clip = s.clip;
                s.source.playOnAwake = s.playWhenAwake;
                
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
            }
            
        }
    }

    void Awake()
    {
        
    }

    
    void Update()
    {
        
    }
}
