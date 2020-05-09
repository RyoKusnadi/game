using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HatredSystem : MonoBehaviour
{
    private Individual individual;

    [SerializeField] private List<string> hatredListShow = new List<string>();
    Dictionary<Individual, int> hatredMap = new Dictionary<Individual, int>();
    private BehaviorTree behaviorTree;
    private MessageSystem messageSystem;

    private void Awake()
    {
        messageSystem = GetComponent<MessageSystem>();
        individual = GetComponent<Individual>();
        behaviorTree = GetComponent<BehaviorTree>();
    }

    private void Start()
    {
        messageSystem.registerAttackEvent((int attackerID, float damage) => { AddHateValue(attackerID); });
        messageSystem.registerDieEvent((int sender) => { if (sender == individual.ID) { this.enabled = false; } });
    }

    private void Update()
    {

    }

    public void AddHateValue(int HateSourceID)
    {
        Individual HateSource = Factory.GetIndividual(HateSourceID);
        if (HateSource == null)
        {
            Logger.Log("HateSource is null", LogType.Hatred);
            return;
        }

        SharedTransform sf = GetMostHatedTarget();
        behaviorTree.SetVariable("MostHatredTarget", sf);

    }
    public Transform GetMostHatedTarget()
    {
        int maxValue = 0;
        Individual targetInd = null;
        foreach (KeyValuePair<Individual, int> kvp in hatredMap)
        {
            if (kvp.Value > maxValue)
            {
                maxValue = kvp.Value;
                targetInd = kvp.Key;
            }
        }

        if (!targetInd)
            return null;

        if (!targetInd.enabled)
        {
            hatredMap.Remove(targetInd);
            hatredListShow.Remove(targetInd.name);
            return null;
        }

        return targetInd.transform;
    }

    private void AddHatredList(Individual HateSource)
    {
        hatredMap.Add(HateSource, HateSource.hatredValue); 
    }
}

