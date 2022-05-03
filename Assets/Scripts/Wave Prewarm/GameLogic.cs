using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class GameLogic : MonoBehaviour
{
    [Header("Buttons,Images and Panels")]
    public Button endTurn;
    public Button next;
    public Button pauseButton;
    public Image playerTurnText;
    public Image enemyTurnText;
    public GameObject winPanel;
    public Image losePanel;
    public Image startDeckPanel;
    public TMP_Text newWave;
    public TMP_Text chooseText;

    [Header("Booleans")]
    public bool clickedEndTurn;
    public bool onClickPause;
    public bool _playerTurn;
    private bool clickedNext;
    public bool continueChoose = true;
    public bool canSelectStartDeck;
    public bool destroyUnselectedCards;
    public bool dissolveForNextRound = false;

    [Header("References")]
    public PlayerProperties playerPropertiesReference;
    private GameObject[] enemyPropertiesReference;
    public Spawner spawnerReference;
    private CardArranger cardarrangerReference;

    [Header("Objects")]
    public GameObject newWaveTransition;
    public GameObject enemyPrefab;
    public GameObject pausePanel;

    private Animator waveAnimator;
    private Animator waveTransitionAnimator;
    private Animator playerTurnTextAnimator;
    private Animator enemyTurnTextAnimator;

    [Header("Floats and Integers")]
    [SerializeField]
    private float fadeTime;
    [SerializeField]
    private int enemySpawnedCount;

    private float alpha = 0.8f;
    private int i;
    private int auxiliar;
    private int waveNumber = 1;

    void Awake()
    {
      Input.multiTouchEnabled = false;
    }


    void Start()
    {
      // Atribuim animatoarele la imaginile lor
      waveAnimator = newWave.gameObject.GetComponent<Animator>();
      waveTransitionAnimator = newWaveTransition.GetComponent<Animator>();
      playerTurnTextAnimator = playerTurnText.gameObject.GetComponent<Animator>();
      enemyTurnTextAnimator = enemyTurnText.gameObject.GetComponent<Animator>();

      cardarrangerReference = GameObject.FindWithTag("CardParent").GetComponent<CardArranger>();
      continueChoose = true;
      auxiliar = Random.Range(2,10);
      // Incepem un nou "Val"
      StartCoroutine(NewWaveStart());
    }

    void Update()
    {

      // endTurn.onClick.AddListener(TaskOnClickEndTurn);
      // pauseButton.onClick.AddListener(delegate {
      //   TaskOnPause();
      // });

      if (Input.GetKeyDown(KeyCode.Escape))
      {
          Application.Quit();
      }

      // Verificam daca nu am ales prea multe carti
      if(Time.frameCount %  fadeTime == 0)
        if(GameObject.FindGameObjectsWithTag("Draggable").Length > 0 && GameObject.FindGameObjectsWithTag("Draggable").Length
        <= startDeckPanel.transform.childCount / 2)
        {
          // next.onClick.AddListener(TaskOnClickNext);
          next.interactable = true;
          continueChoose = true;
        }
        else
        {
          continueChoose = true;
          next.interactable = false;
        }

        // Ascundem butonul "Next" dupa apasarea lui
          if(destroyUnselectedCards)
          {
            next.image.rectTransform.localScale = Vector3.Lerp(next.image.rectTransform.localScale, Vector3.zero, fadeTime * Time.deltaTime);
            if(next.image.rectTransform.localScale.x <= 0.08f)
              next.gameObject.SetActive(false);
          }
          if(!destroyUnselectedCards && canSelectStartDeck)
          {
            next.gameObject.SetActive(true);
            next.image.rectTransform.localScale = Vector3.Lerp(next.image.rectTransform.localScale, Vector3.one, fadeTime * Time.deltaTime);
          }

          enemyPropertiesReference = GameObject.FindGameObjectsWithTag("Enemy");

      // Schimbam opacitatea panelei pentru a alege cartile de la inceput
      if(Time.frameCount %  fadeTime == 0)
      {
        FadePanel();
        chooseText.text = "Choose cards " + GameObject.FindGameObjectsWithTag("Draggable").Length + "/" + startDeckPanel.transform.childCount / 2;
      }

      // Verificam daca nu am ales prea multe carti
      if(GameObject.FindGameObjectsWithTag("Draggable").Length >= startDeckPanel.transform.childCount / 2)
      {
        continueChoose = false;
      }
      // Verificam daca este vreun inamic pe scena
      if(GameObject.FindGameObjectsWithTag("Enemy").Length == 0 )
      {
        endTurn.interactable = false;
        clickedEndTurn = false;
        _playerTurn = false;
        // Stergem toate cartile pentru a incepe un nou wave
        if(GameObject.FindWithTag("CardParent").transform.childCount > 0)
          for(int i = 0; i < GameObject.FindWithTag("CardParent").transform.childCount; i++)
          {
            GameObject.FindWithTag("CardParent").transform.GetChild(i).gameObject.SetActive(false);
            GameObject.FindWithTag("CardParent").transform.GetChild(i).SetParent(GameObject.Find("Inactive Panel").transform);


              cardarrangerReference.numberOfCards = 0;
              cardarrangerReference.cards.Clear();
              cardarrangerReference.cardInterval.Clear();
            }
            winPanel.SetActive(true);
      }
        else
          StopCoroutine(NewWaveStart());
      // You died
      if(playerPropertiesReference.health == 0)
        {
          losePanel.gameObject.SetActive(true);
          endTurn.interactable = false;
          clickedEndTurn = false;
          _playerTurn = false;
        }

      // Facem respawn dupa apasarea butonului "Next"
        if(clickedNext)
        {
          StartCoroutine(spawnerReference.CardSpawn());
          clickedNext = false;
        }

        if(endTurn.interactable == false && clickedEndTurn == true)
        {
          dissolveForNextRound = true;
          // for(i = 0; i < GameObject.FindWithTag("CardParent").transform.childCount; i++)
          //   {
          //     GameObject.FindWithTag("CardParent").transform.GetChild(i).gameObject.SetActive(false);
          //     GameObject.FindWithTag("CardParent").transform.GetChild(i).SetParent(GameObject.Find("Inactive Panel").transform);
          //     Debug.Log(GameObject.FindWithTag("CardParent").transform.childCount);
          //   }
          //   cardarrangerReference.numberOfCards = 0;
          //   cardarrangerReference.cards.Clear();
          //   cardarrangerReference.cardInterval.Clear();
        }


      if(clickedEndTurn)
          StartCoroutine(BotTurn());

      if(_playerTurn)
        StartCoroutine(PlayerTurn());
    }

    private void FadePanel()
    {
      // Daca sa inceput valul atunci apare panela
      if(canSelectStartDeck && startDeckPanel.color.a < 0.8f && !destroyUnselectedCards)
        {
          startDeckPanel.enabled = true;
          Color changeAlpha = new Color(0.2f, 0.2f, 0.2f, Mathf.Lerp(startDeckPanel.color.a, alpha, fadeTime * Time.deltaTime));
          startDeckPanel.color = changeAlpha;

          Color newAlpha = new Color(1f, 1f, 1f, Mathf.Lerp(chooseText.color.a, alpha, fadeTime * Time.deltaTime));
          chooseText.color = newAlpha;

          startDeckPanel.raycastTarget = true;
        }
        // Ascunde panela dupa ce apasam butonul Next
        if(destroyUnselectedCards && canSelectStartDeck)
          {
            startDeckPanel.enabled = false;
            Color changeAlpha = new Color(0.2f, 0.2f, 0.2f, Mathf.Lerp(startDeckPanel.color.a, 0, (fadeTime + 5) * Time.deltaTime));
            startDeckPanel.color = changeAlpha;

            Color newAlpha = new Color(1f, 1f, 1f, Mathf.Lerp(chooseText.color.a, 0, (fadeTime + 12) * Time.deltaTime));
            chooseText.color = newAlpha;

            startDeckPanel.raycastTarget = false;
          }
    }
    IEnumerator BotTurn()
    {

      clickedEndTurn = false;
      enemySpawnedCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

      enemyTurnTextAnimator.SetBool("EnemyTurn", true);

      yield return new WaitForSeconds(3f);

      enemyTurnTextAnimator.SetBool("EnemyTurn", false);

      spawnerReference.CardShuffle();

      yield return new WaitForSeconds(1f);
      if(playerPropertiesReference.armor == 0)
      {
        int auxDamageNoArmor = 0;

        for(int i = 0; i < enemyPropertiesReference.Length; i++)
        {
          auxDamageNoArmor -= enemyPropertiesReference[i].GetComponent<EnemyProperties>().enemyDamage;
          // Debug.Log(-enemyPropertiesReference[i].GetComponent<EnemyProperties>().enemyDamage);
        }
        playerPropertiesReference.SetHealth(auxDamageNoArmor);
      }
      else
        if(playerPropertiesReference.armor > 0)
        {
          int auxDamageWithArmor = 0;

          for(int i = 0; i < enemyPropertiesReference.Length; i++)
            auxDamageWithArmor += enemyPropertiesReference[i].GetComponent<EnemyProperties>().enemyDamage;

          if(auxDamageWithArmor > playerPropertiesReference.armor)
            playerPropertiesReference.SetHealth((playerPropertiesReference.armor - auxDamageWithArmor));

            playerPropertiesReference.SetArmor(-auxDamageWithArmor);
          }

      yield return new WaitForSeconds(2f);
        _playerTurn = true;

    }

    IEnumerator PlayerTurn()
    {

      _playerTurn = false;

      playerTurnTextAnimator.SetBool("YourTurn", true);

      yield return new WaitForSeconds(3f);

      StartCoroutine(spawnerReference.CardSpawn());

      playerTurnTextAnimator.SetBool("YourTurn", false);

      if(playerPropertiesReference.manaPerRound < 5)
      playerPropertiesReference.manaPerRound++;
      playerPropertiesReference.mana = playerPropertiesReference.manaPerRound;

      endTurn.interactable = true;
    }

    IEnumerator NewWaveStart()
    {
      endTurn.interactable = false;
      newWave.text = "Wave " + waveNumber.ToString();
      waveAnimator.SetBool("WaveStart", true);
      waveTransitionAnimator.SetBool("WaveTransition", true);

      yield return new WaitForSeconds(3f);

      if(GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        Instantiate(enemyPrefab, Vector3.zero, transform.rotation);

      waveAnimator.SetBool("WaveStart", false);
      waveTransitionAnimator.SetBool("WaveTransition", false);

      yield return new WaitForSeconds(2f);

      canSelectStartDeck = true;
      continueChoose = true;
      waveNumber++;

      yield return new WaitForSeconds(0.5f);
      int i = 0;
      if(i < auxiliar)
      for(i = 0; i < auxiliar; i++)
        spawnerReference.SpawnCardsToSelect();
    }

    public void TaskOnClickEndTurn()
    {
      clickedEndTurn = true;
      endTurn.interactable = false;
    }
    public void TaskOnClickNext()
    {
      destroyUnselectedCards = true;
      clickedNext = true;
      endTurn.interactable = true;
      continueChoose = false;

      auxiliar = Random.Range(2,10);
    }

    public void TaskOnPause()
    {
      if(onClickPause)
        onClickPause = false;
      else
        onClickPause = true;
      pausePanel.SetActive(onClickPause);
    }
}
