using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : Instancable<SelectionManager>
{
    public static ISelectable selected;

    private void Start()
    {
        TurnManager.onNewTurnStarted += NewTurn;
    }

    private void NewTurn()
    {
        if (selected == null)
        {
            return;
        }
        Deselect();
    }

    void Select(ISelectable _selectable)
    {
        selected = _selectable;
        selected.Select();
        ResearchUI.Instance.Close();
    }

    public void Deselect()
    {
        selected.Deselect();
        selected = null;
        ResearchUI.Instance.Close();
    }

    // Update is called once per frame
    void Update()
    {

        if (GameManager.Instance.myCountry == null || GameManager.Instance.myCountry.turnHasEnded)
        {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && Input.GetMouseButtonDown(0))
        {
            if (hit.collider.GetComponentInParent<ISelectable>() != null)
            {
                if (selected != null)
                {
                    //If we changed the selected by clicking on another selectable object, deselect it and select new one
                    if (selected != hit.collider.GetComponentInParent<ISelectable>())
                    {
                        Deselect();
                        Select(hit.collider.GetComponentInParent<ISelectable>());
                    }
                    else
                    {
                        Select(hit.collider.GetComponentInParent<ISelectable>());
                    }
                }
                else
                {
                    Select(hit.collider.GetComponentInParent<ISelectable>());
                }
            }
            else
            {
                if (selected != null)
                {
                    Deselect();
                }
            }
        }
    }
}
