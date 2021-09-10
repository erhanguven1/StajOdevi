using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Settler : Unit
{
    public override void SetConfigs()
    {
        unitType = UnitType.Settler;
        movement = 2;
        health = 3;
        damage = 0;
    }

    // Update is called once per frame
    public void Update()
    {
        base.Update();

        if (!isSelected)
        {
            return;
        }

        if (photonView.IsMine && Input.GetKeyDown(KeyCode.S))
        {
            Vector3 hexPos = transform.position;
            hexPos.x = (float)System.Math.Round(hexPos.x * 1000f) / 1000f;
            hexPos.z = (float)System.Math.Round(hexPos.z * 1000f) / 1000f;
            hexPos.y = 0;

            Hexagon centerHexagon;

            if (HexManager.instance.allHexagons.TryGetValue(hexPos, out centerHexagon))
            {
                var cityGO = PhotonNetwork.Instantiate("city", centerHexagon.transform.position + Vector3.up * 1.5f, Quaternion.identity);
                cityGO.gameObject.SetActive(false);
                CreateCity(cityGO);
            }
        }
    }

    public void CreateCity(GameObject cityGO)
    {
        StartCoroutine(wait());
        IEnumerator wait()
        {
            yield return new WaitForSeconds(.2f);

            Vector3 hexPos = transform.position;
            hexPos.x = (float)System.Math.Round(hexPos.x * 1000f) / 1000f;
            hexPos.z = (float)System.Math.Round(hexPos.z * 1000f) / 1000f;
            hexPos.y = 0;

            Hexagon centerHexagon;

            if (HexManager.instance.allHexagons.TryGetValue(hexPos, out centerHexagon))
            {
                City city = null;
                if (centerHexagon.OwnerCountry == null)
                {
                    cityGO.gameObject.SetActive(true);
                    city = cityGO.GetComponent<City>();
                    city.InitializeCity(this.country);
                    city.AddHexagon(centerHexagon);
                }
                else
                {
                    PhotonNetwork.Destroy(cityGO);
                    yield break;
                }

                var neighbors = HexManager.instance.GetNeighborHexagons(hexPos);

                foreach (var item in neighbors)
                {
                    if (item.OwnerCountry == null)
                    {
                        city.AddHexagon(item);
                    }
                }

                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

}
