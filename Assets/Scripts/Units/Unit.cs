using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public enum UnitType { Settler, Builder, Warrior }

public abstract class Unit : MonoBehaviourPun, ISelectable, IPunObservable
{
    public bool isSelected;
    public GameObject isSelectedObject;

    public int movement;
    public UnitType unitType;
    public int health;
    public int damage;

    public Country country;
    public bool canAttack;

    public int movementLeft;
    public int healthLeft;

    public bool traveling;
    private Vector3 destination;

    public int id;

    private Unit targetUnit;

    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            this.enabled = false;
        }
        else
        {
            SetMaterialColor();
        }

        SetConfigs();
        movementLeft = movement;
        healthLeft = health;

        TurnManager.onNewTurnStarted += NewTurn;
    }

    void SetMaterialColor()
    {
        GetComponentInChildren<MeshRenderer>().material.color = country.countryColor;
    }

    public abstract void SetConfigs();

    public void NewTurn()
    {
        movementLeft = movement;
        canAttack = true;
    }

    public void SetSelected(bool _selected)
    {
        isSelected = _selected;
        isSelectedObject.SetActive(_selected);
    }

    public void Update()
    {
        if (!isSelected || country.turnHasEnded)
        {
            return;
        }
        Raycast();
    }
    public void Raycast()
    {

        if (Input.GetMouseButtonDown(1) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            if (hit.collider.tag == "Hexagon")
            {
                SetDestination(hit.collider.GetComponent<Hexagon>());
            }
            //Attack if it's our enemy
            else if (canAttack && hit.collider.tag == "Unit" && hit.collider.GetComponentInParent<Unit>().country != country)
            {
                var K = HexManager.instance.GetBestPathToDestination(transform.position - Vector3.up * .5f, hit.collider.transform.position - Vector3.up * 1.5f);
                targetUnit = hit.collider.GetComponentInParent<Unit>();
                if (K.Length == 1)
                {
                    Attack(targetUnit);
                }
                else
                {
                    SetDestination(hit.collider.transform.position - Vector3.up * 1.5f, true);
                }
            }
        }
    }
    public void SetDestination(Hexagon hex)
    {
        SetDestination(hex.hexPosition);
    }
    public void SetDestination(Vector3 pos, bool thenAttack = false)
    {
        if (traveling)
        {
            return;
        }

        destination = pos + Vector3.up * .5f;

        var path = HexManager.instance.GetBestPathToDestination(transform.position - Vector3.up * .5f, pos).ToList();

        if (thenAttack)
        {
            path.Remove(path.Last());
        }

        if (path.Count > movementLeft)
        {
            EndTravel();
            return;
        }

        HexManager.instance.pathLine.positionCount = path.Count + 1;
        for (int i = -1; i < path.Count; i++)
        {
            if (i == -1)
            {
                HexManager.instance.pathLine.SetPosition(0, transform.position);
            }
            else
            {
                HexManager.instance.pathLine.SetPosition(i + 1, path[i] + Vector3.up * 1.5f);
            }

        }

        StartCoroutine(Travel());
        movementLeft -= path.Count;
        UpdateUnitUI();

        IEnumerator Travel()
        {
            if (path.Count > 0)
            {
                traveling = true;

                destination = path.Last();
                int i = 0;
                while (Vector3.Distance(transform.position, destination + Vector3.up * 1.5f) > .2f)
                {
                    var nextTarget = path[i];
                    Vector3 dir = (nextTarget - transform.position).normalized;
                    while (Vector3.Distance(transform.position, nextTarget + Vector3.up * 1.5f) > .2f)
                    {
                        transform.position += dir * 7.5f * Time.fixedDeltaTime;
                        transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);
                        yield return new WaitForFixedUpdate();
                    }
                    i++;
                }
            }
            transform.position = destination + Vector3.up * 1.5f;
            EndTravel(thenAttack);
        }
    }

    public void Attack(Unit target)
    {
        canAttack = false;
        movementLeft = 0;

        //Both units takes damage after attack
        photonView.RPC("TakeDamage", RpcTarget.AllBuffered, target.damage);

        target.photonView.RPC("TakeDamage", RpcTarget.AllBuffered, this.damage);

        if (target.healthLeft <= 0)
        {
            SetDestination(target.transform.position - Vector3.up * .5f);
        }

        UpdateUnitUI();
    }

    private void UpdateUnitUI()
    {
        if (UnitUI.Instance.unit == this)
        {
            UnitUI.Instance.UpdateUnitUI();
        }
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        healthLeft -= damage;
        if (photonView.IsMine && healthLeft <= 0)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        UpdateUnitUI();
    }

    public void EndTravel(bool thenAttack = false)
    {
        if (thenAttack)
        {
            Attack(targetUnit);
        }
        targetUnit = null;
        HexManager.instance.activePath.Clear();
        HexManager.instance.pathLine.positionCount = 0;
        traveling = false;
    }

    private void OnDestroy()
    {
        if (SelectionManager.selected == GetComponent<ISelectable>())
        {
            Deselect();
            SelectionManager.selected = null;
        }
    }

    public void Select()
    {
        isSelected = true;
        isSelectedObject.SetActive(true);
        UnitUI.Instance.SetUnit(this);
        UISelectionManager.Instance.Open(UnitUI.Instance.GetComponent<ISelectableUI>());
        CameraController.Instance.MoveTo(gameObject);
    }

    public void Deselect()
    {
        isSelected = false;
        isSelectedObject.SetActive(false);
        UISelectionManager.Instance.CloseCurrent();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);

            stream.SendNext(country.id);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();

            id = (int)stream.ReceiveNext();
            if (id < GameManager.Instance.countries.Count)
            {
                country = GameManager.Instance.countries[id];
                SetMaterialColor();
            }
        }
    }
}
