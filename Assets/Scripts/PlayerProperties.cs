using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerProperties : MonoBehaviour
{
    [Tooltip("Viata jucatorului")]
    public int health;

    [Tooltip("Armura jucatorului")]
    public int armor;


    [Tooltip("Mana jucatorului")]
    public int mana;


    [Tooltip("Mana in acest round")]
      public int manaPerRound;


    [Tooltip("Textul cu Mana")]
    [SerializeField] private TextMeshProUGUI manaText;
    public TextMeshProUGUI ManaText
    {
        get { return manaText; }
        protected set { }
    }

    [Tooltip("Textul cu Viata")]
    [SerializeField] private TextMeshProUGUI healthText;
    public TextMeshProUGUI HealthText
    {
        get { return healthText; }
        protected set { }
    }

    [Tooltip("Textul cu Armura")]
    [SerializeField] private TextMeshProUGUI armorText;
    public TextMeshProUGUI ArmorText
    {
        get { return armorText; }
        protected set { }
    }

    public Slider healthBarSlider;
    public Slider armorBarSlider;

    private Animator healthBarAnimator;
    private Animator armorBarAnimator;

    public Animator anim;

    [SerializeField]
    private float changeSpeed;

    private float damagedPosX;
    [SerializeField][Header("Pentru a zgudui camera")]
    private float speedShake, amountShake;
    private Vector3 startingPos;
    private bool takeDamage;

    private CardArranger caRef;

    private bool timerStart;
    private float timer;

    void Start()
    {
      SetMaxHealth(100);
      SetMaxArmor(100);

      startingPos.x = Camera.main.transform.position.x;

      anim = healthBarSlider.transform.GetChild(2).GetComponent<Animator>();
      healthBarAnimator = healthBarSlider.transform.parent.gameObject.GetComponent<Animator>();
      armorBarAnimator = armorBarSlider.transform.parent.gameObject.GetComponent<Animator>();

      caRef = GameObject.FindWithTag("CardParent").GetComponent<CardArranger>();
    }

    // Update is called once per frame
    void Update()
    {
      if(Time.frameCount %  changeSpeed / 50 == 0)
      {
        ManaText.text = mana + "/" + manaPerRound;
        HealthText.text = health.ToString();
        ArmorText.text = armor.ToString();
        anim.SetInteger("HP", (int)healthBarSlider.value);
      }
        if(armorBarSlider.value != armor)
        armorBarSlider.value = Mathf.MoveTowards(armorBarSlider.value, armor, changeSpeed * Time.deltaTime);

        if(healthBarSlider.value != health)
          healthBarSlider.value = Mathf.MoveTowards(healthBarSlider.value, health, changeSpeed * Time.deltaTime);
        else
            {
              takeDamage = false;
            }

        damagedPosX = (startingPos.x + Mathf.Sin(Time.time * speedShake) * amountShake);

        if(takeDamage)
          Camera.main.transform.position = new Vector3(damagedPosX, 0, -10);
        else
          Camera.main.transform.position = new Vector3(startingPos.x, 0, -10);

          if(caRef.needAlign == false)
          {
            healthBarAnimator.SetBool("HB_Active", true);
            armorBarAnimator.SetBool("AB_Active", true);
          }
          else
            if(timerStart && timer < 12f)
              timer += Time.deltaTime;
              else if(timer > 12f)
              {
                armorBarAnimator.SetBool("AB_Active", false);
                healthBarAnimator.SetBool("HB_Active", false);
                timer = 0f;
                timerStart = false;
              }

    }

    public int SetMaxHealth(int _health)
    {
        healthBarSlider.maxValue = _health;
        healthBarSlider.value = _health;
        health = _health;
        return _health;
    }

    public int SetHealth(int _health)
    {
        //healthBarSlider.value += _health;
        health = (int)Mathf.Clamp(healthBarSlider.value + _health, 0, healthBarSlider.maxValue);
        if(_health < 0)
          takeDamage = true;
        timerStart = true;
        healthBarAnimator.SetBool("HB_Active", true);
        return _health;
    }

    public int SetMaxArmor(int _armor)
    {
        armorBarSlider.maxValue = _armor;
        return _armor;
    }

    public int SetArmor(int _armor)
    {
        //armorBarSlider.value += _armor;//Mathf.Lerp(armorBarSlider.value, armorBarSlider.value + _armor, changeSpeed * Time.deltaTime);
        armor = (int)Mathf.Clamp(armorBarSlider.value + _armor, 0, armorBarSlider.maxValue);
        timerStart = true;
        armorBarAnimator.SetBool("AB_Active", true);
        return _armor;

    }
}
