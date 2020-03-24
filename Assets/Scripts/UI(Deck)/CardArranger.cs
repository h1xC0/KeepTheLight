using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class CardArranger : MonoBehaviour
{
    public List<GameObject> cards = new List<GameObject>();
    [SerializeField]
    public List<float> cardInterval = new List<float>();
    private List<float> dist = new List<float>();

    public int numberOfCards;
    private int i = 0;
    private int half;


    [SerializeField]
    private float alignRotScale;

    public bool needCalcXPos, needAlign, needCardRotate, needDistVerify;

    [SerializeField]
    private float dealSpeed, animationTime;
    [SerializeField]
    private float intervalScale;
    public float intervalScaleIterate, startIterate;

    private float startIntervalScale;
    private float startDist;
    //startIntervalScaleInverted;
    //private float intervalScaleInverted;


    void Start()
    {
        numberOfCards = this.transform.childCount;

        startIntervalScale = intervalScale;
        //startIntervalScaleInverted = intervalScaleInverted;
        startIterate = intervalScaleIterate;
        if(cards != null)
        {
        CalculateXPos();
        //needCalcXPos = true;
        }


        //needDistVerify = true;
        needAlign = true;
        needCardRotate = true;
        needCalcXPos = true;
    }

    void Update()
    {


      if(transform.childCount != null)
      {
        numberOfCards = cards.Count;
        for(i = 0; i < numberOfCards; i++)
        {

            if(transform.GetChild(i).gameObject != null)
                cards[i] = transform.GetChild(i).gameObject;

        }
      }



        if(needAlign)
        {

       // if(cards[numberOfCards].transform.position.x != cardInterval[numberOfCards])
        //if(cards[i] != null)
        // float aux = Vector3.Distance(this.transform.position, cards[i].transform.position)/100f;
        // Debug.Log(aux);
            StartCoroutine(SmoothAlign());
        }

          if(needCalcXPos)
          {
              intervalScale = startIntervalScale;
              //intervalScaleInverted = startIntervalScaleInverted;
              cardInterval.Clear();
              CalculateXPos();
          }

        if(needCardRotate)
        {
            CardRotate();
        }
        if(needDistVerify && !needAlign)
        {
            //dist.Clear();
            Distance();
            //needAlign = true;
        }
    }

    //Calculam Pozitia pe X
    public void CalculateXPos()
    {
      //  if(cards == null)
          //  return;

        int i = 0;

        if(numberOfCards > 1)
            intervalScale = (this.transform.position.x + (numberOfCards / 0.8f));
        else
            intervalScale = (this.transform.position.x);

        cardInterval.Insert(0, intervalScale);

        for(i = 1; i < numberOfCards; i++)
        {
            intervalScale -= (intervalScaleIterate / 4f);
            cardInterval.Insert(i, intervalScale);
        }
    }

    public void CardRotate()
    {
        float auxScalePerCard = alignRotScale;
        float alignRotPerCard;


        foreach(GameObject card in cards)
        {
            alignRotPerCard = ((this.transform.position.x - card.transform.position.x) / alignRotScale);
            auxScalePerCard = alignRotPerCard;
            card.transform.eulerAngles = new Vector3(0, 0, auxScalePerCard);


        }
        //needCardRotate = false;
    }
    public void Distance()
    {
        //i = 0;
        needAlign = false;
        float newDist;
        foreach(GameObject card in cards)
        {
            newDist = Math.Abs((transform.parent.gameObject.transform.position.x - card.transform.position.x) / 20f);
            Debug.Log(newDist);
            card.transform.position = new Vector3(card.transform.position.x, card.transform.position.y - newDist);
        }
        // float startDistance;
        // for(i=1;i < numberOfCards;i++)
        // {
        //         dist.Insert(i,
        //         (Mathf.Abs(transform.parent.gameObject.transform.position.x - cards[i].transform.position.x))/6f);

        // if(i < numberOfCards)
        //     {
        //         //needDistVerify = false;
        //     }
        // }
    }

    IEnumerator SmoothAlign()
    {
       WaitForSeconds wait = new WaitForSeconds(1.5f);
       float lerp = 0;

      for(i = 0; i < numberOfCards; i++)
          {
            //lerp+= Time.deltaTime / animationTime;
            cards[i].transform.position =
            Vector3.Lerp(cards[i].transform.position,
            new Vector3(this.transform.position.x - cardInterval[i], this.transform.position.y),
            dealSpeed * Time.deltaTime);

          yield return null;
          // Debug.Log("Card Placed");
        }

    }
}
