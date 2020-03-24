using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public CardData card;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI powerText;

    public Image skill;
    private Image cardImage;

    public int cost;

    [SerializeField]
    private Color cardRarity;
    [SerializeField]
    private RectTransform skillPos;

    public string powerType;


    void Start()
    {
      DisplayProperties();
    }


    void Update()
    {

    }

    public void DisplayProperties()
    {
      cardImage = GetComponent<Image>();

      nameText.text = card.CardName;
      descriptionText.text = card.CardDescription;
      powerText.text = card.CardPower.ToString();
      skill.sprite = card.CardSkill;
      skill.preserveAspect = true;
      cost = card.CardCost;
      powerType = card.CardPowerType;

      switch (cost)
      {
          case 1:
              cardRarity = new Color32(192, 192, 192, 255);
              break;
          case 2:
              cardRarity = new Color32(191, 255, 127, 255);
              break;
          case 3:
              cardRarity = new Color32(163, 48, 201, 255);
              break;
          case 4:
              cardRarity = new Color32(255, 125, 10, 255);
              break;
          case 5:
              cardRarity = new Color32(196, 31, 59, 255);
              break;
          default:
              cardRarity = Color.yellow;
              break;
      }
      cardImage.color = cardRarity;
      skillPos.transform.localScale = card.CardSkillPos;
    }
}
