using System.Collections;
using UnityEngine;

public class EasyStory : MonoBehaviour
{
   [FMODUnity.EventRef]
   public string audioSource;
   public string[] subtitles;
   public float[] timing;
   public GameObject otherCondition;
   public bool badCondition;
   public GameObject[] linearActivate;
   public GameObject[] linearDisactivate;
   private SubtitleGuiManager guiManager;
   private LevelManager levManager;

   public bool door;
   public bool engine;
   public bool generator;



   private void Start()
   {
      levManager = FindObjectOfType<LevelManager>();
      guiManager = FindObjectOfType<SubtitleGuiManager>();
   }



   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Player") && !levManager.playDialog)
      {
         if (door)
         {
            if ((otherCondition.GetComponent<Door>().lockDoor && badCondition) || (!otherCondition.GetComponent<Door>().lockDoor && !badCondition))
            {
               Subtitles();
            }
         }
         else if (engine)
         {
            if ((otherCondition.GetComponent<Engine>().power && !badCondition) || (!otherCondition.GetComponent<Engine>().power && badCondition))
            {
               Subtitles();
            }
         }
         else if (generator)
         {
            if ((otherCondition.GetComponent<Generator>().power && !badCondition) || (!otherCondition.GetComponent<Engine>().power && badCondition))
            {
               Subtitles();
            }
         }
         else 
         {
            Subtitles();
         }
      }
   }



   private void Subtitles()
   {
      levManager.playDialog = true;
      FMODUnity.RuntimeManager.PlayOneShot(audioSource);
      GetComponent<BoxCollider>().enabled = false;
      StartCoroutine(DoSubtitle());

      if (linearActivate.Length > 0)
         for (int i = 0; i < linearActivate.Length; i++)
         {
            linearDisactivate[i].SetActive(true);
         }
      if (linearDisactivate.Length > 0)
         for (int i = 0; i < linearDisactivate.Length; i++)
         {
            linearDisactivate[i].SetActive(false);
         }
   }



   private IEnumerator DoSubtitle()
   {
      for (int i = 0; i < subtitles.Length; i++)
      {
         guiManager.SetText(subtitles[i]);
         yield return new WaitForSeconds(timing[i]);
         guiManager.Clear();
         yield return new WaitForSeconds(0.05f);
         if( i == subtitles.Length - 1)
         {
            levManager.playDialog = false;
         }
      }
   }
}
