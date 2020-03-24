using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManageScenes : MonoBehaviour
{
    public bool nextScene = false;
    public bool clickedPlay = true;
    public Animator fadeTransition;
    [SerializeField]
    private float transitionTime;

    void Start()
    {
      // fadeTransition.SetBool("Fade", false);
    }

    // Update is called once per frame
    void Update()
    {
      if(nextScene && Time.frameCount % 2 == 0)
        ChangeScene();
    }

    public void ChangeScene()
    {
            SceneManager.LoadScene(1);

    }

    public void StartTransition()
    {
      StartCoroutine(LoadBattleLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadBattleLevel(int levelIndex)
    {
      clickedPlay = false;
      fadeTransition.SetTrigger("Fade");

      yield return new WaitForSeconds(transitionTime);

      SceneManager.LoadSceneAsync(levelIndex);


    }
}
