using AutoBattler.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamService
{
    private readonly Dictionary<TeamEnum, List<UnitData>> _teams = new();
    private readonly Dictionary<TeamEnum, List<UnitData>> _inventoryUnits = new();
    private readonly Dictionary<TeamEnum, List<BaseUnit>> _fieldUnits = new();

    private Dictionary<TeamEnum, int> _teamCapacities;
    private readonly Dictionary<TeamEnum, int> _fieldCapacities;

    private readonly Dictionary<TeamEnum, Dictionary<UnitTypeEnum, int>> _typeCount = new();
    private readonly Dictionary<TeamEnum, Dictionary<UnitFactionEnum, int>> _factionCount = new();

    public TeamService(int defaultTeamCapacity = 8, int defaultFieldCapacity = 1)
    {
        SubscribeToEvents();

        _teamCapacities = new Dictionary<TeamEnum, int>();
        _fieldCapacities = new Dictionary<TeamEnum, int>();

        foreach (TeamEnum team in Enum.GetValues(typeof(TeamEnum)))
        {
            _teams[team] = new List<UnitData>();
            _inventoryUnits[team] = new List<UnitData>();
            _fieldUnits[team] = new List<BaseUnit>();

            _teamCapacities[team] = defaultTeamCapacity;
            _fieldCapacities[team] = defaultFieldCapacity;

            InitializeTypeAndFactionCounts();
        }
    }
    ~TeamService()
    {
        UnsubscribeToEvents();
    }

    void SubscribeToEvents()
    {
        EventBusManager.Instance.Subscribe(EventNameEnum.LevelChanged, OnLevelChanged_Team);
    }

    void UnsubscribeToEvents()
    {
        EventBusManager.Instance.Unsubscribe(EventNameEnum.LevelChanged, OnLevelChanged_Team);
    }

    public bool AddUnitToTeam(UnitData unit, TeamEnum team)
    {
        if (_teams[team].Count >= _teamCapacities[team]) return false;

        _teams[team].Add(unit);
        _inventoryUnits[team].Add(unit);
        return true;
    }

    public bool RemoveUnitFromTeam(UnitData unit, TeamEnum team)
    {
        return _teams[team].Remove(unit);
    }

    public bool RemoveUnitFromField(BaseUnit unit, TeamEnum team)
    {
        _fieldUnits[team].Remove(unit);
        RemoveUnitCount(unit, team);
        return RemoveUnitFromTeam(unit.UnitData, team);
    }

    public bool RemoveUnitFromInventory(UnitData unit, TeamEnum team)
    {
        _inventoryUnits[team].Remove(unit);
        return RemoveUnitFromTeam(unit, team);
    }

    public IReadOnlyList<UnitData> GetTeamUnits(TeamEnum team) => _teams[team].AsReadOnly();

    public IReadOnlyList<UnitData> GetInventoryUnits(TeamEnum team) => _inventoryUnits[team].AsReadOnly();

    public IReadOnlyList<BaseUnit> GetFieldUnits(TeamEnum team) => _fieldUnits[team].AsReadOnly();

    public bool MoveToField(BaseUnit unit, TeamEnum team)
    {
        if (!_inventoryUnits[team].Contains(unit.UnitData)) return false;
        if (_fieldUnits[team].Count >= _fieldCapacities[team]) return false;

        _fieldUnits[team].Add(unit);
        _inventoryUnits[team].Remove(unit.UnitData);
        AddUnitCount(unit, team);
        return true;
    }

    public bool MoveToInventory(BaseUnit unit, TeamEnum team)
    {
        if (!_fieldUnits[team].Contains(unit)) return false;

        _fieldUnits[team].Remove(unit);
        RemoveUnitCount(unit, team);
        _inventoryUnits[team].Add(unit.UnitData);
        return true;
    }

    public void ResetFieldUnits(TeamEnum team)
    {
        var units = _fieldUnits[team].ToList();
        foreach (var unit in units)
        {
            MoveToInventory(unit, team);
        }
    }

    public int GetTeamCapacity(TeamEnum team) => _teamCapacities[team];
    public int GetFieldCapacity(TeamEnum team) => _fieldCapacities[team];

    public void SetTeamCapacity(TeamEnum team, int capacity) => _teamCapacities[team] = capacity;
    public void SetFieldCapacity(TeamEnum team, int capacity) => _fieldCapacities[team] = capacity;

    public int GetTeamUnitsCount(TeamEnum team) => _teams[team].Count;
    public int GetFieldUnitsCount(TeamEnum team) => _fieldUnits[team].Count;

    public bool CanAddUnitToField(TeamEnum team)
    {
        return _fieldUnits[team].Count < _fieldCapacities[team];
    }

    private void InitializeTypeAndFactionCounts()
    {
        foreach (TeamEnum team in Enum.GetValues(typeof(TeamEnum)))
        {
            _typeCount[team] = new Dictionary<UnitTypeEnum, int>();
            _factionCount[team] = new Dictionary<UnitFactionEnum, int>();

            foreach (UnitTypeEnum type in Enum.GetValues(typeof(UnitTypeEnum)))
                _typeCount[team][type] = 0;

            foreach (UnitFactionEnum faction in Enum.GetValues(typeof(UnitFactionEnum)))
                _factionCount[team][faction] = 0;
        }
    }

    private void AddUnitCount(BaseUnit unit, TeamEnum team)
    {
        var type = unit.UnitData.unitType;
        var faction = unit.UnitData.unitFaction;

        _typeCount[team][type]++;
        _factionCount[team][faction]++;

        EventBusManager.Instance.Raise(EventNameEnum.UnitAddedOnField, team, type, faction); 
    }

    private void RemoveUnitCount(BaseUnit unit, TeamEnum team)
    {
        var type = unit.UnitData.unitType;
        var faction = unit.UnitData.unitFaction;

        _typeCount[team][type]--;
        _factionCount[team][faction]--;

        EventBusManager.Instance.Raise(EventNameEnum.UnitRemovedFromField, team, type, faction);
    }

    public int GetTypeCount(TeamEnum team, UnitTypeEnum type)
    {
        return _typeCount[team][type];
    }

    public int GetFactionCount(TeamEnum team, UnitFactionEnum faction)
    {
        return _factionCount[team][faction];
    }

    private void OnLevelChanged_Team(object[] parameters)
    {
        int newFieldCapacity = (int)parameters[1];
        SetFieldCapacity(TeamEnum.Team1, newFieldCapacity);
    }
}
