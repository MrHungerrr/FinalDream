using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

    public Item[] Items;

    //public Transform  inventoryTransform;
    public GameObject Pager;
    public GameObject Container;
    public Image DescriptionImage;
    public GameObject file;
    public GameObject audioNotes;
    private GameObject notecanavsimage;

    public GameObject PressE;
    public GameObject NoteChecked;

    public int KristalCount = 0;//количество кристалов

    private string ipath;

    int k = 0;
    int n = 0;

    private void Start()
    {
        Items = new Item[30];
        notecanavsimage = audioNotes.transform.Find("NoteCanavs").GetChild(0).gameObject;
    }

    void Update ()
    {
        
        if (Pager.transform.Find("Files").gameObject.activeInHierarchy)
        {

            if (Input.GetKeyDown(KeyCode.DownArrow) && k < n-1)
                k++;
            else if (k > 0 && Input.GetKeyDown(KeyCode.UpArrow))
                k--;

            if (n>0)
            {
                ipath = Items[k].sprite;
                DescriptionImage.color = new Color(1, 1, 1, 1);
                DescriptionImage.sprite = Resources.Load<Sprite>(ipath);
                DescriptionImage.preserveAspect = true;
                //DescriptionImage.gameObject.AddComponent<Button>().onClick.AddListener(() => OnButtonClick(Items[k]));
                file.transform.Find("Namefield").GetComponent<Text>().text = Items[k].Name + "\n" + Items[k].timecollected.ToShortDateString() + "\n" + Items[k].timecollected.ToLongTimeString();
                file.transform.Find("Typefield").GetComponent<Text>().text = Items[k].Type;
            }
            else
                DescriptionImage.color = new Color(0, 0, 0, 0);
            
        }




        if (Input.GetKeyDown(KeyCode.Tab))
        {
            /*
                        if (Pager.activeSelf)
                        {

                            for (int i = 0; i < Items.Count; i++)
                            {
                                Item item = Items[i];

                                if (inventoryTransform.GetChild(i).childCount == 0)
                                {
                                    GameObject img = Instantiate(Container, inventoryTransform.GetChild(i));
                                    img.GetComponent<Image>().sprite = Resources.Load<Sprite>(item.sprite);
                                    img.GetComponent<Image>().preserveAspect = true;
                                   
                                }
                                else break;
                            }

                        }
                        else
                        {
                            for (int i = 0; i < inventoryTransform.childCount; i++)
                            {
                                if (inventoryTransform.GetChild(i).childCount != 0)
                                {
                                    Destroy(inventoryTransform.GetChild(i).GetChild(0).gameObject);
                                }
                            }
                        }
            */
        }


    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Item")
        {
            PressE.GetComponent<Text>().text = "Press 'E' to collect " + col.GetComponent<Item>().Name;
            PressE.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                PressE.SetActive(false);
                Items[n] = col.GetComponent<Item>();
                Items[n].iscollected = true;
                Items[n].timecollected = DateTime.Now;
                n++;
                if (col.GetComponent<Item>().prefab == ("prefabs/Кристал"))
                {
                    KristalCount++;
                }
                else if (col.GetComponent<Item>().prefab == "prefabs/Audio")
                {
                    audioNotes.GetComponent<AudioSource>().clip = col.GetComponent<AudioSource>().clip;
                    audioNotes.GetComponent<AudioSource>().Play();
                }
                else if (col.GetComponent<Item>().prefab == "prefabs/Note")
                {
                    notecanavsimage.GetComponent<Image>().sprite = col.GetComponent<Image>().sprite;
                    notecanavsimage.GetComponent<Image>().enabled = true;
                }
                col.gameObject.SetActive(false);
            }
        }
    }

    public void OnTriggerExit2D()
    {
        PressE.SetActive(false);
    }

   /* private Transform GetFreeSlot()
    {
        foreach (Transform t in inventoryTransform)
        {
            if (t.childCount == 0)
            {
                return t;
            }
        }

        return null;
    }
    */


    void OnButtonClick(Item obj)
    {
        if (obj.name[0] == 'A')
            {
                
            }
      /*  {
            if (!obj.GetComponent<AudioSource>())
            {
                obj.AddComponent<AudioSource>().clip = Resources.Load<AudioClip>("audio/диктофон");
            }
            obj.GetComponent<AudioSource>().Play();
        }
        else if (obj.GetComponent<Image>().sprite == Resources.Load<Sprite>("icons/записка"))
        {
            NoteChecked.SetActive(!NoteChecked.activeInHierarchy);
        }
            */
        else
        {

        }
        
    }
}
