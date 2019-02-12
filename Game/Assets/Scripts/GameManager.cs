using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private bool pause = false;
    
    public GameObject player;
    public GameObject PauseUI;
    public GameObject Pager;


    void Update () {
	
      if(Input.GetKeyDown(KeyCode.Escape) && !Pager.activeSelf)
         if(!pause)
         {
                Time.timeScale = 0;
                pause = true;
                PauseUI.SetActive(true);
                player.GetComponent<PlayerScript>().enabled = false;
         }
         else
         {
                Time.timeScale = 1;
                pause = false;
                PauseUI.SetActive(false);
                player.GetComponent<PlayerScript>().enabled = true;
         }

      if (Input.GetKeyDown(KeyCode.Tab) && !PauseUI.activeSelf)
      {
            Pager.SetActive(!Pager.activeSelf);
            player.GetComponent<PlayerScript>().enabled = !player.GetComponent<PlayerScript>().enabled;
      }

	}
}
