using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchUI : Instancable<ResearchUI>, ISelectableUI
{
    public GameObject panel;
    public Button technologyButtonPrefab;

    private List<TechnologyButton> technologyButtons = new List<TechnologyButton>();
    private bool isPanelActive;

    public void OnClickResearchButton()
    {
        if (isPanelActive)
        {
            UISelectionManager.Instance.CloseCurrent();
        }
        else
        {
            UISelectionManager.Instance.Open(GetComponent<ISelectableUI>());
        }
    }

    public void Open()
    {
        isPanelActive = true;
        panel.gameObject.SetActive(true);
        FillUI();
    }

    private void FillUI()
    {
        if (ResearchManager.Instance.research.technologies.Count > 0)
        {
            foreach (var item in ResearchManager.Instance.research.technologies)
            {
                var o = Instantiate(technologyButtonPrefab);
                o.transform.GetChild(0).GetComponent<Text>().text = item.Key.ToString();
                o.transform.SetParent(panel.transform, false);

                var researchButton = new TechnologyButton();
                researchButton.InitializeButton(item.Key, o);

                technologyButtons.Add(researchButton);

                researchButton.onClick += OnTapTechnologyButton;

            }
        }
    }

    public void Close()
    {
        isPanelActive = false;

        if (technologyButtons.Count > 0)
        {
            foreach (var item in technologyButtons)
            {
                item.onClick -= OnTapTechnologyButton;
                Destroy(item.myButton.gameObject);
            }
            technologyButtons.Clear();
        }

        panel.gameObject.SetActive(false);
    }

    private void OnTapTechnologyButton(TechnologyButton item)
    {
        Debug.Log("Tapped ");
        ResearchManager.Instance.OnTapTechnologyButton(item.type);
    }
}

public class TechnologyButton
{
    public TechnologyType type;

    public Button myButton;

    public delegate void OnClick(TechnologyButton item);
    public event OnClick onClick;

    public void InitializeButton(TechnologyType _type, Button btn)
    {
        type = _type;
        myButton = btn;
        myButton.onClick.AddListener(OnTap);
    }

    public void OnTap()
    {
        if (onClick != null)
        {
            onClick(this);
        }
    }
}