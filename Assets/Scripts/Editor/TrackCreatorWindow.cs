using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TrackCreatorWindow : EditorWindow
{
    private float trackWidth = 10f;
    private float angle = 90f;
    private float wallThickness = 1f;

    [MenuItem("Window/Track Creator")]
    public static void OpenWindow()
    {
        var window = GetWindow<TrackCreatorWindow>();
        window.Show();
    }

    private void OnGUI()
    {
        this.titleContent = new GUIContent("Track Creator");
        GUILayout.Label("Wall Thickness");
        float.TryParse(GUILayout.TextField(this.wallThickness.ToString()), out this.wallThickness);
        GUILayout.Label("Width");
        float.TryParse(GUILayout.TextField(this.trackWidth.ToString()), out this.trackWidth);
        GUILayout.Label("Bend");
        GUILayout.Label("Angle");
        float.TryParse(GUILayout.TextField(this.angle.ToString()), out this.angle);
        if (GUILayout.Button("Generate Bend"))
        {
            var bend = CreateBend(this.trackWidth, this.angle);
            this.PositionCreatedGameObject(bend);
        };

        GUILayout.Label("Straight");
        if (GUILayout.Button("Generate Straight"))
        {
            var straight = CreateStraight(this.trackWidth);
            this.PositionCreatedGameObject(straight);
        };
    }

    public GameObject CreateBend(float width, float angle)
    {
        var bend = new GameObject("Bend");
        var innerWall = CreateCubeArc(bend.transform, 1, angle * Mathf.Deg2Rad);
        var outerWall = CreateCubeArc(bend.transform, width + 1, angle* Mathf.Deg2Rad);

        var innerVertices = DoOverArc(1, angle * Mathf.Deg2Rad, false, (pos, t, segmentLength) => pos);
        var outerVertices = DoOverArc(width + 1, angle * Mathf.Deg2Rad, false, (pos, t, segmentLength) => pos);
        var centralVertices = DoOverArc(1 + width * 0.5f, angle * Mathf.Deg2Rad, false, (pos, t, segmentLength) => pos);
        GenerateGround(angle < 0 ? innerVertices : innerVertices.Reverse(), angle < 0 ? outerVertices : outerVertices.Reverse(), bend.transform);

        var startConnectionPoint = new GameObject("Start Connection Point");
        startConnectionPoint.AddComponent<TrackConnectionPoint>().Type = TrackConnectionPoint.ConnectionType.Start;
        startConnectionPoint.transform.SetParent(bend.transform);
        startConnectionPoint.transform.localPosition = centralVertices.First();
        var endConnectionPoint = new GameObject("End Connection Point");
        endConnectionPoint.AddComponent<TrackConnectionPoint>().Type = TrackConnectionPoint.ConnectionType.End;
        endConnectionPoint.transform.SetParent(bend.transform);
        endConnectionPoint.transform.localPosition = centralVertices.Last();
        endConnectionPoint.transform.localRotation = Quaternion.Euler(0, angle, 0);

        return bend;
    }

    public GameObject CreateStraight(float width)
    {
        var straight = new GameObject("Straight");
        var innerWall = GenerateWall(straight.transform);
        innerWall.transform.position = new Vector3(-width * 0.5f, 0, 0);
        innerWall.transform.localScale = new Vector3(this.wallThickness, 1, width);
        var outerWall = GenerateWall(straight.transform);
        outerWall.transform.position = new Vector3(width * 0.5f, 0, 0);
        outerWall.transform.localScale = new Vector3(this.wallThickness, 1, width);
        var innerVertices = new[] { innerWall.transform.position - width * 0.5f * innerWall.transform.forward, innerWall.transform.position + width * 0.5f * innerWall.transform.forward };
        var outerVertices = new[] { outerWall.transform.position - width * 0.5f * outerWall.transform.forward, outerWall.transform.position + width * 0.5f * outerWall.transform.forward };
        GenerateGround(innerVertices, outerVertices, straight.transform);
        var startConnectionPoint = new GameObject("Start Connection Point");
        startConnectionPoint.AddComponent<TrackConnectionPoint>().Type = TrackConnectionPoint.ConnectionType.Start;
        startConnectionPoint.transform.SetParent(straight.transform);
        startConnectionPoint.transform.localPosition = new Vector3(0, 0, -width * 0.5f);
        var endConnectionPoint = new GameObject("End Connection Point");
        endConnectionPoint.AddComponent<TrackConnectionPoint>().Type = TrackConnectionPoint.ConnectionType.End;
        endConnectionPoint.transform.SetParent(straight.transform);
        endConnectionPoint.transform.localPosition = new Vector3(0, 0, width * 0.5f);

        return straight;
    }

    private void PositionCreatedGameObject(GameObject gameObject)
    {
        if (Selection.activeTransform != null)
        {
            var startConnectionPoint = gameObject.GetComponentsInChildren<TrackConnectionPoint>().FirstOrDefault(c => c.Type == TrackConnectionPoint.ConnectionType.Start);
            var endConnectionPoint = (Selection.activeGameObject.GetComponentsInChildren<TrackConnectionPoint>()).FirstOrDefault(c => c.Type == TrackConnectionPoint.ConnectionType.End);
            // Rejig hierarchy to make connection points parents
            TransformUtils.InvertParentChildRelationship(gameObject, startConnectionPoint.gameObject);
            TransformUtils.InvertParentChildRelationship(Selection.activeGameObject, endConnectionPoint.gameObject);
            // Position game object
            startConnectionPoint.transform.SetPositionAndRotation(endConnectionPoint.transform.position, endConnectionPoint.transform.rotation);
            // Revert hierarchy
            TransformUtils.InvertParentChildRelationship(startConnectionPoint.gameObject, gameObject);
            TransformUtils.InvertParentChildRelationship(endConnectionPoint.gameObject, Selection.activeGameObject);
        }

        Selection.activeGameObject = gameObject;
    }

    private IEnumerable<T> DoOverArc<T>(float radius, float angleInRadians, bool doAtCentres, Func<Vector3, float, float, T> action)
    {
        var desiredSegmentLength = 0.2f;
        var segments = Mathf.Ceil(angleInRadians / desiredSegmentLength);
        var segmentLength = angleInRadians / segments;
        var results = new List<T>();
        if (angleInRadians < 0)
        {
            angleInRadians = -angleInRadians;
            for (var t = doAtCentres ? segmentLength * 0.5f : 0; t < angleInRadians + segmentLength * 0.25f; t += segmentLength)
            {
                results.Add(action(new Vector3(radius * Mathf.Cos(t), 0, radius * Mathf.Sin(t)), t, segmentLength));
            }
        }
        else
        {
            for (var t = Mathf.PI + (doAtCentres ? segmentLength * 0.5f : 0); t < Mathf.PI + angleInRadians + segmentLength * 0.25f; t += segmentLength)
            {
                results.Add(action(new Vector3(radius * Mathf.Cos(-t), 0, -radius * Mathf.Sin(t)), -t, segmentLength));
            }
        }

        return results;
    }

    private GameObject CreateCubeArc(Transform parent, float radius, float angleInRadians)
    {
        var arc = new GameObject("Arc");
        arc.transform.parent = parent;

        DoOverArc(radius, angleInRadians, true, (position, t, segmentLength) =>
        {
            var wall = GenerateWall(arc.transform);
            wall.transform.localScale = new Vector3(this.wallThickness, 1, (radius + 0.6f * this.wallThickness) * segmentLength);
            wall.transform.position = position;
            wall.transform.rotation = Quaternion.Euler(0, -t * Mathf.Rad2Deg + 180, 0);
            return true;
        });

        return arc;
    }

    private GameObject GenerateGround(IEnumerable<Vector3> innerVertices, IEnumerable<Vector3> outerVertices, Transform parent)
    {
        var trackGround = new GameObject("Ground", typeof(MeshRenderer), typeof(MeshFilter));
        var groundMeshFilter = trackGround.GetComponent<MeshFilter>();
        groundMeshFilter.sharedMesh = GenerateGroundMesh(innerVertices, outerVertices);
        trackGround.transform.SetParent(parent);
        var trackMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Prefabs/World/TrackMaterial.mat");
        trackGround.GetComponent<MeshRenderer>().sharedMaterial = trackMaterial;
        return trackGround;
    }

    private GameObject GenerateWall(Transform parent)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "Wall";
        var wallMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Prefabs/World/WallMaterial.mat"); ;
        cube.GetComponent<MeshRenderer>().material = wallMaterial;
        cube.transform.SetParent(parent);
        return cube;
    }

    /// <summary>
    /// Generates ground mesh for a section of track. Expects innerVertices.length == outerVertices.length
    /// </summary>
    private Mesh GenerateGroundMesh(IEnumerable<Vector3> innerVertices, IEnumerable<Vector3> outerVertices)
    {
        var groundMesh = new Mesh();
        groundMesh.vertices = innerVertices.Concat(outerVertices).ToArray();
        groundMesh.uv = groundMesh.vertices.Select(v => new Vector2(v.x, v.z)).ToArray();
        List<int> triangles = new List<int>();
        for (int i = 0; i < innerVertices.Count() - 1; i++)
        {
            triangles.Add(i + 1);
            triangles.Add(i + innerVertices.Count());
            triangles.Add(i);
            triangles.Add(i + innerVertices.Count() + 1);
            triangles.Add(i + innerVertices.Count());
            triangles.Add(i + 1);
        }

        groundMesh.triangles = triangles.ToArray();
        return groundMesh;
    }
}
