using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : BaseIndividualController
{
    public float removeTime = 3.0f; 

    private Individual selfIndividual;
    private Animator animator;
    private new Rigidbody rigidbody;
    private NavMeshAgent navMeshAgent;
    private BehaviorTree behaviorTree;
    private HatredSystem hatredSystem;

    private void Awake()
    {
        selfIndividual = GetComponent<Individual>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        behaviorTree = GetComponent<BehaviorTree>();
        hatredSystem = GetComponent<HatredSystem>();
    }

    private void Start()
    {
        InitRegister();
    }

    private void Update()
    {
        //navMeshAgent.speed = selfIndividual.speed;
        //fixed position due doesnt have time
    }

    private void FixedUpdate()
    {
        Walk(rigidbody.velocity);
    }

    /// <param name="velocity"></param>
    public override void Walk(Vector3 velocity)
    {
        animator.SetFloat("Velocity",velocity.magnitude);
    }

    /// <param name="sourceID"></param>
    /// <param name="damage"></param>
    public override void GetDamaged(int sourceID, float damage)
    {
        selfIndividual.HealthChange(-damage);
        if (selfIndividual.health < 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Hit");
        }
    }

    public override void Attack(Individual ind)
    {
        if (ind == null) return;

        messageSystem.SendMessage(1, ind.ID, selfIndividual.attack);
    }
    
    // Todo : Enemy First.
    public override void Die()
    {
        gameObject.layer = 12;
        animator.SetTrigger("Die");
        behaviorTree.enabled = false;
        messageSystem.enabled = false;
        selfIndividual.enabled = false;
        this.enabled = false;
        messageSystem.SendMessage(3,0,0);
        StartCoroutine(RemoveObject());
    }
    
    public void StartAttack()
    {
        var target = hatredSystem.GetMostHatedTarget();
        if (target && (target.position-transform.position).sqrMagnitude < 2.0f * transform.localScale.x * transform.localScale.x)
        {
            Attack(target.GetComponent<Individual>());
        }

    }

    private IEnumerator RemoveObject()
    {
        yield return new WaitForSeconds(removeTime);
        Destroy(gameObject);
        yield break;
    }

}
