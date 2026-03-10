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
    }

    public void InitializeBuffs()
    {
        foreach(var buffs in _buffData)
        {
            UIManager.Instance.AddBuffDetailUICard(buffs.Value);
            _appliedBuffs.Add(buffs.Key, 0);
        }
    }

    public void ResetBuffs()
    {
        _appliedBuffs.Clear();
    }

    private void UpdateAppliedBuffs()
    {

    }
}
