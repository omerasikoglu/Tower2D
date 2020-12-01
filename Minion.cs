using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour
{
    public static Minion Create(MinionTypeSO minionType, bool isMate, Vector3 position, float movementSpeed = 5f)
    {
        Transform pfMinion;
        bool stronger = Random.Range(0, 100) >= 80;
        if (stronger)
        {
            pfMinion = Resources.Load<Transform>("Minions/pfStronger" + minionType.nameString);
        }
        else
        {
            pfMinion = Resources.Load<Transform>("Minions/pf" + minionType.nameString);
        }

        Transform minionTransform = Instantiate(pfMinion, position, Quaternion.identity);
        Minion minion = minionTransform.GetComponent<Minion>();
        minion.isMate = isMate;
        minion.movementSpeed = movementSpeed;
        return minion;
    }

    [SerializeField] private int damageAmount;
    [SerializeField] private bool haveMinionTargeting = true;

    private bool isMate = true;
    private float movementSpeed;

    private Rigidbody2D rigidbody2d;
    private Transform targetTransform;
    private Transform targetMinionTransform;
    private HealthSystem healthSystem;

    private float lookForTargetTimer;
    private float lookForTargetTimerMax = .2f;

    private void Start()
    {
        if (isMate)
        {
            if (BuildingManager.Instance.GetEnemyHQBuilding() != null) targetTransform = BuildingManager.Instance.GetEnemyHQBuilding().transform;

            else Destroy(gameObject);
        }
        else
        {
            if (BuildingManager.Instance.GetYourHQBuilding() != null) targetTransform = BuildingManager.Instance.GetYourHQBuilding().transform;

            else Destroy(gameObject);
        }

        rigidbody2d = GetComponent<Rigidbody2D>();
        healthSystem = GetComponent<HealthSystem>();

        lookForTargetTimer = Random.Range(0f, lookForTargetTimerMax);

        healthSystem.OnDied += HealthSystem_OnDied;
    }
    private void HealthSystem_OnDied(object sender, System.EventArgs e)
    {
        Destroy(gameObject);
    }
    private void Update()
    {
        //if (targetBuildingTransform == null) Destroy(gameObject);
        HandleMovement();
        HandleTargeting();
    }
    private void HandleMovement()
    {
        if (targetTransform != null)
        {
            Vector3 moveDir = (targetTransform.position - transform.position).normalized;
            rigidbody2d.velocity = moveDir * movementSpeed;
        }
        else
        {
            rigidbody2d.velocity = Vector2.zero;
        }
    }
    private void HandleTargeting()
    {

        lookForTargetTimer -= Time.deltaTime;
        if (lookForTargetTimer < 0f)
        {
            lookForTargetTimer += lookForTargetTimerMax;
            LookForMinions();

        }
    }
    private void OnCollisionEnter2D(Collision2D collision)  //collision -->çarptıgımız objenin collider'ı
    {
        Building building = collision.gameObject.GetComponent<Building>();
        Minion enemyMinion = collision.gameObject.GetComponent<Minion>();
        if (building != null)
        {
            HealthSystem healthSystem = building.GetComponent<HealthSystem>();
            healthSystem.Damage(damageAmount);
            Destroy(gameObject);
        }
        if (enemyMinion != null)
        {
            HealthSystem healthSystem = enemyMinion.GetComponent<HealthSystem>();
            healthSystem.Damage(damageAmount);
        }
    }
    private void LookForMinions()
    {
        float targetMaxRadius = 10f;
        Collider2D[] collider2DArray = Physics2D.OverlapCircleAll(transform.position, targetMaxRadius);

        foreach (Collider2D collider2D in collider2DArray)
        {
            Minion otherMinion = collider2D.GetComponent<Minion>();
            if (otherMinion != null && otherMinion.isMate != isMate)
            {
                if (targetMinionTransform == null)
                {
                    targetMinionTransform = otherMinion.transform;
                }
                else
                {   //hedef minyon varsa ama daha yakın minyon da varsa
                    if (Vector3.Distance(transform.position, otherMinion.transform.position) <
                        Vector3.Distance(transform.position, targetMinionTransform.position))
                    {
                        targetMinionTransform = otherMinion.transform;
                    }
                }
            }
        }
        if (targetMinionTransform == null) LookForBuildings();
        else targetTransform = targetMinionTransform;
    }
    private void LookForBuildings()
    {
        float targetMaxRadius = 10f;
        Collider2D[] collider2DArray = Physics2D.OverlapCircleAll(transform.position, targetMaxRadius);

        foreach (Collider2D collider2D in collider2DArray)
        {
            Building building = collider2D.GetComponent<Building>();    //etrafında düşman bina varsa 

            if (building != null && building.isFriendlyBuilding != isMate)
            {
                if (targetTransform == null)    //biz henüz biyere hedef almadıysak
                {
                    targetTransform = building.transform;
                }
                else
                {   //hedef bina varsa ama daha yakın bina da varsa
                    if (Vector3.Distance(transform.position, building.transform.position) <
                        Vector3.Distance(transform.position, targetTransform.position))
                    {
                        targetTransform = building.transform;
                    }
                }
            }


        }
        if (targetTransform == null)    //hareket halindeyken etrafta bina bulamadıysa
        {
            Destroy(gameObject);
            //if (isMate) targetBuildingTransform = BuildingManager.Instance.GetEnemyHQBuilding().transform;
            //else targetBuildingTransform = BuildingManager.Instance.GetYourHQBuilding().transform;
        }

    }

    
}
