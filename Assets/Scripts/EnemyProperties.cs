using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyProperties : MonoBehaviour
{
    public float degreesPerSecond = 15.0f;
    public float amplitude = 0.5f;
    public float frequency = 1f;

    Vector3 posOffset = new Vector3 ();
    Vector3 tempPos = new Vector3 ();

    private bool takeDamage;
    [SerializeField]
    private float speedShake, amountShake;

    [HideInInspector]
    public bool protectedEnemy, protectSelf;

    [Tooltip("Viata inamicului")]
    public int enemyHealth;
    [Header("Damage-ul al Inamicului")]
    public int enemyDamage;
    public string typeOfEnemy;

    public Transform healthBar;
    private Vector3 startingPos;
    private float damagedPosX;
    public TextMeshProUGUI enemyHealthText;

    [SerializeField]
    private float hpBarFollowSpeed, enemyScale;

    public Slider enemyHealthBarSlider;
    private Animator animComponent;
    [HideInInspector]
    private Animator shieldAnimComponent;

    // public MaterialPropertyBlock block;
    // public Vector3 matBlockSensitivity;
    // public SpriteRenderer _spriteRenderer;

    [SerializeField]
    private float sliderSmoothSpeed;

    void Start()
    {
      SetEnemyMaxHealth(enemyHealth);
      animComponent = GetComponent<Animator>();
      // _spriteRenderer = GetComponent<SpriteRenderer>();
      // block = new MaterialPropertyBlock();


      startingPos.x = transform.position.x;
      posOffset = transform.position;

      // enemyHealthBarSlider = transform.GetChild(0).GetComponent<Slider>();
      healthBar = transform.GetChild(0).GetComponent<Transform>();
      enemyHealthText.text = enemyHealth.ToString();

      enemyHealthBarSlider.transform.SetParent(GameObject.Find("OtherUI_Canvas").transform);

      healthBar.localScale = Vector3.one;
      if(typeOfEnemy == "Protector")
      {
        protectSelf = true;
        shieldAnimComponent = this.transform.GetChild(0).GetComponent<Animator>();
      }

    }

    void Update()
    {
      healthBar.position = Vector3.Lerp(healthBar.position, new Vector3(this.transform.position.x, this.transform.position.y + enemyScale), hpBarFollowSpeed * Time.deltaTime);

      if(enemyHealth != enemyHealthBarSlider.value)
      {
        enemyHealthBarSlider.value = Mathf.MoveTowards(enemyHealthBarSlider.value, enemyHealth, sliderSmoothSpeed * Time.deltaTime);
      }
      if(enemyHealth == enemyHealthBarSlider.value)
      {
        takeDamage = false;
      }
      if(Time.frameCount % 5 == 0 && typeOfEnemy == "Damager")
        animComponent.SetBool("Protection", protectedEnemy);
      if(Time.frameCount % 5 == 0)
      {
        animComponent.SetInteger("EnemyHP", (int)enemyHealthBarSlider.value);
        if(shieldAnimComponent != null)
        shieldAnimComponent.SetBool("ProtectSelf?", protectSelf);
      }

      if(protectedEnemy || protectSelf)
        GetComponent<Collider2D>().enabled = false;
      else
          GetComponent<Collider2D>().enabled = true;

          if(Time.frameCount % 10 == 0)
            if(typeOfEnemy == "Protector" && GameObject.FindGameObjectsWithTag("Enemy").Length < 4)
            {
              enemyDamage = 0;
              protectSelf = false;
            }


        damagedPosX = (startingPos.x + Mathf.Sin(Time.time * speedShake) * amountShake);
        //transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);
        // Float up/down with a Sin()
        tempPos = posOffset;
        tempPos.y += Mathf.Sin (Time.fixedTime * Mathf.PI * frequency) * amplitude;

        if(takeDamage)
          transform.position = new Vector3(damagedPosX, tempPos.y);
        else
          transform.position = new Vector3(startingPos.x, tempPos.y);
    }

    public void OnDie()
    {
      if(enemyHealthBarSlider.value == 0)
      {
        enemyHealthBarSlider.gameObject.SetActive(false);
          this.gameObject.SetActive(false);
      }
    }

    public int SetEnemyMaxHealth(int health)
    {
        enemyHealthBarSlider.maxValue = health;
        enemyHealthBarSlider.value = health;
        enemyHealth = health;

        return health;
    }

    public int SetEnemyHealth(int health)
    {
        enemyHealth = (int)Mathf.Clamp(health + enemyHealthBarSlider.value, 0, enemyHealthBarSlider.maxValue);
        enemyHealthText.text = enemyHealth.ToString();
        takeDamage = true;
        return health;
    }

    public void EnemyBehaviour()
    {
    }
}
