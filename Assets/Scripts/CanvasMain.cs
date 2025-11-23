using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasMain : MonoBehaviour
{
    [SerializeField] private GameBoard gameBoard;

    [SerializeField] private Button runStopButton;
    [SerializeField] private TextMeshProUGUI runStopText;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button patternsButton;
    [SerializeField] private GameObject panelPatternSelection;
    private bool isRun = true;
    private void Awake()
    {
        runStopButton.onClick.AddListener(() =>
        {
            if (isRun)
            {
                gameBoard.StartSimulate();
                runStopText.text = "STOP";
            }
            else
            {
                gameBoard.StopSimulate();
                runStopText.text = "RUN";
            }
            isRun = !isRun;
        });
        resetButton.onClick.AddListener(() =>
        {
            gameBoard.ResetPattern();
        });
        nextButton.onClick.AddListener(() =>
        {
            gameBoard.UpdateNextState();
        });
        patternsButton.onClick.AddListener(() =>
        {
            if (panelPatternSelection.activeInHierarchy)
            {
                panelPatternSelection.SetActive(false);
            }
            else
            {
                panelPatternSelection.SetActive(true);
            }
        });
    }
}
