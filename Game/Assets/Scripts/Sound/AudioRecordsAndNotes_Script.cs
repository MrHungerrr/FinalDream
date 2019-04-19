using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AudioRecordsAndNotes
{
    public static class AudioRecords_Script
    {
        static int count = 0;
        private static string[] AudioRecords = new string[1]; //массив звукозаписей

        public static void AddAudioRecord(string path)
        {
            AudioRecords[count] = path;
            AudioListening(count);
            count++;
        }

        public static void AudioListening(int k)
        {
            if (AudioRecords[k] != null)
            {
                FMODUnity.RuntimeManager.PlayOneShot(AudioRecords[k]);
            }
        }
    }
    public static class Notes_Script
    {
        public static bool cheking = false;
        private static GameObject note = null;
        public static GameObject NoteImage = GameObject.Find("NoteImage");

        public static void OpenNote(string path, GameObject note)
        {
            NoteImage.GetComponent<Image>().sprite =  note.GetComponent<Image>().sprite;
            NoteImage.GetComponent<Image>().enabled = true;
            FMODUnity.RuntimeManager.PlayOneShot(path);
            cheking = true;
        }

        public static void CloseNote(string path)
        {     
            FMODUnity.RuntimeManager.PlayOneShot(path);
            NoteImage.GetComponent<Image>().enabled = false;
            cheking = false;
        }
    }
}



