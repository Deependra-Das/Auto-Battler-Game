using AutoBattler.Event;
using AutoBattler.Main;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BuffService
{
    private Dictionary<BuffNameEnum, BuffData> _buffData = new();
    private Dictionary<TeamEnum, Dictionary<BuffNameEnum, int>> _appliedBuffs = new();

    public BuffService(BuffScriptableObjectScript buff_SO)
    {
        SubscribeToEvents();

        foreach (var buffs in buff_SO.buffData)
        {
            _buffData.Add(buffs.buffName, buffs);
        }
    }

    ~BuffService() 
    {
        UnsubscribeToEvents();
    }

    void SubscribeToEvents()
    {
        EventBusManager.Instance.Subscribe(EventNameEnum.UnitAddedOnField, OnUnitAddedOnField_Buff);
    }

    void UnsubscribeToEvents()
    {
        EventBusManager.Instance.Unsubscribe(EventNameEnum.UnitAddedOnField, OnUnitAddedOnField_Buff);
    }


    public void InitializeBuffs()
    {
        foreach (TeamEnum team in Enum.GetValues(typeof(TeamEnum)))
        {
            _appliedBuffs[team] = new Dictionary<BuffNameEnum, int>();

            foreach (var buffs in _buffData)
            {
                _appliedBuffs[team].Add(buffs.Key, 0);

                if (team == TeamEnum.Team1)
                {
                    UIManager.Instance.AddBuffDetailUICard(buffs.Value);
                }
            }
        }
    }

    public void ResetBuffs()
    {
        _appliedBuffs.Clear();
    }

    private void OnUnitAddedOnField_Buff(object[] parameters)
    {
        TeamEnum team = (TeamEnum)parameters[0];
        UnitTypeEnum type = (UnitTypeEnum)parameters[1];
        UnitFactionEnum faction = (UnitFactionEnum)parameters[2];

        UpdateAppliedBuffsParticipants(team, type, faction);
    }

    private void UpdateAppliedBuffsParticipants(TeamEnum team, UnitTypeEnum type, UnitFactionEnum faction)
    {
        int typeParticpantCount = GameManager.Instance.Get<TeamService>().GetTypeCount(team, type);
        int factionParticpantCount = GameManager.Instance.Get<TeamService>().GetFactionCount(team, faction);

        switch (type)
        {
            case UnitTypeEnum.Attacker:
                break;
            case UnitTypeEnum.Ranged:
                break;
            case UnitTypeEnum.Support:
                break;
            case UnitTypeEnum.Tank:
                break;
        }

        switch (faction)
        {
            case UnitFactionEnum.Crusader:
                break;
            case UnitFactionEnum.Spartan:
                break;
            case UnitFactionEnum.Viking:
                break;
        }

        Debug.Log(typeParticpantCount + "--" + factionParticpantCount);
    }
}
