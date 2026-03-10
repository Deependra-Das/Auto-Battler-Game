using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffDetailsUICard : MonoBehaviour
{
    [SerializeField] private TMP_Text _buffNameText;
    [SerializeField] private Image _buffIcon;
    [SerializeField] private Image _activatedBlockImage;
    [SerializeField] private Image _deactivatedBlockImage;
    [SerializeField] private List<TMP_Text> _buffParticipantsTextList;
    [SerializeField] private List<Image> _buffParticipantsBlockImageList;

    private int _activatedParticipantBlock = -1;
    private BuffData _buffData;

    public void Initialize(BuffData buffData)
    {
        _buffData = buffData;
        _buffNameText.text = buffData.buffName.ToString();
        _buffIcon = buffData.buffImage;

        for(int num = 0; num < buffData.buffParticipants.Length; num++)
        {
            _buffParticipantsTextList[num].text = buffData.buffParticipants[num].ToString();
            _buffParticipantsBlockImageList[num] = _deactivatedBlockImage;
        }
    }

    public void ActivateParticipantBlock(int participants)
    {
        DeactivateParticipantBlock();

        for (int num = 0; num < _buffData.buffParticipants[0]; num++)
        {
            if (participants > _buffData.buffParticipants[0])
            {
                _buffParticipantsBlockImageList[num] = _activatedBlockImage;
            }
        }
    }

    private void DeactivateParticipantBlock()
    {
        if (_activatedParticipantBlock > -1)
        {
            _buffParticipantsBlockImageList[_activatedParticipantBlock] = _deactivatedBlockImage;
        }
    }
}
