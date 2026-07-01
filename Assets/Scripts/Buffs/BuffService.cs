using AutoBattler.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffService
{
    private Dictionary<BuffNameEnum, BuffData> _buffData = new();
    private Dictionary<TeamEnum, Dictionary<BuffNameEnum, int>> _appliedBuffs = new();
    private Dictionary<TeamEnum, TeamBuffData> _teamBuffData = new();

    public BuffService(BuffScriptableObjectScript buff_SO)
    {
        SubscribeToEvents();

        foreach (var buffs in buff_SO.buffData)
        {
            _buffData.Add(buffs.buffName, buffs);
        }

        foreach (TeamEnum team in Enum.GetValues(typeof(TeamEnum)))
        {
            _teamBuffData[team] = new TeamBuffData();
        }
    }

    public void Dispose() 
    {
        UnsubscribeToEvents();
        _buffData.Clear();
        _appliedBuffs.Clear();
        _teamBuffData.Clear();
    }

    void SubscribeToEvents()
    {
        EventBusManager.Instance.Subscribe(EventNameEnum.UnitAddedOnField, OnUnitAddedOnField_Buff);
        EventBusManager.Instance.Subscribe(EventNameEnum.UnitRemovedFromField, OnUnitRemovedFromField_Buff);
    }

    void UnsubscribeToEvents()
    {
        EventBusManager.Instance.Unsubscribe(EventNameEnum.UnitAddedOnField, OnUnitAddedOnField_Buff);
        EventBusManager.Instance.Unsubscribe(EventNameEnum.UnitRemovedFromField, OnUnitRemovedFromField_Buff);
    }

    public void InitializeBuffs()
    {
        foreach (TeamEnum team in Enum.GetValues(typeof(TeamEnum)))
        {
            _appliedBuffs[team] = new Dictionary<BuffNameEnum, int>();

            foreach (var buffs in _buffData)
            {
                _appliedBuffs[team].Add(buffs.Key, 0);
                UIManager.Instance.AddBuffDetailUICard(buffs.Value, team);
            }
        }
    }

    private void OnUnitAddedOnField_Buff(object[] parameters)
    {
        HandleUnitChange(parameters, true);
    }

    private void OnUnitRemovedFromField_Buff(object[] parameters)
    {
        HandleUnitChange(parameters, false);
    }

    private void HandleUnitChange(object[] parameters, bool isAdded)
    {
        TeamEnum team = (TeamEnum)parameters[0];
        UnitTypeEnum type = (UnitTypeEnum)parameters[1];
        UnitFactionEnum faction = (UnitFactionEnum)parameters[2];

        ModifyBuff(team, GetTypeBuff(type), isAdded);
        ModifyBuff(team, GetFactionBuff(faction), isAdded);
    }

    private void ModifyBuff(TeamEnum team, BuffNameEnum buff, bool isAdded)
    {
        if (!_appliedBuffs.ContainsKey(team) || !_appliedBuffs[team].ContainsKey(buff))
            return;

        int oldCount = _appliedBuffs[team][buff];
        int newCount = Mathf.Max(0, oldCount + (isAdded ? 1 : -1));

        _appliedBuffs[team][buff] = newCount;

        int oldTier = GetTierIndex(_buffData[buff].buffParticipantTierList, oldCount);
        int newTier = GetTierIndex(_buffData[buff].buffParticipantTierList, newCount);

        if (oldTier == newTier) return;

        TeamBuffData teamBuff = CalculateTeamBuff(team);
        _teamBuffData[team] = teamBuff;
        UIManager.Instance.UpdateBuffParticipantCountUI(buff, newCount, team);
        EventBusManager.Instance.Raise(EventNameEnum.TeamBuffUpdated, team, teamBuff);        
    }

    private TeamBuffData CalculateTeamBuff(TeamEnum team)
    {
        TeamBuffData data = new TeamBuffData();

        foreach (var buff in _appliedBuffs[team])
        {
            float value = GetBuffValue(_buffData[buff.Key].buffParticipantTierList, buff.Value);

            switch (buff.Key)
            {
                case BuffNameEnum.Might: data.attackBonus = value; break;
                case BuffNameEnum.Guard: data.shieldBonus = value; break;
                case BuffNameEnum.Recovery: data.hpBonus = value; break;
                case BuffNameEnum.Haste: data.attackSpeedBonus = value; break;

                case BuffNameEnum.Blaze: data.fireDamageBonus = value; break;
                case BuffNameEnum.Surge: data.thunderDamageBonus = value; break;
                case BuffNameEnum.Verdant: data.natureDamageBonus = value; break;
            }
        }

        return data;
    }

    private BuffNameEnum GetTypeBuff(UnitTypeEnum type)
    {
        return type switch
        {
            UnitTypeEnum.Attacker => BuffNameEnum.Might,
            UnitTypeEnum.Ranged => BuffNameEnum.Haste,
            UnitTypeEnum.Support => BuffNameEnum.Recovery,
            UnitTypeEnum.Tank => BuffNameEnum.Guard,
            _ => throw new Exception($"Unhandled UnitTypeEnum: {type}")
        };
    }

    private BuffNameEnum GetFactionBuff(UnitFactionEnum faction)
    {
        return faction switch
        {
            UnitFactionEnum.Crusader => BuffNameEnum.Blaze,
            UnitFactionEnum.Viking => BuffNameEnum.Surge,
            UnitFactionEnum.Spartan => BuffNameEnum.Verdant,
            _ => throw new Exception($"Unhandled UnitFactionEnum: {faction}")
        };
    }

    private float GetBuffValue(BuffParticipantTier[] tiers, int count)
    {
        float value = 0f;
        foreach (var tier in tiers)
        {
            if (count >= tier.participants)
                value = tier.buffValue;
        }
        return value;
    }

    private int GetTierIndex(BuffParticipantTier[] tiers, int count)
    {
        int index = -1;
        for (int i = 0; i < tiers.Length; i++)
        {
            if (count >= tiers[i].participants)
                index = i;
        }
        return index;
    }

    public void RemoveAllAppliedBuffs(TeamEnum team)
    {
        if (!_appliedBuffs.ContainsKey(team))
            return;

        _appliedBuffs[team].Keys.ToList().ForEach(key => _appliedBuffs[team][key] = 0);
    }

    public TeamBuffData GetTeamBuffData(TeamEnum team)
    {
        return _teamBuffData[team];
    }

    public void Reset()
    {
        UIManager.Instance.RemoveAllBuffDetailUICards();

        foreach (var team in _appliedBuffs.Keys.ToList())
        {
            RemoveAllAppliedBuffs(team);
        }

        foreach (var team in _teamBuffData.Keys.ToList())
        {
            _teamBuffData[team] = new TeamBuffData();
        }

        foreach (TeamEnum team in _teamBuffData.Keys.ToList())
        {
            _teamBuffData[team] = new TeamBuffData();
        }
    }
}
