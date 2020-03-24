using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class Restart : MonoBehaviour, IPointerClickHandler
{
    public SpriteRenderer thisSprite;

    public GameObject globalVolume;

    public Spawner spawnerReference;
    public TMP_Dropdown developerTool;

    public void DontDoThis()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch(developerTool.options[developerTool.value].text)
        {
          case "Restart Scene":
                SceneManager.LoadScene(SceneManager.GetActiveScene().name); break;
          // case "Add Card":
          //       StartCoroutine(spawnerReference.CardSpawn()); break;
          case "Full Restart":
                SceneManager.LoadScene(0); break;
          case "PostProcess.Active?":
                {
                  if(!globalVolume.activeSelf)
                    globalVolume.SetActive(true);
                  else
                    globalVolume.SetActive(false);
                } break;
        }

    }

    private void Start() {
        //Application.targetFrameRate = 60;
    }

}
