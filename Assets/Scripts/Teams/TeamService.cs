using System.Collections.Generic;
using UnityEngine;

public class TeamService
{
    private Dictionary<TeamEnum, List<BaseUnit>> _teams = new Dictionary<TeamEnum, List<BaseUnit>>();

    private Dictionary<TeamEnum, int> _teamCapacities;

    public int GetTeamCapacity(TeamEnum team) => _teamCapacities[team];

    public TeamService()
    {
        _teams.Add(TeamEnum.Team1, new List<BaseUnit>());
        _teams.Add(TeamEnum.Team2, new List<BaseUnit>());
        _teamCapacities = new Dictionary<TeamEnum, int> {{ TeamEnum.Team1, 1 },{ TeamEnum.Team2, 1 }};
    }

    public bool AddUnitToTeam(BaseUnit unit, TeamEnum team)
    {
        var teamList = _teams[team];

        if (teamList.Count >= _teamCapacities[team])
            return false;

        teamList.Add(unit);
        return true;
    }

    public bool RemoveUnitFromTeam(BaseUnit unit, TeamEnum team)
    {
        return _teams[team].Remove(unit);
    }
}
