using AutoBattler.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BuffDetailsUICard : MonoBehaviour
{
    [SerializeField] private TMP_Text _buffNameText;
    [SerializeField] private Image _buffIcon;
    [SerializeField] private Image _buffIconContainer;
    [SerializeField] private List<TMP_Text> _buffParticipantsTextList;
    [SerializeField] private List<Image> _buffParticipantsBlockImageList;
    [SerializeField] private Color _color1;
    [SerializeField] private Color _color2;

    private int _activatedParticipantBlock = -1;
    private BuffData _buffData;
    private IconService _unitIconServiceObj;

    public void Initialize(BuffData buffData)
    {
        _unitIconServiceObj = GameManager.Instance.Get<IconService>();

        _buffData = buffData;
        _buffNameText.text = buffData.buffName.ToString();
        _buffIcon.sprite = _unitIconServiceObj.GetBuffIcon(_buffData.buffName);
        for (int num = 0; num < buffData.buffParticipantTierList.Length; num++)
        {
            _buffParticipantsTextList[num].text = buffData.buffParticipantTierList[num].participants.ToString();
            _buffParticipantsBlockImageList[num].color = _color2;
            _buffParticipantsTextList[num].color = _color1;
        }
    }

    public void ActivateParticipantBlock(int participants)
    {
        DeactivateParticipantBlock();

        int index = _buffData.buffParticipantTierList
            .Select((tier, i) => new { tier, i })
            .Where(x => x.tier.participants <= participants)
            .Select(x => x.i)
            .DefaultIfEmpty(-1)
            .Max();

        if (index != -1)
        {
            _buffParticipantsBlockImageList[index].color = _color1;
            _buffParticipantsTextList[index].color = _color2;
            _activatedParticipantBlock = index;
        }
    }

    public void DeactivateParticipantBlock()
    {
        if (_activatedParticipantBlock >= 0 && _activatedParticipantBlock < _buffParticipantsBlockImageList.Count)
        {
            _buffParticipantsBlockImageList[_activatedParticipantBlock].color = _color2;
            _buffParticipantsTextList[_activatedParticipantBlock].color = _color1;
        }

        _activatedParticipantBlock = -1;
    }
}
