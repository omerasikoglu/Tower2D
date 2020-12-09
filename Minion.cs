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
            minionType = pfMinion.GetComponent<Minion>().minionType;
        }
        else
        {
            pfMinion = Resources.Load<Transform>("Minions/pf" + minionType.nameString);
        }
        Transform minionTransform = Instantiate(pfMinion, position, Quaternion.identity);
        Minion minion = minionTransform.GetComponent<Minion>();
        minion.isMate = isMate;
        minion.movementSpeed = movementSpeed;
        minion.minionType = minionType;
        return minion;
    }


    bool paper = false;
    bool rock = false;
    bool scissors = false;
    bool strongerPaper = false;
    bool strongerRock = false;
    bool strongerScissors = false;

    private const int damageAmount = 2;
    private const int mdamageAmount = 4;

    public bool isMate;
    public MinionTypeSO minionType;

    public float movementSpeed;
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


        if (minionType.nameString == "Paper") paper = true;
        if (minionType.nameString == "Rock") rock = true;
        if (minionType.nameString == "Scissors") scissors = true;
        if (minionType.nameString == "StrongerPaper") strongerPaper = true;
        if (minionType.nameString == "StrongerRock") strongerRock = true;
        if (minionType.nameString == "StrongerScissors") strongerScissors = true;



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
            Vector3 movDire = new Vector3(0, 0, 0);

            if (isMate)
            {
                if (BuildingManager.Instance.GetEnemyHQBuilding() != null)
                {
                    movDire = ((BuildingManager.Instance.GetEnemyHQBuilding().transform.position) - transform.position).normalized;
                }

                else Destroy(gameObject);
            }
            else
            {
                if (BuildingManager.Instance.GetYourHQBuilding() != null)
                {
                    movDire = ((BuildingManager.Instance.GetYourHQBuilding().transform.position) - transform.position).normalized;
                }

                else Destroy(gameObject);
            }
            rigidbody2d.velocity = movDire * movementSpeed;
            //rigidbody2d.velocity = Vector2.zero;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)  //collision --> çarptıgımız objenin collider'ı
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

            if (scissors)
            {
                if (enemyMinion.minionType.nameString == "Rock") healthSystem.Damage(damageAmount / 2);
                if (enemyMinion.minionType.nameString == "Scissors") healthSystem.Damage(damageAmount);
                if (enemyMinion.minionType.nameString == "StrongerRock") healthSystem.Damage(damageAmount / 2);
                if (enemyMinion.minionType.nameString == "Paper") healthSystem.Damage(mdamageAmount);
                if (enemyMinion.minionType.nameString == "StrongerPaper") healthSystem.Damage(damageAmount);
                if (enemyMinion.minionType.nameString == "StrongerScissors") healthSystem.Damage(damageAmount);

            }
            if (paper)
            {
                if (enemyMinion.minionType.nameString == "Paper") healthSystem.Damage(damageAmount);
                if (enemyMinion.minionType.nameString == "Rock") healthSystem.Damage(mdamageAmount);
                if (enemyMinion.minionType.nameString == "StrongerRock") healthSystem.Damage(damageAmount);
                if (enemyMinion.minionType.nameString == "Scissors") healthSystem.Damage(damageAmount / 2);
                if (enemyMinion.minionType.nameString == "StrongerPaper") healthSystem.Damage(damageAmount);
                if (enemyMinion.minionType.nameString == "StrongerScissors") healthSystem.Damage(damageAmount / 2);
            }
            if (rock)
            {
                if (enemyMinion.minionType.nameString == "Rock") healthSystem.Damage(damageAmount);
                if (enemyMinion.minionType.nameString == "Scissors") healthSystem.Damage(damageAmount);
                if (enemyMinion.minionType.nameString == "StrongerRock") healthSystem.Damage(damageAmount);
                if (enemyMinion.minionType.nameString == "Paper") healthSystem.Damage(damageAmount / 2);
                if (enemyMinion.minionType.nameString == "StrongerPaper") healthSystem.Damage(damageAmount / 2);
                if (enemyMinion.minionType.nameString == "StrongerScissors") healthSystem.Damage(mdamageAmount);
            }
            if (strongerRock)
            {
                if (enemyMinion.minionType.nameString == "StrongerRock") healthSystem.Damage(mdamageAmount);
                if (enemyMinion.minionType.nameString == "Scissors") healthSystem.Damage(mdamageAmount);
                if (enemyMinion.minionType.nameString == "Rock") healthSystem.Damage(mdamageAmount);
                if (enemyMinion.minionType.nameString == "Paper") healthSystem.Damage(mdamageAmount / 2);
                if (enemyMinion.minionType.nameString == "StrongerPaper") healthSystem.Damage(mdamageAmount / 2);
                if (enemyMinion.minionType.nameString == "StrongerScissors") healthSystem.Damage(mdamageAmount);
            }
            if (strongerPaper)
            {
                if (enemyMinion.minionType.nameString == "StrongerPaper") healthSystem.Damage(mdamageAmount);
                if (enemyMinion.minionType.nameString == "Rock") healthSystem.Damage(mdamageAmount);
                if (enemyMinion.minionType.nameString == "StrongerRock") healthSystem.Damage(mdamageAmount);
                if (enemyMinion.minionType.nameString == "Scissors") healthSystem.Damage(mdamageAmount / 2);
                if (enemyMinion.minionType.nameString == "Paper") healthSystem.Damage(mdamageAmount);
                if (enemyMinion.minionType.nameString == "StrongerScissors") healthSystem.Damage(mdamageAmount / 2);
            }
            if (strongerScissors)
            {
                if (enemyMinion.minionType.nameString == "StrongerScissors") healthSystem.Damage(mdamageAmount);
                if (enemyMinion.minionType.nameString == "Rock") healthSystem.Damage(mdamageAmount / 2);
                if (enemyMinion.minionType.nameString == "StrongerRock") healthSystem.Damage(mdamageAmount / 2);
                if (enemyMinion.minionType.nameString == "Paper") healthSystem.Damage(mdamageAmount);
                if (enemyMinion.minionType.nameString == "StrongerPaper") healthSystem.Damage(mdamageAmount);
                if (enemyMinion.minionType.nameString == "Scissors") healthSystem.Damage(mdamageAmount);

            }
        }
    }

}
//private int ToggleMinionStringToInteger(MinionTypeSO minionType)
//{
//    string minionName = minionType.nameString;
//    int minionNumber;
//    switch (minionName)
//    {
//        case "Paper":
//            minionNumber = 0;
//            break;
//        case "Rock":
//            minionNumber = 1;
//            break;
//        case "Scissors":
//            minionNumber = 2;
//            break;
//        case "StrongerPaper":
//            minionNumber = 3;
//            break;
//        case "StrongerRock":
//            minionNumber = 4;
//            break;
//        case "StrongerScissors":
//            minionNumber = 5;
//            break;
//        default:
//            minionNumber = 0;
//            break;

//    }
//    return minionNumber;
//}

//if (minionType.nameString == "Scissors")
//{
//    if (minionType.nameString == "Rock") healthSystem.Damage(damageAmount / 2);
//    if (minionType.nameString == "StrongerRock") healthSystem.Damage(damageAmount / 2);
//    if (minionType.nameString == "Paper") healthSystem.Damage(damageAmount);
//    if (minionType.nameString == "StrongerPaper") healthSystem.Damage(damageAmount);
//    if (minionType.nameString == "StrongerScissors") healthSystem.Damage(damageAmount);

//}
//if (minionType.nameString == "Paper")
//{
//    if (minionType.nameString == "Rock") healthSystem.Damage(damageAmount);
//    if (minionType.nameString == "StrongerRock") healthSystem.Damage(damageAmount);
//    if (minionType.nameString == "Scissors") healthSystem.Damage(damageAmount / 2);
//    if (minionType.nameString == "StrongerPaper") healthSystem.Damage(damageAmount);
//    if (minionType.nameString == "StrongerScissors") healthSystem.Damage(damageAmount / 2);
//}
//if (minionType.nameString == "Rock")
//{
//    if (minionType.nameString == "Scissors") healthSystem.Damage(damageAmount);
//    if (minionType.nameString == "StrongerRock") healthSystem.Damage(damageAmount);
//    if (minionType.nameString == "Paper") healthSystem.Damage(damageAmount / 2);
//    if (minionType.nameString == "StrongerPaper") healthSystem.Damage(damageAmount / 2);
//    if (minionType.nameString == "StrongerScissors") healthSystem.Damage(damageAmount);
//}
//if (minionType.nameString == "StrongerRock")
//{
//    if (minionType.nameString == "Scissors") healthSystem.Damage(mdamageAmount);
//    if (minionType.nameString == "Rock") healthSystem.Damage(mdamageAmount);
//    if (minionType.nameString == "Paper") healthSystem.Damage(mdamageAmount / 2);
//    if (minionType.nameString == "StrongerPaper") healthSystem.Damage(mdamageAmount / 2);
//    if (minionType.nameString == "StrongerScissors") healthSystem.Damage(mdamageAmount);
//}
//if (minionType.nameString == "StrongerPaper")
//{
//    if (minionType.nameString == "Rock") healthSystem.Damage(mdamageAmount);
//    if (minionType.nameString == "StrongerRock") healthSystem.Damage(mdamageAmount);
//    if (minionType.nameString == "Scissors") healthSystem.Damage(mdamageAmount / 2);
//    if (minionType.nameString == "Paper") healthSystem.Damage(mdamageAmount);
//    if (minionType.nameString == "StrongerScissors") healthSystem.Damage(mdamageAmount / 2);
//}
//if (minionType.nameString == "StrongerScissors")
//{
//    if (minionType.nameString == "Rock") healthSystem.Damage(mdamageAmount / 2);
//    if (minionType.nameString == "StrongerRock") healthSystem.Damage(mdamageAmount / 2);
//    if (minionType.nameString == "Paper") healthSystem.Damage(mdamageAmount);
//    if (minionType.nameString == "StrongerPaper") healthSystem.Damage(mdamageAmount);
//    if (minionType.nameString == "Scissors") healthSystem.Damage(mdamageAmount);

//}