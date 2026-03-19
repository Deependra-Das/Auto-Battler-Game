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

        UpdateAppliedBuffsParticipants(team, type, faction, true);
    }

    private void OnUnitRemovedFromField_Buff(object[] parameters)
    {
        TeamEnum team = (TeamEnum)parameters[0];
        UnitTypeEnum type = (UnitTypeEnum)parameters[1];
        UnitFactionEnum faction = (UnitFactionEnum)parameters[2];

        UpdateAppliedBuffsParticipants(team, type, faction, false);
    }

    private void UpdateAppliedBuffsParticipants(TeamEnum team, UnitTypeEnum type, UnitFactionEnum faction, bool action)
    {
        int typeParticpantCount = GameManager.Instance.Get<TeamService>().GetTypeCount(team, type);
        int factionParticpantCount = GameManager.Instance.Get<TeamService>().GetFactionCount(team, faction);

        if (action)
        {
            ApplyBuff(team, GetTypeBuff(type));
            ApplyBuff(team, GetFactionBuff(faction));
        }
    }

    private BuffNameEnum GetTypeBuff(UnitTypeEnum type)
    {
        BuffNameEnum typeBuff = type switch
        {
            UnitTypeEnum.Attacker => BuffNameEnum.Might,
            UnitTypeEnum.Ranged => BuffNameEnum.Haste,
            UnitTypeEnum.Support => BuffNameEnum.Recovery,
            UnitTypeEnum.Tank => BuffNameEnum.Guard,
            _ => throw new Exception($"Unhandled UnitTypeEnum: {type}")
        };

        return typeBuff;
    }

    private BuffNameEnum GetFactionBuff(UnitFactionEnum faction)
    {
        BuffNameEnum factionBuff = faction switch
        {
            UnitFactionEnum.Crusader => BuffNameEnum.Flame,
            UnitFactionEnum.Spartan => BuffNameEnum.Verdant,
            UnitFactionEnum.Viking => BuffNameEnum.Thunder,
            _ => throw new Exception($"Unhandled UnitFactionEnum: {faction}")
        };

        return factionBuff;
    }

    void ApplyBuff(TeamEnum team, BuffNameEnum buff)
    {
        if (!_appliedBuffs.TryGetValue(team, out var buffs))
            return;

        if (buffs.ContainsKey(buff))
        {
            buffs[buff]++;
            UIManager.Instance.UpdateBuffParticipantCount(buff, buffs[buff]);
        }
    }
}
