using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public CardArranger cardArrangerReference;
    public CardDisplay cardDisplayReference;
    public TouchManager touchManagerReference;
    public CardData[] cardDataReference;
    public GameLogic gamelogicReference;
    public static Spawner spawnerInstance;

    [SerializeField]
    private Transform spawnPosition, spawnRotation;
    [SerializeField]
    private GameObject prefabToSpawn;
    private List<int> initialDeckOfCards = new List<int>();
    private List<int> currentDeckOfCards = new List<int>();


    public bool standardCardsDealed = false;
    private float alpha = 1.5f;
    private int aux;

    void Start()
    {
      gamelogicReference = GameObject.Find("GameLogic").GetComponent<GameLogic>();

      initialDeckOfCards.Add(0);
      initialDeckOfCards.Add(0);
      initialDeckOfCards.Add(0);
      initialDeckOfCards.Add(1);
      initialDeckOfCards.Add(1);
      initialDeckOfCards.Add(2);
      initialDeckOfCards.Add(3);
      initialDeckOfCards.Add(3);
      initialDeckOfCards.Add(0);
      initialDeckOfCards.Add(4);

      /*for(int i = 0; i < 5; i++)
      {
          if(Random.value > 0.0) initialDeckOfCards.Add(Random.Range(0, 4));
            else if(Random.value > 0.2) initialDeckOfCards.Add(0);
            else if(Random.value > 0.4) initialDeckOfCards.Add(Random.Range(1, 2));
            else if(Random.value > 0.6) initialDeckOfCards.Add(Random.Range(3, 4));
          // else if(Random.value > 0.6f) initialDeckOfCards.Add(3);
          // else if(Random.value > 0.8f) initialDeckOfCards.Add(4);

        Debug.Log(initialDeckOfCards[i]);
      }*/
      currentDeckOfCards = initialDeckOfCards;

      if(spawnerInstance == null)
        spawnerInstance = this;
      else
      if(spawnerInstance != null && gamelogicReference == null)
        Destroy(gameObject);
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CardShuffle()
    {
      /*
      Simple Strike = 1 / nr de carti
      Fireball
      ThunderStrike
      Paladi's Call
      Anti-Milk Shield
      */
      int i = 0, randomNumber = 0, aux = 0;

      for(i = 0; i < initialDeckOfCards.Count - 2; i++)
      {
        randomNumber = Random.Range(i, initialDeckOfCards.Count - 1);

        aux = initialDeckOfCards[i];
        initialDeckOfCards[i] = initialDeckOfCards[randomNumber];
        initialDeckOfCards[randomNumber] = aux;
      }

      currentDeckOfCards = initialDeckOfCards;
    }

    public IEnumerator CardSpawn()
    {
      int i = 0;
      List<int> tempList = new List<int>();
      for(i = 0; i < initialDeckOfCards.Count; i++)
        tempList.Add(initialDeckOfCards[i]);
      // for(i = 0; i < 4; i++)
      // {
      //   quarter.Insert(i, Random.Range(0, initialDeckOfCards.Count));
      // }

      for(i = 0; i < 4; i++)
      {
        yield return new WaitForSeconds(0.1f);
        int index = Random.Range(0, tempList.Count - 1);
        CalculateCardDisplay(tempList[index]);
        tempList.Remove(tempList[index]);
        yield return new WaitForSeconds(1f);

        if(i == 3)
          standardCardsDealed = true;
        else
          if(i != 3)
            standardCardsDealed = false;
      }
    }

    public int CalculateCardDisplay(int skillCardType)
    {
      GameObject pooledCard = ObjectPool.SharedInstance.GetPooledObject("Draggable");
      if (pooledCard != null)
      { 
          cardArrangerReference.cards.Add(pooledCard);
          cardDisplayReference = pooledCard.GetComponent<CardDisplay>();
          touchManagerReference = pooledCard.GetComponent<TouchManager>();

          pooledCard.transform.position = spawnPosition.position;
          pooledCard.transform.rotation = spawnRotation.rotation;
          touchManagerReference.canDissolve = false;
          pooledCard.transform.SetParent(GameObject.FindWithTag("CardParent").transform);



          cardDisplayReference.card = cardDataReference[skillCardType];//cardDataReference[Random.Range(0, cardDataReference.Length)];
          cardDisplayReference.DisplayProperties();

          for(int i = 0; i < pooledCard.transform.childCount; i++)
          {
            pooledCard.transform.GetChild(i).gameObject.SetActive(true);
          }
          pooledCard.SetActive(true);
          pooledCard.transform.SetSiblingIndex(GameObject.FindWithTag("CardParent").transform.childCount);

      }
      return skillCardType;
    }

    public void SpawnCardsToSelect()
    {
      CardDisplay selectCard = prefabToSpawn.GetComponent<CardDisplay>();
      selectCard.card = cardDataReference[Random.Range(0,5)];
      Instantiate(prefabToSpawn, transform.position, transform.rotation, GameObject.Find("Select Cards").transform);
    }
}
