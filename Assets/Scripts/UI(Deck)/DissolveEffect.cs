using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;

public class DissolveEffect : MonoBehaviour, IPointerDownHandler,IEndDragHandler
{
    private TouchManager tm;

    private Material material;
    //[SerializeField] private Renderer _renderer;
   //[SerializeField] private MaterialPropertyBlock _propBlock;
    private Texture2D mainTex;

    private float dissolveAmount;
    private float amount;
    private bool isDissolving;

    [SerializeField]
    private bool face, back;

    [SerializeField][Range(0,10)][Tooltip("Waiting time to complete shader")]
    private float counter;

    public GameObject selectCard;

    private CardArranger ca;
    public GameLogic gamelogicReference;
    private Spawner spawnReference;

    void Awake()
    {
        //_propBlock = new MaterialPropertyBlock();
        // _renderer = gameObject.GetComponent<CanvasRenderer>().;
    }

    private void Start()
    {
        mainTex = this.GetComponent<Image>().sprite.texture;

        // De fiecare data cream un nou material pentru a controla proprietatile materialelor asincronizat
        Material mat = Instantiate(this.GetComponent<Image>().material);
        this.GetComponent<Image>().material = mat;
        material = this.GetComponent<Image>().material;

        if(face)
          tm = GetComponent<TouchManager>();

        dissolveAmount = 0;
        amount = 0;
        isDissolving = false;
        //material.SetFloat("_DissolveAmount", amount);

        if(face)
          ca = GameObject.FindWithTag("CardParent").GetComponent<CardArranger>();

        gamelogicReference = GameObject.Find("GameLogic").GetComponent<GameLogic>();
        spawnReference = GameObject.Find("ObjectPool").GetComponent<Spawner>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        if (!isDissolving && gameObject.tag == "UnSelected" && gamelogicReference.continueChoose)
        {
          this.gameObject.tag = "Draggable";
          Instantiate(selectCard, new Vector3(transform.position.x, transform.position.y + 4f) , this.transform.rotation, this.transform);
        }
        else
          if (!isDissolving && gameObject.tag == "Draggable" && gamelogicReference.endTurn.interactable == false)
          {
            this.gameObject.tag = "UnSelected";
            if(transform.childCount == 5)
              Destroy(this.transform.GetChild(4).gameObject);
          }

       //else
            //isDissolving = false;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDissolving && tm.canDissolve)
            isDissolving = true;
    }

    void Update()
    {

       // _renderer.GetPropertyBlock(_propBlock);

       if(!isDissolving && gamelogicReference.destroyUnselectedCards && gameObject.tag == "UnSelected")
       {
         amount = Mathf.Clamp01(amount + Time.deltaTime);
         this.material.SetFloat("_DissolveAmount", amount);

         StartCoroutine(DissolveUnselected());
       }
       if(!isDissolving && gamelogicReference.destroyUnselectedCards && gameObject.tag == "Draggable")
       {
          StartCoroutine(SelectedCardsArrange());
       }

        if (isDissolving && face || gamelogicReference.dissolveForNextRound)
        {
            dissolveAmount = Mathf.Clamp01(dissolveAmount + Time.deltaTime);
            this.material.SetFloat("_DissolveAmount", dissolveAmount);
            StartCoroutine(DissolveTime());
        }
         else
         if(!isDissolving && dissolveAmount >= 0.1f && !gamelogicReference.dissolveForNextRound)
         {
             dissolveAmount = Mathf.Clamp01(dissolveAmount - Time.deltaTime);
             this.material.SetFloat("_DissolveAmount", dissolveAmount);
         }


       // _renderer.SetPropertyBlock(_propBlock);
    }


    IEnumerator DissolveTime()
    {
        ca.needAlign = false;
        ca.needCardRotate = false;
        yield return new WaitForSeconds(counter-0.5f);

        for (int i= 0;i < this.transform.childCount;i++)
            this.transform.GetChild(i).gameObject.SetActive(false);

        yield return new WaitForSeconds(counter-0.7f);

        this.gameObject.SetActive(false);
        this.transform.SetParent(GameObject.FindWithTag("NewParent").transform);
        ca.cards.Remove(this.gameObject);

        isDissolving = false;

        ca.needAlign = true;
        ca.needCardRotate = true;
        ca.needCalcXPos = true;
        gamelogicReference.dissolveForNextRound = false;
    }
    IEnumerator DissolveUnselected()
    {
      yield return new WaitForSeconds(counter-0.5f);

      for (int i= 0;i < this.transform.childCount;i++)
          this.transform.GetChild(i).gameObject.SetActive(false);

      yield return new WaitForSeconds(counter-0.7f);

      gamelogicReference.destroyUnselectedCards = false;
      gamelogicReference.canSelectStartDeck = false;

      Destroy(this.gameObject);
    }
    IEnumerator SelectedCardsArrange()
    {
      ca.cards.Clear();
      if(transform.childCount == 5)
      Destroy(this.transform.GetChild(4).gameObject);
      yield return new WaitForSeconds(1.2f);

      ca.cards = GameObject.FindGameObjectsWithTag("Draggable").ToList();
        foreach(GameObject card in ca.cards)
        {
          card.transform.SetParent(GameObject.FindWithTag("CardParent").transform);
          tm.enabled = true;
        }


    }
}
