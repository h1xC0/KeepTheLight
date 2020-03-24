using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.UI;

public class TouchManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private Vector3 addPos;

    private Vector3 startCardPos;

    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 camOffset = new Vector3(0, 0, 10);
    private LineRenderer dragLine;

    [SerializeField]
    private AnimationCurve lineCurve;
    [SerializeField]
    private Gradient lineGradient;

    public Transform DefaultParentTransform;
    public Transform DragParentTransform;

    public bool canDissolve;
    public bool overHP = true;
    private bool veryCloseToDeck;

    private Vector3 startRot;

    [SerializeField]
    private float lerpTime;
    private float aux = 0.8f;

    public CardArranger ca;
    public PlayerProperties pp;
    public CardDisplay cd;
    private EnemyProperties ep;
    private GameLogic gl;
    private Spawner sp;

    private bool beginDragging;

    void Start()
    {
        DefaultParentTransform = GameObject.FindWithTag("CardParent").GetComponent<Transform>();
        DragParentTransform = GameObject.FindWithTag("CardParent").GetComponent<Transform>();

        ca = GameObject.FindWithTag("CardParent").GetComponent<CardArranger>();
        sp = GameObject.Find("ObjectPool").GetComponent<Spawner>();
        cd = this.GetComponent<CardDisplay>();
        pp = GameObject.FindWithTag("Player").GetComponent<PlayerProperties>();
        gl = GameObject.Find("GameLogic").GetComponent<GameLogic>();

        this.transform.SetParent(GameObject.FindWithTag("CardParent").transform);
        this.transform.localScale = new Vector3(0, 0, 1.5f);
    }

    void Update()
    {

        if(transform.position.y <= startCardPos.y)
            veryCloseToDeck = false;

        if(veryCloseToDeck)
        {
            transform.position = Vector3.MoveTowards(transform.position, startCardPos, lerpTime * Time.deltaTime);
            transform.eulerAngles = startRot;
        }
        if(ep == null)
        {
          ep = GameObject.FindWithTag("Enemy").GetComponent<EnemyProperties>();
        }
        scaleChanger();

    }

    void LineGradientColor()
    {
      float alpha = 1.0f;
      lineGradient.SetKeys(
      new GradientColorKey[]{
        new GradientColorKey(new Color32(255, 0, 14, 255), 0.0f),
        new GradientColorKey(new Color32(255, 157, 0, 255), 0.5f),
        new GradientColorKey(new Color32(231, 255, 6, 255), 1.0f)},
      new GradientAlphaKey[]{
        new GradientAlphaKey(alpha, 0.0f),
        new GradientAlphaKey(alpha, 0.5f),
        new GradientAlphaKey(alpha, 1.0f)}
      );

      dragLine.material = new Material(Shader.Find("Sprites/Default"));
      dragLine.colorGradient = lineGradient;
    }

    public void scaleChanger()
    {
      aux = Vector3.Distance(transform.position, GameObject.FindWithTag("CardParent").transform.position);

      if(transform.localScale.x < 1.5f)
        transform.localScale =
        Vector3.MoveTowards(transform.localScale, Vector3.ClampMagnitude((Vector3.Normalize(new Vector3(aux, aux, 1.5f))+ Vector3.one),10), 1 * Time.deltaTime);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
      if(transform.localScale.x <= 1.6f && transform.localScale.x > 1.5f && gl.endTurn.interactable == true && sp.standardCardsDealed)
        {
          transform.SetParent(DragParentTransform);

          startCardPos = transform.position;
          startRot = transform.eulerAngles;

          transform.eulerAngles = Vector3.zero;

          ca.needCardRotate = false;
          ca.needAlign = false;
        //if (Input.touchCount > 0 && Input.touchCount < 2) // Work only on mobile
          if(cd.powerType == "Attack")
          {
            transform.position = new Vector3(transform.position.x, startCardPos.y + 2f);



              Debug.Log(transform.position);
          if(dragLine == null)
            dragLine = gameObject.AddComponent<LineRenderer>();

          dragLine.enabled = true;
          dragLine.positionCount = 2;

          startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + camOffset;

          LineGradientColor();
          dragLine.SetPosition(0, startPos);
          dragLine.useWorldSpace = true;
          dragLine.widthCurve = lineCurve;
          dragLine.numCapVertices = 10;
          dragLine.sortingOrder = 4;
          }

        }

    }
    public void OnDrag(PointerEventData eventData)
    {
       if(transform.localScale.x <= 1.6f && transform.localScale.x > 1.5f && !ca.needAlign && gl.endTurn.interactable == true && sp.standardCardsDealed)
       {
         ca.needCardRotate = false;
         ca.needAlign = false;

         if(cd.powerType == "Heal" || cd.powerType == "Defense")
         {

           if(dragLine != null)
           dragLine.enabled = false;

           transform.position = Camera.main.ScreenToWorldPoint(new Vector3
              (Input.mousePosition.x, Input.mousePosition.y, 1));
         }
         // else
         //    {
         //      transform.SetParent(DefaultParentTransform);
         //      this.transform.position = new Vector3(transform.position.x ,startCardPos.y);
         //
         //      veryCloseToDeck = true;
         //      //ca.needAlign = true;
         //      ca.needCardRotate = true;
         //    }
          }

       RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.position);
       if(transform.localScale.x <= 1.6f && transform.localScale.x > 1.5f && gl.endTurn.interactable == true && sp.standardCardsDealed)
       {
         if(hit.collider != null && cd.powerType == "Attack")
         {
          if(hit.collider.tag == "Enemy")
          {
            ep.GetComponent<SpriteRenderer>().material.SetFloat("_OutlineThickness", 0);
            ep = hit.collider.GetComponent<EnemyProperties>();
            // if(!ep.protectedEnemy)
            ep.GetComponent<SpriteRenderer>().material.SetFloat("_OutlineThickness", 1);
          }
       }
       if(hit.collider == null && ep != null)
       {
         ep.GetComponent<SpriteRenderer>().material.SetFloat("_OutlineThickness", 0);
       }
       }

       if(dragLine !=null)
       {
         endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + camOffset;
         dragLine.SetPosition(1, endPos);
       }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      int aux = 0;
      veryCloseToDeck = false;

      if(dragLine != null)
      dragLine.enabled = false;

      ca.needAlign = true;
      ca.needCardRotate = true;

      if(pp.healthBarSlider.maxValue < pp.healthBarSlider.value + int.Parse(cd.powerText.text) && cd.powerType == "Heal")
      {
        overHP = true;
      }
      else
          overHP = false;

      RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.position);

      if(ep != null)
      {
        ep.GetComponent<SpriteRenderer>().material.SetFloat("_OutlineThickness", 0);
      }

      //Ray2D ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if(transform.localScale.x <= 1.6f && transform.localScale.x > 1.5f && gl.endTurn.interactable == true && sp.standardCardsDealed)
        if(hit.collider != null && !overHP && pp.mana >= cd.cost) //|| hit.collider == null && !overHP && pp.mana >= cd.cost && cd.powerType != "Attack")
        {
          ep = hit.collider.GetComponent<EnemyProperties>();
          // if(!ep.protectedEnemy)
            switch(hit.collider.tag)
            {
              case "Enemy":
              {
                    switch(cd.powerType)
                    {
                      case "Attack":
                      {
                        canDissolve = true;
                        //ca.needDistVerify = false;
                        pp.mana -= cd.cost;
                        ep.SetEnemyHealth(-int.Parse(cd.powerText.text)); break;
                      }

                      case "Heal":
                      {
                        canDissolve = true;
                        //ca.needDistVerify = false;
                        pp.mana -= cd.cost;
                        ep.SetEnemyHealth(int.Parse(cd.powerText.text)); break;
                      }
                    }
                  }
                break;
              case "ArmorBar":
              {
                canDissolve = true;
                //ca.needDistVerify = false;
                pp.mana -= cd.cost;

                if(cd.powerType == "Defense")
                  pp.SetArmor(int.Parse(cd.powerText.text));
                if(cd.powerType == "Heal")
                  pp.SetHealth(int.Parse(cd.powerText.text));
              }
              break;
              case "HealthBar":
              {
                canDissolve = true;
                //ca.needDistVerify = false;

                pp.mana -= cd.cost;

                if(cd.powerType == "Heal")
                  pp.SetHealth(int.Parse(cd.powerText.text));
                if(cd.powerType == "Defense")
                  pp.SetArmor(int.Parse(cd.powerText.text));
              }
              break;
            }
          }
          else
            if (/*this.transform.position.y < startCardPos.y + addPos.y ||  this.transform.position.y < startCardPos.y - addPos.y  ||*/
             pp.mana < cd.cost || overHP)
            {
                transform.SetParent(DefaultParentTransform);
                this.transform.position = new Vector3(transform.position.x ,startCardPos.y);

                veryCloseToDeck = true;
                ca.needAlign = true;
                ca.needCardRotate = true;
            }
        // Debug.Log(pp.healthBarSlider.value + int.Parse(cd.powerText.text));
    }
}
