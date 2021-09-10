using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlayersUI : Instancable<PlayersUI>
{
    [SerializeField] private Button playerButtonPrefab;
    private List<Button> playerButtons = new List<Button>();

    [SerializeField] private Text dominationText;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.onAddedCountry += OnAddedCountry;
    }

    private void OnAddedCountry(int countryId, bool isMine)
    {
        var btn = Instantiate(playerButtonPrefab, transform);
        string text = countryId.ToString();


        if (isMine)
        {
            playerButtons.Insert(0, btn);
            playerButtons[0].transform.SetAsFirstSibling();
            text += "\n me";
        }
        else
        {
            playerButtons.Add(btn);
        }

        btn.onClick.AddListener(() => OnTapPlayerButton(GameManager.Instance.countries.FirstOrDefault(x => x.id == countryId)));

        btn.transform.GetChild(0).GetComponent<Text>().text = text;
    }

    public void RemoveCountry(int countryId)
    {
        var buttonToRemove = countryId == GameManager.Instance.myCountry.id ? playerButtons[0] : playerButtons.FirstOrDefault(x => x.transform.GetChild(0).GetComponent<Text>().text == countryId.ToString());
        playerButtons.Remove(buttonToRemove);
        Destroy(buttonToRemove.gameObject);
    }

    public void UpdateUI(int countryId)
    {
        var btn = playerButtons.FirstOrDefault(x => x.transform.GetChild(0).GetComponent<Text>().text == countryId.ToString());
        btn.transform.GetChild(0).GetComponent<Text>().text = countryId.ToString();
    }

    public void OnTapPlayerButton(Country country)
    {
        dominationText.text = country.domination.ToString();
    }
}
