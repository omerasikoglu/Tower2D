using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour, IHandleTargeting
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
    public bool isMate;
    private float movementSpeed;
    //private MinionTypeHolder minionTypeHolder;
    private Rigidbody2D rigidbody2d;
    private Transform targetTransform;
   
    private HealthSystem healthSystem;

    

    public void HandleTargeting(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
    }
    private void Start()
    {
        //minionTypeHolder = GetComponent<MinionTypeHolder>();

        rigidbody2d = GetComponent<Rigidbody2D>();
        healthSystem = GetComponent<HealthSystem>();

       

        healthSystem.OnDied += HealthSystem_OnDied;
    }
    private void HealthSystem_OnDied(object sender, System.EventArgs e)
    {
        Destroy(gameObject);
    }
    private void Update()
    {
        HandleMovement();
        
    }
    private void HandleMovement()
    {
        if (targetTransform != null)
        {
            Vector3 moveDir = (targetTransform.position - transform.position).normalized;
            rigidbody2d.velocity = moveDir * movementSpeed;
        }
        else //hedefi ölünce bu minyonun durması
        {
            //rigidbody2d.velocity = Vector2.zero;
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)  //collision --> çarptıgımız objenin collider'ı
    {
        Building building = collision.gameObject.GetComponent<Building>();
        Minion enemyMinion = collision.gameObject.GetComponent<Minion>();
        MinionTypeHolder type = collision.gameObject.GetComponent<MinionTypeHolder>();
        
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
    
}
