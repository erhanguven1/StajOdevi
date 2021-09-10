using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Hexagon : MonoBehaviour
{
    public int id;

    public Vector3 hexPosition;
    private bool isEmpty;
    public bool IsEmpty
    {
        get => IsEmpty;
        set
        {
            isEmpty = value;
            HexagonSerializer.Instance.photonView.RPC("UpdateHexagonIsEmpty", Photon.Pun.RpcTarget.MasterClient, id, isEmpty);
        }
    }

    public ResourceType resourceType;

    [SerializeField] private Country ownerCountry;

    public Country OwnerCountry
    {
        get => ownerCountry;
        set
        {
            ownerCountry = value;
            OnUpdatedOwnerCountry();
            if (ownerCountry)
            {
                HexagonSerializer.Instance.photonView.RPC("UpdateHexagonOwnerCountry", Photon.Pun.RpcTarget.MasterClient, id, ownerCountry.id);

            }
        }
    }
    private bool isOnDestination;
    public bool IsOnDestination
    {
        get => isOnDestination;
        set
        {
            isOnDestination = value;
        }
    }

    public List<Hexagon> neighbors = new List<Hexagon>();

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public bool isCreatedFromFirst;

    private Image icon;

    private void Start()
    {
        tag = "Hexagon";
        hexPosition = transform.position;

        if (!isCreatedFromFirst)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

            GenerateMesh();

            gameObject.AddComponent<MeshCollider>();
        }
        meshRenderer.material = HexManager.instance.hexMaterial;

        GenerateNeighborHexagons();
        if (Photon.Pun.PhotonNetwork.IsMasterClient)
        {
            ResourcesGenerator.Instance.GenerateResource(this);
        }
    }

    public void SetIsEmptyManual(bool _isEmpty)
    {
        isEmpty = _isEmpty;
    }

    public void SetOwnerCountryManual(Country _ownerCountry)
    {
        ownerCountry = _ownerCountry;
        OnUpdatedOwnerCountry();
    }

    public void SetResourceType(ResourceType type)
    {
        resourceType = type;

        if (type != ResourceType.Empty)
        {
            IngameCanvasManager.Instance.AddIcon(ref icon, transform.position, ResourcesGenerator.Instance.textures[(int)type -1]);
        }
        else if (icon)
        {
            Destroy(icon.gameObject);
            icon = null;
        }
    }

    private void OnUpdatedOwnerCountry()
    {
        if (ownerCountry)
        {
            GetComponent<MeshRenderer>().material.color = ownerCountry.countryColor;
        }
    }

    public void GenerateNeighborHexagons()
    {

        for (int i = 0; i < 6; i++)
        {
            var hex = HexManager.instance.GenerateHexagon(transform.position + new Vector3(Mathf.Sin(Mathf.Deg2Rad * (i * 60)), 0, Mathf.Cos(Mathf.Deg2Rad * (i * 60))) * Mathf.Sqrt(3));
        }
    }
    private int[] Triangles =
    {
      6, 0, 1,
      6, 1, 2,
      6, 2, 3,
      6, 3, 4,
      6, 4, 5,
      6, 5, 0,

      13, 7, 8,
      13, 8, 9,
      13, 9, 10,
      13, 10, 11,
      13, 11, 12,
      13, 12, 7,

      0, 8, 7,
      0, 1, 8,
      1, 9, 8,
      1, 2, 9,
      2, 10, 9,
      2, 3, 10,
      3, 11, 10,
      3, 4, 11,
      4, 12, 11,
      4, 5, 12,
      5, 7, 12,
      5, 0, 7

   };
    public void GenerateMesh()
    {
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        Vector3[] vertices = new Vector3[14];

        for (int y = 0; y <= 7; y+=7)
        {
            vertices[0 + y] = new Vector3(-1, y/7, 0);
            vertices[1 + y] = new Vector3(-1 / 2f, y/7, Mathf.Sqrt(3) / 2f);
            vertices[2 + y] = new Vector3(1 / 2f, y/7, Mathf.Sqrt(3) / 2f);
            vertices[3 + y] = new Vector3(1, y/7, 0);
            vertices[4 + y] = new Vector3(1 / 2f, y/7, -Mathf.Sqrt(3) / 2f);
            vertices[5 + y] = new Vector3(-1 / 2f, y/7, -Mathf.Sqrt(3) / 2f);
            vertices[6 + y] = new Vector3(0, y/7, 0);
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();

        mesh.triangles = Triangles;
        mesh.RecalculateNormals();

        Vector2[] UVDirections = vertices.Select((v => new Vector2(v.x, v.z) + Vector2.one * 0.5f)).ToArray();


        var rotated = UVDirections;
        int i = 0;
        foreach (var item in UVDirections)
        {
            Vector2 source_uv = item;
            rotated[i] = Quaternion.AngleAxis(30f, Vector3.up) * source_uv;
            i++;
        }
        mesh.uv = rotated;
        mesh.RecalculateNormals();

        Quaternion rot = Quaternion.Euler(0, 0, 30);
        Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, rot, Vector3.one);
        GetComponent<Renderer>().material.SetMatrix("_TextureRotation", m);
    }

    private void OnDrawGizmos()
    {
        if (meshFilter)
        {
            foreach (var item in meshFilter.mesh.vertices)
            {
                Gizmos.DrawSphere(transform.position + item, .1f);
            }
        }
    }
}