using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI2 : MonoBehaviour
{
    [SerializeField] private Button endTurnButton;
    [SerializeField] private TextMeshProUGUI turnNumberText;
    private void Start()
    {
        endTurnButton.onClick.AddListener(HandleNextTurnButton);
        UpdateTurnText();
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void TurnSystem_OnTurnChanged(object sender, System.EventArgs e)
    {
        UpdateTurnText();
    }

    private void HandleNextTurnButton()
    {
        TurnSystem.Instance.NextTurn();
    }

    private void UpdateTurnText()
    {
        turnNumberText.text = $"TURN {TurnSystem.Instance.GetTurnNumber()}";
    }
}
