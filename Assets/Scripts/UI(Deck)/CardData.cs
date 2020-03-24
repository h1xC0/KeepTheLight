using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CreateAssetMenu(menuName = "Cards/Standard Card", fileName = "New Card")]
public class CardData : ScriptableObject
{
    [Tooltip("Art-ul cartii")]
    [SerializeField] private Sprite cardSkill;
    public Sprite CardSkill
    {
        get { return cardSkill;  }
        protected set { }
    }

    [Tooltip("Pozitia art-ului cartii")]
    [SerializeField] private Vector3 cardSkillPos;
    public Vector3 CardSkillPos
    {
        get { return cardSkillPos; }
        protected set { }
    }

    [Tooltip("Raritatea cartii")]
    [SerializeField] private Color cardColor;
    public Color CardColor
    {
        get { return cardColor; }
        protected set { }
    }

    [Tooltip("Power attack")]
    [SerializeField] private int cardPower;
    public int CardPower
    {
        get { return cardPower; }
        protected set { }
    }

    [Tooltip("Costul")]
    [SerializeField] private int cardCost;
    public int CardCost
    {
        get { return cardCost; }
        protected set { }
    }

    [Tooltip("Textul cu denumirea cartii")]
    [SerializeField] private string cardName;
    public string CardName
    {
        get { return cardName; }
        protected set { }
    }

    [Tooltip("Textul cu descrierea cartii")]
    [SerializeField] private string cardDescription;
    public string CardDescription
    {
        get { return cardDescription; }
        protected set { }
    }

    [Tooltip("Tipul putereii Attack/Defense/Heal etc.")]
    [SerializeField] private string  cardPowerType;
    public string CardPowerType
    {
        get { return cardPowerType; }
        protected set { }
    }

}