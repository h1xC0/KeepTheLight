using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{

    public Button lightButton;
    public Button playButton;
    public Button returnButton;
    public Button settingsButton;
    public Button exitButton;
    public Dropdown graphicsDropdown;

    private Animator lightButtonAnimator;

    public GameObject globalLight;
    public GameObject target;
    public bool moveToTarget;

    private Vector3 startPos;

    [SerializeField]
    private float timeMultiplier;
    private bool nextScene;

    public ManageScenes sceneManageReference;


    void Start()
    {
      lightButtonAnimator = lightButton.GetComponent<Animator>();
      Application.targetFrameRate = 60;
      //PopulateDropdown (graphicsDropdown, dropdownObjects);
      int qualityLevel = QualitySettings.GetQualityLevel();
      graphicsDropdown.value = qualityLevel;

      startPos = Camera.main.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // lightButton.onClick.AddListener(TurnLights);
        // settingsButton.onClick.AddListener(OpenSettingsPanel);
        // returnButton.onClick.AddListener(delegate {
        //   StartCoroutine(CloseSettingsPanel());
        // });
        // graphicsDropdown.onValueChanged.AddListener(delegate {
        //     ChangeValueDropdown();
        // });

        if(moveToTarget)
          Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, new Vector3(target.transform.position.x, target.transform.position.y , -10), timeMultiplier * Time.deltaTime);
        else
          Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, startPos, timeMultiplier * Time.deltaTime);

        //if(Input.GetMouseButtonDown(0))
          // playButton.onClick.AddListener(TaskOnClick);

          if(nextScene)
          {
            sceneManageReference.StartTransition();
            // playButton.onClick.RemoveAllListeners();
            nextScene = false;
          }

        // exitButton.onClick.AddListener(ExitGame);
    }



    public void TurnLights()
    {
      if(globalLight.activeSelf)
      {
        globalLight.SetActive(false);
        lightButtonAnimator.SetBool("Activated?", false);
      }

      else
      {
        globalLight.SetActive(true);
        lightButtonAnimator.SetBool("Activated?", true);
      }
    }
    public void TaskOnClick()
    {
      nextScene = true;
    }

    public void OpenSettingsPanel()
    {
      settingsButton.transform.parent.transform.parent.gameObject.SetActive(false);
      moveToTarget = true;
      // graphicsDropdown.options = new List<Dropdown.OptionData>();
      // foreach(string item in dropdownOptions)
    }

    public void CloseSettings()
    {
        StartCoroutine(CloseSettingsPanel());
    }

    IEnumerator CloseSettingsPanel()
    {
      moveToTarget = false;
      yield return new WaitForSeconds(0.5f);
      settingsButton.transform.parent.transform.parent.gameObject.SetActive(true);

    }

    public void ChangeValueDropdown () {
      switch(graphicsDropdown.options[graphicsDropdown.value].text)
      {
        case "Very Low":
              QualitySettings.SetQualityLevel(0, true); break;
        case "Low":
              QualitySettings.SetQualityLevel(1, true); break;
        case "Medium":
              QualitySettings.SetQualityLevel(2, true); break;
        case "High":
              QualitySettings.SetQualityLevel(3, true); break;
        case "Very High":
              QualitySettings.SetQualityLevel(4, true); break;
        case "Ultra":
              QualitySettings.SetQualityLevel(5, true); break;
      }
 }

    public void ExitGame()
    {
      Application.Quit();
    }
}
