using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
   private CameraController camControl;
   public GameObject startText;
   public GameObject startText2;
   private InputManager inputMan;
   private Animator anim;
   private EnemyHelperAI eHelpAI;
   private bool cutSceneAct = true;
   private byte index = 0;
   public Collider first;
   public Collider second;
   public Collider third;
   public bool playDialog;
   private SubtitleGuiManager guiManager;

   private FMOD.Studio.EventInstance natureSound;
   private FMOD.Studio.ParameterInstance natureVolume;
   private FMOD.Studio.ParameterInstance natureLPF;



   void Awake()
   {
      camControl = GameObject.Find("Main Camera").GetComponent<CameraController>();
      anim = GetComponent<Animator>();
      eHelpAI = GameObject.Find("EnemyHelper").GetComponent<EnemyHelperAI>();
      inputMan = GameObject.Find("GameManager").GetComponent<InputManager>();
      playDialog = false;
      anim.enabled = false;
   }

   void Start()
   {
      guiManager = FindObjectOfType<SubtitleGuiManager>();
      natureSound = FMODUnity.RuntimeManager.CreateInstance("event:/Blizzard");
      natureSound.getParameter("LPFNature", out natureLPF);
      natureSound.getParameter("VolumeNature", out natureVolume);
      inputMan.cutScene = true;
      natureSound.start();
      natureLPF.setValue(22000);
      natureVolume.setValue(1f);
      StartCoroutine(DemoStart());
   }


   public void NatureSound(int cutOff, float volume)
   {
      natureLPF.setValue(cutOff);
      natureVolume.setValue(volume);
   }
   

   public void CutSceneEnd()
   {
      if (cutSceneAct)
      {
         cutSceneAct = false;
         anim.enabled = true;
         anim.SetTrigger("StartCS0");
         startText2.SetActive(false);
         inputMan.cutScene = false;
      }
   }

   private void EndCS0()
   {
      eHelpAI.night = true;
      eHelpAI.Night();
   }



   IEnumerator DemoStart()
   {
      while(cutSceneAct)
      {
         startText.SetActive(true);
         yield return new WaitForSeconds(1.5f);
         startText.SetActive(false);
         yield return new WaitForSeconds(1.5f);
      }
   }



   private void OnTriggerEnter(Collider other)
   {
      if (other.tag == "Player")
      {
         if (first.enabled == true)
         {
            first.enabled = false;
            eHelpAI.orblessAI[0].Manipulate(new Vector3(0, 0, 0));
         }
         else if (second.enabled == true)
         {
            second.enabled = false;
            eHelpAI.orblessAI[0].Kill(GameObject.FindGameObjectWithTag("Player"));
         } else
         {
            third.enabled = false;
            Time.timeScale = 0;
            inputMan.game = false;
            guiManager.SetText("Ты убежал! Поздравляю) Спасибо, что поиграл. Если есть что сказать - обязательно напиши!");
         }
      }
   }

}
