using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelectionManager : Instancable<UISelectionManager>
{
    public ISelectableUI selectedUI;

    public void CloseCurrent()
    {
        if (selectedUI != null)
        {
            selectedUI.Close();
            selectedUI = null;
        }
    }

    public void Open(ISelectableUI _selectedUI)
    {
        if (selectedUI != null)
        {
            if (selectedUI == _selectedUI)
            {
                return;
            }

            CloseCurrent();
        }

        selectedUI = _selectedUI;
        selectedUI.Open();
    }
}
