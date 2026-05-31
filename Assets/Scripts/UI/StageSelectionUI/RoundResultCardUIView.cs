using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundResultCardUIView : MonoBehaviour
{
    [SerializeField] private TMP_Text _roundNumberText;
    [SerializeField] private GameObject _roundMarker;
    [SerializeField] private Image _roundDefaultImage;
    [SerializeField] private Image _roundWinImage;
    [SerializeField] private Image _roundDrawImage;
    [SerializeField] private Image _roundLoseImage;

    public int RoundIndex { get; private set; }

    public void Initialize(int roundIndex)
    {
        RoundIndex = roundIndex;
        ResetRoundResult();
        SetRoundNumberUIText();
    }

    private void SetRoundNumberUIText()
    {
        _roundNumberText.text = (RoundIndex + 1).ToString();
    }

    public void SetRoundResultImage(RoundResultEnum roundResult)
    {
        ResetRoundResult();

        switch (roundResult)
        {
            case RoundResultEnum.Win:
                _roundWinImage.gameObject.SetActive(true);
                break;
            case RoundResultEnum.Lose:
                _roundLoseImage.gameObject.SetActive(true);
                break;
            case RoundResultEnum.Draw:
                _roundDrawImage.gameObject.SetActive(true);
                break;
        }
    }

    public void ResetRoundResult()
    {
        _roundDefaultImage.gameObject.SetActive(true);
        _roundWinImage.gameObject.SetActive(false);
        _roundLoseImage.gameObject.SetActive(false);
        _roundDrawImage.gameObject.SetActive(false);
    }

    public void ToggleRoundMarker(bool value)
    {
        _roundMarker.SetActive(value);
    }
}
