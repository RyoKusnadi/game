using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseIndividualController
{
    public GameObject model;
    public PlayerInput playerInput;
    public LockController lockController;
    public float runMultiplier = 2.4f;
    public float jumpVelocity = 5.0f;
    public float rollVeticalVelocity = 0f;

    public float rotateRatio = 0.3f;
    public float runRatio = 0.3f; 

    [Space(10)]
    public PhysicMaterial frictionOne;
    public PhysicMaterial frictionZero;

    [SerializeField]
    private Animator animator;

    private new Rigidbody rigidbody;
    private CapsuleCollider capsuleCollider;
    private Vector3 planeVec;
    private Vector3 thrustVec;
    private bool canAttack;
    private bool lockPlane = false;
    private bool trackDirection = false;
    private Vector3 deltaPos;
    public bool leftIsShield = true;
    private float walkSpeed = 1.6f;

    private float forward = 0.0f;
    private float right = 0.0f;

    private Individual selfIndividual;


    void Awake()
    {
        selfIndividual = GetComponent<Individual>();
        animator = model.GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }
    private void Start()
    {
        InitRegister();
    }

    void FixedUpdate()
    {
        walkSpeed = selfIndividual.speed / 8.0f;

        rigidbody.position += deltaPos;
        rigidbody.velocity = new Vector3(planeVec.x, rigidbody.velocity.y, planeVec.z) + thrustVec;
        thrustVec = Vector3.zero;
        deltaPos = Vector3.zero;

        if (playerInput.lockon)
        {
            lockController.LockSwitch();
        }

        if (lockController.lockState == false)
        {
            float targetRunMulti = ((playerInput.run) ? 2.0f : 1.0f);
            forward = playerInput.Dmag * Mathf.Lerp(animator.GetFloat("forward"), targetRunMulti, runRatio);
        }
        else
        {
            Vector3 localDevc = transform.InverseTransformVector(playerInput.Dvec);
            forward = localDevc.z * ((playerInput.run) ? 2.0f : 1.0f);
            right = localDevc.x * ((playerInput.run) ? 2.0f : 1.0f);
        }


        if (playerInput.roll)
        {
            animator.SetTrigger("roll");
            canAttack = false;
        }

        if (playerInput.jump)
        {
            animator.SetTrigger("jump");
            canAttack = false;
        }

        if ((playerInput.lHand || playerInput.rHand) && (CheckState("ground") || CheckStateTag("attack")) && canAttack)
        {
            if (playerInput.rHand)
            {
                animator.SetBool("R0L1", false);
                animator.SetTrigger("attack");
            }
            else if (playerInput.lHand && !leftIsShield)
            {
                animator.SetBool("R0L1", true);
                animator.SetTrigger("attack");
            }
        }

        //----TODO
        if (lockController.lockState == false)
        {
            if (playerInput.Dmag > 0.1f)
            {
                Vector3 targetForward = Vector3.Slerp(model.transform.forward, playerInput.Dvec, rotateRatio);
                model.transform.forward = targetForward;
            }

            if (lockPlane == false)
            {
                planeVec = playerInput.Dmag * model.transform.forward * walkSpeed *
                    ((playerInput.run) ? runMultiplier : 1.0f);
            }
        }
        else
        {
            if (trackDirection == false)
            {
                model.transform.forward = transform.forward;
            }
            else
            {
                model.transform.forward = planeVec.normalized;
            }
            if (lockPlane == false)
            {
                planeVec = playerInput.Dvec * walkSpeed * ((playerInput.run) ? runMultiplier : 1.0f);
            }
        }


        animator.SetFloat("attackSpeed", selfIndividual.attackSpeed);
        animator.SetFloat("bodyVelocityMagnitude", rigidbody.velocity.magnitude);

        Walk(new Vector3(forward, right, 0));
    }

    /// <param name="stateName"></param>
    /// <param name="layerName"></param>
    /// <returns></returns>
    private bool CheckState(string stateName, string layerName = "Base Layer")
    {
        return animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex(layerName)).IsName(stateName);
    }

    private bool CheckStateTag(string tagName, string layerName = "Base Layer")
    {
        return animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex(layerName)).IsTag(tagName);
    }

    public void OnJumpEnter()
    {
        thrustVec = new Vector3(0, jumpVelocity, 0);
        playerInput.inputEnabled = false;
        lockPlane = true;
        trackDirection = true;
    }
    public void IsGround()
    {
        animator.SetBool("isGround", true);
    }
    public void IsNotGround()
    {
        animator.SetBool("isGround", false);
    }
    public void OnGroundEnter()
    {
        playerInput.inputEnabled = true;
        lockPlane = false;
        canAttack = true;
        capsuleCollider.material = frictionOne;
        trackDirection = false;
    }
    public void OnGroundExit()
    {
        capsuleCollider.material = frictionZero;
    }
    public void OnFallEnter()
    {
        playerInput.inputEnabled = false;
        lockPlane = true;
    }
    public void OnRollEnter()
    {
        thrustVec = new Vector3(0, rollVeticalVelocity, 0);
        playerInput.inputEnabled = false;
        lockPlane = true;
        trackDirection = true;
    }

    public void OnRollExit()
    {

    }

    public void OnJabEnter()
    {
        playerInput.inputEnabled = false;
        lockPlane = true;
    }
    public void OnJabUpdate()
    {
        thrustVec = model.transform.forward * animator.GetFloat("jabVelocity");
    }
    public void OnAttack1hAEnter()
    {
        playerInput.inputEnabled = false;
        // lockPlane = true;
        // lerpTarget = 1.0f;
    }
    public void OnAttack1hAUpdate()
    {
        thrustVec = model.transform.forward * animator.GetFloat("attack1aAVelocity");
      
    }

    #region AnimationEvent
    public void OnUpdateRM(object _deltaPos)
    {
        if (CheckState("attack1hC"))
        {
            //deltaPos += (Vector3)_deltaPos;
            deltaPos += (0.8f * deltaPos + 0.2f * (Vector3)_deltaPos) / 1.0f;
        }
    }

    #endregion

    public override void Walk(Vector3 velocity)
    {
        animator.SetFloat("forward", velocity.x);
        animator.SetFloat("right", velocity.y);
    }

    public override void GetDamaged(int sourceID, float damage)
    {
        selfIndividual.HealthChange(-damage);
        if (selfIndividual.health < 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("hit");
        }
    }
    public override void Attack(Individual ind)
    {
        if (ind == null) return;
        messageSystem.SendMessage(1, ind.ID, selfIndividual.attack);
    }

    public override void Die()
    {
        animator.SetTrigger("die");

        
        gameObject.layer = 0;//default layer

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        selfIndividual.enabled = false;
        this.enabled = false;
        messageSystem.SendMessage(3, 0, 0);
    }

}