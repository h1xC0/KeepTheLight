using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    Vector2 whereToCast;
    public GameObject mainComponent;
    RaycastHit2D hit;
    [SerializeField] private bool left, right;
    EnemyProperties enemyReference;

    void Start()
    {
      GenerateHit();
    }

    // Update is called once per frame
    void Update()
    {
      if(Time.frameCount % 10 == 0)
      CheckHit();
    }

    void GenerateHit()
    {
      if(left)
        whereToCast = this.gameObject.transform.TransformDirection(Vector2.left);
      if(right)
        whereToCast = this.gameObject.transform.TransformDirection(Vector2.right);

      Debug.DrawRay(this.gameObject.transform.position, whereToCast * 5, Color.red);

      hit = Physics2D.Raycast(this.gameObject.transform.position, new Vector2(whereToCast.x * 5, whereToCast.y));
    }

    public void CheckHit()
    {

      if(GameObject.FindGameObjectsWithTag("Enemy").Length < 4)

      if(hit.collider != null)
      if(hit.collider.tag == "Enemy")
      {
        enemyReference = hit.collider.transform.GetComponent<EnemyProperties>();

        if(!enemyReference.protectedEnemy)
        enemyReference.protectedEnemy = true;

        if(!mainComponent.activeSelf)
        {
          enemyReference.protectedEnemy = false;
          this.gameObject.SetActive(false);
        }
      }
    }
}
