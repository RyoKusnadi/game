using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour
{
    #region Fields
    [SerializeField]
    private int _MAX_IDQUEUE_SIZE = 256;

    private MessageSystem messageSystem;

    private static Queue<int> _IDQueue;

    private static Dictionary<int, Individual> _IDToIndividualDictionary;

    private static List<int> _IDToRemove;

    private static Individual player = null;

    private static Individual baseIndividual = null;

    #endregion

    #region Properties

    public static Dictionary<int, Individual> IDToIndividualDictionary { get => _IDToIndividualDictionary;}

    public static Individual PlayerIndividual { get => player;}

    public static Individual BaseIndividual { get => baseIndividual;}

    #endregion

    #region Public Methods

    public static void RegisterIndividual(Individual ind, IndividualType individualType = IndividualType.Normal)
    {
        switch (individualType)
        {
            case IndividualType.Normal:
                break;
            case IndividualType.Player:
                if (player) { Logger.Log("Warning!PlayerIndividual already existed!!", LogType.Individual); }
                player = ind;
                break;
            case IndividualType.BaseIndividual:
                if (baseIndividual) { Logger.Log("Warning!baseIndividual already existed!!", LogType.Individual); }
                baseIndividual = ind;
                break;
        }
    }
    public static void RemoveIndividual(int individualID)
    {
    }

    public static Individual GetIndividual(int ID)
    {
        if (IDToIndividualDictionary.ContainsKey(ID))
        {
            return IDToIndividualDictionary[ID];
        }
        Logger.Log($"Individual { ID } is NOT found.", LogType.Individual);
        return null;
    }

    public static void TraversalIndividuals(Action<Individual> action, Func<Individual, bool> condition)
    {
        foreach (var pair in IDToIndividualDictionary)
        {
            Individual ind = pair.Value;
            if (condition(ind))
            {
                action(ind);
            }
        }
    }

    public static void TraversalIndividuals(Action<Individual> action)
    {
        foreach (var pair in IDToIndividualDictionary)
        {
           action(pair.Value);
        }
    }

    public static void TraversalIndividualsInCircle(Action<Individual> action, Vector3 point, float radius, Func<Individual, bool> condition)
    {
        foreach (var pair in IDToIndividualDictionary)
        {
            Individual ind = pair.Value;
            if ((ind.transform.position - point).sqrMagnitude < radius * radius && condition(ind))
            {
                action(ind);
            }
        }
    }
    public static void TraversalIndividualsInCircle(Action<Individual> action, Vector3 point, float radius)
    {
        foreach (var pair in IDToIndividualDictionary)
        {
            Individual ind = pair.Value;
            if ((ind.transform.position - point).sqrMagnitude < radius * radius)
            {
                action(ind);
            }
        }
    }

    public static bool HasMonsterIndividual()
    {
        foreach (var ind in _IDToIndividualDictionary.Values)
        {
            if (ind.power == Individual.Power.Monster) return true;
        }

        return false;
    }


    public static bool IsPlayerDead()
    {
        Individual player = GetIndividual(0);
        if (player)
        {
            if (player.health <= 0)
            {
                return true;
            }
            return false;
        }
        else
        {
            return true;
        }
    }

    public static bool IsBaseDestroyed()
    {
        Individual iBase = GetIndividual(1);
        if (iBase)
        {
            if (iBase.health <= 0)
            {
                return true;
            }
            return false;
        }
        else
        {
            return true;
        }
    }

    public static bool IsGameOver()
    {
        return IsPlayerDead() || IsBaseDestroyed();
    }

    #endregion


    #region Mono
    void Awake()
    {
        messageSystem = GetComponent<MessageSystem>();
        _IDQueue = new Queue<int>(_MAX_IDQUEUE_SIZE);
        for (int id = 1; id < _MAX_IDQUEUE_SIZE; id++) _IDQueue.Enqueue(id);

        _IDToIndividualDictionary = new Dictionary<int, Individual>();

        _IDToRemove = new List<int>();
    }

    void Start()
    {

    }

    void Update()
    {

    }

    private void LateUpdate()
    {
        LazyRemoveIndividuals();
    }

    #endregion

    #region private

    private void LazyRemoveIndividuals()
    {
        for (int i = 0; i < _IDToRemove.Count; ++i)
        {
            if (IDToIndividualDictionary.Remove(_IDToRemove[i]))
            {
                _IDQueue.Enqueue(_IDToRemove[i]);
            }
        }
        _IDToRemove.Clear();
    }

    #endregion
}

public enum IndividualType {
    Normal, Player, BaseIndividual
};