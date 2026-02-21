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

    public TeamService(int defaultTeamCapacity = 8, int defaultFieldCapacity = 4)
    {
        _teamCapacities = new Dictionary<TeamEnum, int>();
        _fieldCapacities = new Dictionary<TeamEnum, int>();

        foreach (TeamEnum team in Enum.GetValues(typeof(TeamEnum)))
        {
            _teams[team] = new List<UnitData>();
            _inventoryUnits[team] = new List<UnitData>();
            _fieldUnits[team] = new List<BaseUnit>();

            _teamCapacities[team] = defaultTeamCapacity;
            _fieldCapacities[team] = defaultFieldCapacity;
        }

        Debug.Log(_fieldCapacities[TeamEnum.Team1] + "---" + _fieldCapacities[TeamEnum.Team2]);
    }

    public bool AddUnitToTeam(UnitData unit, TeamEnum team)
    {
        var teamList = _teams[team];
        if (teamList.Contains(unit)) return false;
        if (teamList.Count >= _teamCapacities[team]) return false;

        teamList.Add(unit);

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

        _inventoryUnits[team].Remove(unit.UnitData);
        _fieldUnits[team].Add(unit);
        return true;
    }

    public bool MoveToInventory(BaseUnit unit, TeamEnum team)
    {
        if (!_fieldUnits[team].Contains(unit)) return false;

        _fieldUnits[team].Remove(unit);
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

    public bool CanAddUnitToField(TeamEnum team)
    {
        return _fieldUnits[team].Count < _fieldCapacities[team];
    }
}
