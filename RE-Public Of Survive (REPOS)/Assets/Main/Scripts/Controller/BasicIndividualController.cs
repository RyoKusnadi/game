using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseIndividualController : MonoBehaviour
{
    protected MessageSystem messageSystem;
    //deklarasi
    protected void InitRegister()
    {
        messageSystem = GetComponent<MessageSystem>();
        messageSystem.registerAttackEvent(GetDamaged);
    }

    public abstract void Walk(Vector3 velocity);

    public abstract void GetDamaged(int sourceID, float damage);

    public abstract void Attack(Individual ind);

    public abstract void Die();

}
