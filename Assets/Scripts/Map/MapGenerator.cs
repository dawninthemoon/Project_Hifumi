using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RieslingUtils;

public class MapGenerator : MonoBehaviour {
    [SerializeField] private EncounterMarker[] _encounterPrefabs = null;
    [SerializeField] private Transform _originPosition = null;
    [SerializeField] private Transform _gameMapParent = null;
    [SerializeField] private float _offsetX = 10f;
    [SerializeField] private float _offsetY = 10f;
    [SerializeField, Range(2, 6)] private int _executeTime = 2;

    public CustomGrid<EncounterMarker> GenerateMap(int width, int height, EncounterMarker.OnMarkerSelected onMarkerSelected) {
        List<int>[] pathList = new List<int>[_executeTime];

        CustomGrid<EncounterMarker> mapGrid = GeneratePath(pathList, width, height);
        GenerateNodes(mapGrid, pathList, width, height, onMarkerSelected);
        GenerateVertices(mapGrid, pathList);

        return mapGrid;
    }

    public CustomGrid<EncounterMarker> GeneratePath(List<int>[] pathList, int width, int height) {
        CustomGrid<EncounterMarker> mapGrid = new CustomGrid<EncounterMarker>(width, height, _originPosition.position, _offsetX, _offsetY);
        
        for (int i = 0; i < pathList.Length; ++i) {
            pathList[i] = new List<int>();

            int col = Random.Range(0, width);
            if (i < 2 && mapGrid.GetElement(0, col)) {
                i--;
                continue;
            }
            for (int row = 0; row < height; ++row) {
                if (row > 0) {
                    col += Random.Range(-1, 2);
                    col = Mathf.Clamp(col, 0, width - 1);
                }

                pathList[i].Add(col);
            }
        }
        return mapGrid;
    }

    public void GenerateNodes(CustomGrid<EncounterMarker> mapGrid, List<int>[] pathList, int width, int height, EncounterMarker.OnMarkerSelected callback) {
        // Instantiate Nodes
        for (int t = 0; t < _executeTime; ++t) {
            for (int floor = 0; floor < height; ++floor) {
                int x = pathList[t][floor];
                if (!mapGrid.GetElement(floor, x)) {
                    Vector3 nodePosition = mapGrid.RowcolToPoint(floor, x);
                    EncounterMarker newEncounter = CreateEncounter(nodePosition, floor, x, height, callback);
                    mapGrid.SetElement(floor, x, newEncounter);
                }
            }
        }

        for (int t = 0; t < _executeTime; ++t) {
            for (int floor = 0; floor < height - 1; ++floor) {
                int x = pathList[t][floor];
                int nextX = pathList[t][floor + 1];

                EncounterMarker cur = mapGrid.GetElement(floor, x);
                EncounterMarker next = mapGrid.GetElement(new Rowcol(floor + 1, nextX));

                cur.ConnectNode(next);
            }
        }
    }
    private EncounterMarker CreateEncounter(Vector3 position, int row, int col, int height, EncounterMarker.OnMarkerSelected callback) {
        EncounterType encounterType = GetEncounterType(row + 1, height);
        EncounterMarker nodePrefab = _encounterPrefabs[(int)encounterType];

        EncounterMarker node = Instantiate(nodePrefab, position, Quaternion.identity, _gameMapParent);
        node.Initialize(encounterType, new Rowcol(row, col), callback);

        return node;
    }

    public void GenerateVertices(CustomGrid<EncounterMarker> mapGrid, List<int>[] pathList) {
        Queue<Rowcol> bfsQueue = new Queue<Rowcol>();

        for (int t = 0; t < _executeTime; ++t) {
            Rowcol initialRowcol = new Rowcol(0, pathList[t][0]);
            MakeVertex(mapGrid, initialRowcol, new Rowcol(1, pathList[t][1]));
        }
    }

    private void MakeVertex(CustomGrid<EncounterMarker> mapGrid, Rowcol from, Rowcol to) {
        EncounterMarker prev = mapGrid.GetElement(from);
        EncounterMarker cur = mapGrid.GetElement(to);

        Vector3 prevPosition = prev.transform.position;
        Vector3 currentPosition = cur.transform.position;

        LineRenderer lineRenderer = new GameObject().AddComponent<LineRenderer>();
        lineRenderer.transform.SetParent(_gameMapParent);

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, prevPosition);
        lineRenderer.SetPosition(1, currentPosition);

/*
        Vector3 diff = currentPosition - prevPosition;
        float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg - 90f;
        
        float distance = Vector3.Distance(prevPosition, currentPosition);
        int dotCounts = (int)(distance * 7f);

        for (int i = 2; i < dotCounts - 1; ++i) {
            Vector3 dotPosition = Vector3.Lerp(prevPosition, currentPosition, (float)i / dotCounts);
            float dotAngle = angle + Random.Range(-15f, 15f);

            var dot = _vertextDotPool.GetObject();
            dot.transform.position = dotPosition;
            dot.transform.rotation = Quaternion.Euler(0f, 0f, dotAngle);
        }*/
        
        foreach (EncounterMarker nextNode in cur.AdjustSet) { 
            MakeVertex(mapGrid, to, nextNode.Rowcol);
        }
    }

    private EncounterType GetEncounterType(int floor, int height) {
        if (floor == 1) {
            return EncounterType.SHOP;
        }
        else if (floor == 2) {
            return EncounterType.COMBAT;
        }
        
        int randNum = Random.Range(0, 1000);
        if (randNum < 120) {
            return EncounterType.ALLY;
        }
        else if (randNum < 340) {
            return EncounterType.EVENT;
        }
        return EncounterType.COMBAT;
    }
}
