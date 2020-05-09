using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("System/MessageSystem")]
public class MessageSystem : MonoBehaviour
{

    public int selfID = 0;
    private List<Action<int, float>> attackEventListeners = new List<Action<int, float>>();
    private List<Action<int, int>> buffEventListeners = new List<Action<int, int>>();
    private List<Action<int>> dieEventListeners = new List<Action<int>>();

    private void Awake()
    {
        EventCenter.AddListener<int, int, int, object>(EventType.Message, SolveMessage);
    }


    /// </summary>
    /// <param name="messageID"></param>
    /// <param name="ob"></param>
    /// <param name="senderID"></param>
    /// <param name="receverID"></param>
    public void SolveMessage(int messageID, int senderID, int receverID, object ob)
    {
        if (receverID != 0 && receverID != selfID ) return;

        switch (messageID)
        {
            case 1: UnderAttack(senderID, (float)ob); break;
            case 2: GainBuff(senderID, (int)ob); break;
            case 3: IndividualDie(senderID); break;
            default: break;
        }
    }


    /// <param name="messageID"></param>
    /// <param name="receverID"></param>
    /// <param name="ob"></param>
    public void SendMessage(int messageID, int receverID, object ob)
    {
        switch (messageID)
        {
            case 1: EventCenter.Broadcast<int, int, int, object>(EventType.Message, messageID, selfID, receverID, ob); break;
            case 2: EventCenter.Broadcast<int, int, int, object>(EventType.Message, messageID, selfID, receverID, ob); break;
            case 3: EventCenter.Broadcast<int, int, int, object>(EventType.Message, messageID, selfID, receverID, ob); break;
            default: break;
        }
    }

    public void registerAttackEvent(Action<int,float> action)
    {
        attackEventListeners.Add(action);
    }

    public void registerBuffEvent(Action<int, int> action)
    {
        buffEventListeners.Add(action);
    }

    /// <param name="action"></param>
    public void registerDieEvent(Action<int> action)
    {
        dieEventListeners.Add(action);
    }


    private void UnderAttack(int senderID, float damage)
    {
        for (int i = 0; i < attackEventListeners.Count; ++i)
        {
            attackEventListeners[i](senderID, damage);
        }
    }
    private void GainBuff(int senderID, int buffID)
    {
        for(int i = 0; i < buffEventListeners.Count; ++i)
        {
            buffEventListeners[i](senderID,buffID);
        }
    }
    private void IndividualDie(int senderID)
    {
        for (int i = 0; i < dieEventListeners.Count; ++i)
        {
            dieEventListeners[i](senderID);
        }
    }
}
