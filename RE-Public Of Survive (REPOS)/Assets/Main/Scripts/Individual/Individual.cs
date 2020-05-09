using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Individual : MonoBehaviour
{
    public IndividualType individualType = IndividualType.Normal; 

    public int ID = 0;
    public float health = 100;         
    public float maxHealth = 100;      
    public float attack = 10;          
    public float attackSpeed = 1.0f;   
    public float speed = 1.0f;         
    public float attackDistance = 2.0f;
    public float recoverRate = 0.5f;   

    public int hatredValue = 10;       

    public enum Power {Monster, Human, Neutral}
    public Power power = Power.Neutral;

    public bool movable = true;        
    public bool attackable = true;     
    public bool controllable = true;   

    public bool tauntable = true;      
    public bool charmable = true;      
    public bool restrictable = true;   
    public bool speedDownAble = true;  
    public bool dizzyAble = true;      

    public int reviveCount = 0;         
    public int maxReviveCount = 0;      

    private void Awake()
    {
        Factory.RegisterIndividual(this, individualType);
        GetComponent<MessageSystem>().selfID = ID;
    }

    private void OnDisable()
    {
        Factory.RemoveIndividual(ID);
    }

    void Start()
    {
    }
    private void Update()
    {
        health += recoverRate * Time.deltaTime;
    }

    public void HealthChange(float increment)
    {
        health += increment;
        health = Mathf.Min(health, maxHealth);
    }
    public void AttackChange(int increment)
    {
        attack += increment;
    }

    public void AttackChange(double increment_p)
    {
        attack = (int)(1.0f + increment_p) * attack;
    }

    public void AttackSpeedChange(double increment_p)
    {
        attackSpeed = (float)(attackSpeed + increment_p);
    }

    public void SpeedChange(double increment_p)
    {
        speed = (float)(speed + increment_p);
    }

    public void RecoverRateChange(int increment)
    {
        recoverRate += increment;
    }

    public void RecoverRateChange(double increment_p)
    {
        recoverRate = (float)(1.0f + increment_p) * recoverRate;
    }

    public void ReviveCountChange(int increment)
    {
        reviveCount += increment;
    }

}
