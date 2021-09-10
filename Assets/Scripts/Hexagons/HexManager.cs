using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventList<T> : List<T>
{
    public new void Add(T item)
    {
        base.Add(item);
        (item as Hexagon).IsOnDestination = true;
    }

    public new void Clear()
    {
        foreach (var item in this)
        {
            (item as Hexagon).IsOnDestination = false;
        }
        base.Clear();
    }
}

public class HexManager : MonoBehaviour
{
    public static HexManager instance;

    private void Awake()
    {
        instance = this;
    }

    public int maxHexagonCount => size.x * size.y;
    public int currentHexagonCount;
    public int K;
    public Vector2Int size;

    private GameObject hexagonPrefab;
    public Dictionary<Vector3, Hexagon> allHexagons = new Dictionary<Vector3, Hexagon>();
    public Material hexMaterial;

    public LineRenderer pathLine;
    public EventList<Hexagon> activePath = new EventList<Hexagon>();


    public void Initialize()
    {
        GenerateHexagon(Vector3.zero);
        StartCoroutine(calculateNeighbors());
        IEnumerator calculateNeighbors()
        {
            yield return new WaitForEndOfFrame();
            foreach (var item in allHexagons)
            {
                item.Value.neighbors.AddRange(GetNeighborHexagons(item.Value.hexPosition));
            }
        }
    }

    public Hexagon GenerateHexagon(Vector3 hexPositon)
    {
        hexPositon.x = (float)System.Math.Round(hexPositon.x * 1000f) / 1000f;
        hexPositon.z = (float)System.Math.Round(hexPositon.z * 1000f) / 1000f;

        if (Mathf.Abs(hexPositon.x) > size.x || Mathf.Abs(hexPositon.z) > size.y || currentHexagonCount > 2000)
        {
            return null;
        }

        if (!allHexagons.ContainsKey(hexPositon))
        {
            GameObject hexObject = null;

            if (hexagonPrefab == null)
            {
                hexObject = new GameObject("hex");
                hexObject.AddComponent<Hexagon>();
                hexagonPrefab = hexObject;
            }
            else
            {
                hexagonPrefab.GetComponent<Hexagon>().isCreatedFromFirst = true;
                hexObject = Instantiate(hexagonPrefab).gameObject;
            }

            hexObject.transform.position = hexPositon;
            hexObject.transform.parent = transform;

            if (hexObject != null && hexObject.GetComponent<Hexagon>())
            {
                currentHexagonCount++;
                allHexagons.Add(hexPositon, hexObject.GetComponent<Hexagon>());
                hexObject.GetComponent<Hexagon>().id = allHexagons.Count - 1;

                return hexObject.GetComponent<Hexagon>();
            }
        }
        return null;
    }

    public Hexagon[] GetNeighborHexagons(Vector3 centerHexagon, bool getOnlyNonCountryHexagons = false)
    {
        List<Hexagon> neighbors = new List<Hexagon>();

        for (int i = 0; i < 6; i++)
        {
            Hexagon hexToAdd;

            Vector3 hexPosition = centerHexagon + new Vector3(Mathf.Sin(Mathf.Deg2Rad * (i * 60)), 0, Mathf.Cos(Mathf.Deg2Rad * (i * 60))) * Mathf.Sqrt(3);
            hexPosition.x = (float)System.Math.Round(hexPosition.x * 1000f) / 1000f;
            hexPosition.z = (float)System.Math.Round(hexPosition.z * 1000f) / 1000f;

            if (allHexagons.TryGetValue(hexPosition, out hexToAdd))
            {
                if (getOnlyNonCountryHexagons)
                {
                    if (hexToAdd.OwnerCountry == null)
                    {
                        neighbors.Add(hexToAdd);
                    }
                }
                else
                {
                    neighbors.Add(hexToAdd);
                }
            }
        }

        return neighbors.ToArray();
    }

    public Vector3[] GetBestPathToDestination(Vector3 A, Vector3 B)
    {
        List<Vector3> path = new List<Vector3>();
        List<Hexagon> hexagons = new List<Hexagon>();

        Ray ray = new Ray(A, B - A);
        var hits = Physics.RaycastAll(ray, (B - A).magnitude);

        foreach (var item in hits)
        {
            if (item.collider.tag == "Hexagon")
            {
                item.collider.GetComponent<Hexagon>().IsOnDestination = true;
                path.Add(item.transform.position);
                hexagons.Add(item.collider.GetComponent<Hexagon>());
            }
        }
        path = SimplifyPath(hexagons, A);

        return path.ToArray();
    }

    public List<Vector3> SimplifyPath(List<Hexagon> currentPath, Vector3 startPos)
    {
        currentPath = currentPath.OrderBy(x => Vector3.Distance(startPos, x.hexPosition)).ToList();
        List<Vector3> positions = new List<Vector3>();
        List<int> indicesToRemove = new List<int>();

        var currentPathCopy = currentPath;

        for (int i = 0; i < currentPathCopy.Count; i++)
        {
            List<Hexagon> hexagonsFrontOfMe = new List<Hexagon>();
            if (ContainsMoreThanOne(currentPathCopy, currentPathCopy[i], ref hexagonsFrontOfMe))
            {
                var hexToDelete = hexagonsFrontOfMe.OrderBy(x => Vector3.Distance(x.hexPosition, startPos)).First();

                if (hexToDelete != currentPathCopy[i])
                {
                    currentPathCopy.Remove(hexToDelete);
                    hexToDelete.IsOnDestination = false;
                }
            }
            positions.Add(currentPathCopy[i].hexPosition);
        }
        foreach (var item in currentPathCopy)
        {
            activePath.Add(item);
        }

        return positions;
    }

    public bool ContainsMoreThanOne(List<Hexagon> currentPath, Hexagon hex, ref List<Hexagon> neighborsFrontOfMe)
    {
        int count = 0;
        int hexIndex = currentPath.IndexOf(hex);

        foreach (var item in hex.neighbors)
        {
            int itemIndex = currentPath.IndexOf(item);

            if (itemIndex > -1 && currentPath.Contains(item) && itemIndex  > hexIndex)
            {
                count++;
                neighborsFrontOfMe.Add(item);
            }
        }
        return count > 1;
    }
}