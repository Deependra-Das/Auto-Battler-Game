using System.Collections.Generic;
using System.Linq;
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
        //_buffIcon.sprite = buffData.buffImage.sprite;

        for(int num = 0; num < buffData.buffParticipantTierList.Length; num++)
        {
            _buffParticipantsTextList[num].text = buffData.buffParticipantTierList[num].participants.ToString();
            //_buffParticipantsBlockImageList[num].sprite = _deactivatedBlockImage.sprite;
            _buffParticipantsBlockImageList[num].color = Color.lightGray;
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
            //_buffParticipantsBlockImageList[index].sprite = _activatedBlockImage.sprite;
            _buffParticipantsBlockImageList[index].color = Color.white;
            _activatedParticipantBlock = index;
        }
    }

    private void DeactivateParticipantBlock()
    {
        if (_activatedParticipantBlock >= 0 && _activatedParticipantBlock < _buffParticipantsBlockImageList.Count)
        {
            //_buffParticipantsBlockImageList[_activatedParticipantBlock].sprite = _deactivatedBlockImage.sprite;
            _buffParticipantsBlockImageList[_activatedParticipantBlock].color = Color.lightGray;
        }

        _activatedParticipantBlock = -1;
    }
}
