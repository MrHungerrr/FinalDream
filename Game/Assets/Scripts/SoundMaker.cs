using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMaker : MonoBehaviour
{
   public float rad;
   public EnemyHelperAI enemyHelpAI;
   

   private void OnCollisionEnter(Collision collision)
   {
      enemyHelpAI.Sound(transform.position, rad);
   }


}
