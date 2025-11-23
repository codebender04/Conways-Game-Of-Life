using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPattern : MonoBehaviour
{
    [SerializeField] private Pattern pattern;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI patternName;
    [SerializeField] private GameObject patternSelectionPanel;
    private void OnValidate()
    {
        if (pattern != null)
        {
            patternName.text = pattern.name;
        }
    }
    private void Start()
    {
        button.onClick.AddListener(() =>
        {
            GameBoard.Instance.SetPattern(pattern);
            patternSelectionPanel.SetActive(false);
        });
    }
}
