using System.Collections.Generic;
using UnityEngine;

public class BuffService
{
    private Dictionary<BuffNameEnum, BuffData> _buffData = new();
    private Dictionary<BuffNameEnum, int> _appliedBuffs = new();

    public BuffService(BuffScriptableObjectScript buff_SO)
    {
        foreach(var buffs in buff_SO.buffData)
        {
            _buffData.Add(buffs.buffName, buffs);
        }

        InitializeAppliedBuffs();
    }

    private void InitializeAppliedBuffs()
    {
        foreach(var buffs in _buffData)
        {
            _appliedBuffs.Add(buffs.Key, 0);
        }
    }

    private void UpdateAppliedBuffs()
    {

    }
}
