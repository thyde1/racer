using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TrackCreatorWindow : EditorWindow
{
    private float bendWidth = 10f;
    private float angle = 90f;
    private float straightWidth;

    [MenuItem("Window/Track Creator")]
    public static void OpenWindow()
    {
        var window = GetWindow<TrackCreatorWindow>();
        window.Show();
    }

    private void OnGUI()
    {
        this.titleContent = new GUIContent("Track Creator");
        GUILayout.Label("Bend");
        GUILayout.Label("Width");
        float.TryParse(GUILayout.TextField(this.bendWidth.ToString()), out this.bendWidth);
        GUILayout.Label("Angle");
        float.TryParse(GUILayout.TextField(this.angle.ToString()), out this.angle);
        if (GUILayout.Button("Generate Bend"))
        {
            CreateBend(this.bendWidth, this.angle);
        };

        GUILayout.Label("Straight");
        GUILayout.Label("Width");
        float.TryParse(GUILayout.TextField(this.bendWidth.ToString()), out this.straightWidth);
        if (GUILayout.Button("Generate Straight"))
        {
            CreateStraight(this.straightWidth);
        };
    }

    public static void CreateBend(float width, float angle)
    {
        var bend = new GameObject("Bend");
        var innerWall = CreateCubeArc(bend.transform, 1, angle * Mathf.Deg2Rad);
        var outerWall = CreateCubeArc(bend.transform, width, angle* Mathf.Deg2Rad);

        var innerVertices = DoOverArc(1, angle * Mathf.Deg2Rad, false, (pos, t, segmentLength) => pos);
        var outerVertices = DoOverArc(width, angle * Mathf.Deg2Rad, false, (pos, t, segmentLength) => pos);
        GenerateGround(innerVertices, outerVertices, bend.transform);

        Selection.activeGameObject = bend;
    }

    public static void CreateStraight(float width)
    {
        var straight = new GameObject("Straight");
        var innerWall = GenerateWall(straight.transform);
        innerWall.transform.position = new Vector3(1, 0, 0);
        innerWall.transform.localScale = new Vector3(1, 1, width - 1);
        var outerWall = GenerateWall(straight.transform);
        outerWall.transform.position = new Vector3(width, 0, 0);
        outerWall.transform.localScale = new Vector3(1, 1, width - 1);
        var innerVertices = new[] { innerWall.transform.position - (width - 1) * 0.5f * innerWall.transform.forward, innerWall.transform.position + (width - 1) * 0.5f * innerWall.transform.forward };
        var outerVertices = new[] { outerWall.transform.position - (width - 1) * 0.5f * outerWall.transform.forward, outerWall.transform.position + (width - 1) * 0.5f * outerWall.transform.forward };
        GenerateGround(innerVertices, outerVertices, straight.transform);

        Selection.activeGameObject = straight;
    }

    private static IEnumerable<T> DoOverArc<T>(float radius, float angleInRadians, bool doAtCentres, Func<Vector3, float, float, T> action)
    {
        var desiredSegmentLength = 0.2f;
        var segments = Mathf.Ceil(angleInRadians / desiredSegmentLength);
        var segmentLength = angleInRadians / segments;
        var results = new List<T>();
        for (var t = doAtCentres ? segmentLength * 0.5f : 0; t < angleInRadians + segmentLength * 0.25f; t += segmentLength)
        {
            results.Add(action(new Vector3(radius * Mathf.Cos(t), 0, radius * Mathf.Sin(t)), t, segmentLength));
        }

        return results;
    }

    private static GameObject CreateCubeArc(Transform parent, float radius, float angleInRadians)
    {
        var arc = new GameObject("Arc");
        arc.transform.parent = parent;

        DoOverArc(radius, angleInRadians, true, (position, t, segmentLength) =>
        {
            var wall = GenerateWall(arc.transform);
            wall.transform.localScale = new Vector3(1, 1, (radius + 0.6f) * segmentLength);
            wall.transform.position = position;
            wall.transform.rotation = Quaternion.Euler(0, -t * Mathf.Rad2Deg + 180, 0);
            return true;
        });

        return arc;
    }

    private static GameObject GenerateGround(IEnumerable<Vector3> innerVertices, IEnumerable<Vector3> outerVertices, Transform parent)
    {
        var trackGround = new GameObject("Ground", typeof(MeshRenderer), typeof(MeshFilter));
        var groundMeshFilter = trackGround.GetComponent<MeshFilter>();
        groundMeshFilter.sharedMesh = GenerateGroundMesh(innerVertices, outerVertices);
        trackGround.transform.SetParent(parent);
        var groundMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Prefabs/World/TrackMaterial.mat");
        trackGround.GetComponent<MeshRenderer>().sharedMaterial = groundMaterial;
        return trackGround;
    }

    private static GameObject GenerateWall(Transform parent)
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
    private static Mesh GenerateGroundMesh(IEnumerable<Vector3> innerVertices, IEnumerable<Vector3> outerVertices)
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
