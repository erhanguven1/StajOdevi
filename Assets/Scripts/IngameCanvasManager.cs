using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameCanvasManager : Instancable<IngameCanvasManager>
{
    private Canvas ingameCanvas;
    public Image iconPrefab;

    // Start is called before the first frame update
    void Start()
    {
        ingameCanvas = GetComponent<Canvas>();
        ingameCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(HexManager.instance.size.x, HexManager.instance.size.y) * 2;
    }

    public void AddIcon(ref Image icon, Vector3 pos, Sprite spr)
    {
        if (icon)
        {
            Destroy(icon.gameObject);
            icon = null;
        }
        icon = Instantiate(iconPrefab, ingameCanvas.GetComponent<RectTransform>(), false);
        icon.sprite = spr;
        icon.transform.position = pos;
        icon.GetComponent<RectTransform>().localPosition = new Vector3(icon.GetComponent<RectTransform>().localPosition.x, icon.GetComponent<RectTransform>().localPosition.y, 1.1f);
    }
}
