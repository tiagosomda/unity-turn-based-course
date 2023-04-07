using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] Transform actionButtonPrefab;
    [SerializeField] Transform actionButtonContainerTransform;
    [SerializeField] TextMeshProUGUI actionPointsText;
    
    private List<ActionButtonUI> actionButtonUIList;

    private void Awake()
    {
        actionButtonUIList= new List<ActionButtonUI>();
    }
    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        Unit.OnAnyActionPointsChange += Unit_OnAnyActionPointsChange;
        CreateUnitActionButtons();
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, System.EventArgs e)
    {
        UpdateSelectedVisuals();
    }

    private void UnitActionSystem_OnActionStarted(object sender, System.EventArgs e)
    {
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, System.EventArgs e)
    {
        CreateUnitActionButtons();
        UpdateSelectedVisuals();
        UpdateActionPoints();
    }

    private void CreateUnitActionButtons()
    {
        foreach(Transform buttonTransform in actionButtonContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }

        actionButtonUIList.Clear();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        if (selectedUnit != null)
        {
            foreach (BaseAction baseAction in selectedUnit.GetBaseActionArray())
            {
                Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
                ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
                actionButtonUI.SetBaseAction(baseAction);

                actionButtonUIList.Add(actionButtonUI);
            }
        }
    }

    private void UpdateSelectedVisuals()
    {
        foreach (ActionButtonUI actionButtonUI in actionButtonUIList)
        {
            actionButtonUI.UpdateSelectedVisual();
        }
    }

    private void UpdateActionPoints()
    {
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        actionPointsText.text = $"Action Points: {selectedUnit.GetActionPoints()}";
    }

    private void TurnSystem_OnTurnChanged(object sender, System.EventArgs e)
    {
        UpdateActionPoints();
    }

    private void Unit_OnAnyActionPointsChange(object sender, System.EventArgs e)
    {
        UpdateActionPoints();
    }

}
