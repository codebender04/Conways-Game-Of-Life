using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PatternTypeTabs : MonoBehaviour
{
    [SerializeField] private Button[] buttonTabArray;
    [SerializeField] private ButtonTabFocus buttonTabFocus;
    [SerializeField] private GameObject[] panelArray;
    private void Awake()
    {
        for (int i = 0; i < buttonTabArray.Length; i++)
        {
            int index = i;
            buttonTabArray[i].onClick.AddListener(() =>
            {
                CloseAllPanels();
                SetActiveAllNavButtons();
                buttonTabArray[index].gameObject.SetActive(false);
                panelArray[index].SetActive(true);
                string text = buttonTabArray[index].GetComponent<PatternTypeName>().FullPatternTypeName;
                buttonTabFocus.SetFocusButton(text);
                buttonTabFocus.transform.SetSiblingIndex(index);
                Canvas.ForceUpdateCanvases();
            });
        }
    }
    private void CloseAllPanels()
    {
        foreach (GameObject panel in panelArray)
        {
            panel.SetActive(false);
        }
    }
    private void SetActiveAllNavButtons()
    {
        foreach (Button buttonTab in buttonTabArray)
        {
            buttonTab.gameObject.SetActive(true);
        }
    }
}
