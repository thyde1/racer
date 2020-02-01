using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TrackCreatorWindow : EditorWindow
{
    private float width = 10f;
    private float angle = 90f;

    [MenuItem("Window/Track Creator")]
    public static void OpenWindow()
    {
        var window = GetWindow<TrackCreatorWindow>();
        window.Show();
    }

    private void OnGUI()
    {
        this.titleContent = new GUIContent("Track Creator");
        GUILayout.Label("Width");
        float.TryParse(GUILayout.TextField(this.width.ToString()), out this.width);
        GUILayout.Label("Angle");
        float.TryParse(GUILayout.TextField(this.angle.ToString()), out this.angle);
        if (GUILayout.Button("Generate"))
        {
            CreateTrack(this.width, this.angle);
        };

    }

    public static void CreateTrack(float width, float angle)
    {
        var track = new GameObject("Track");
        var innerWall = CreateCubeArc(track.transform, 1, angle * Mathf.Deg2Rad);
        var outerWall = CreateCubeArc(track.transform, width + 1, angle* Mathf.Deg2Rad);

        var trackGround = new GameObject("Ground", typeof(MeshRenderer), typeof(MeshFilter));
        trackGround.transform.SetParent(track.transform);
        var groundMeshFilter = trackGround.GetComponent<MeshFilter>();
        var groundMesh = groundMeshFilter.sharedMesh = new Mesh();
        var innerVertices = DoOverArc(1, angle * Mathf.Deg2Rad, false, (pos, t, segmentLength) => pos);
        var outerVertices = DoOverArc(width + 1, angle * Mathf.Deg2Rad, false, (pos, t, segmentLength) => pos);
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
    }

    private static IEnumerable<T> DoOverArc<T>(float radius, float angleInRadians, bool doAtCentres, Func<Vector3, float, float, T> action)
    {
        var desiredSegmentLength = 0.4f;
        var segments = Mathf.Ceil(angleInRadians / desiredSegmentLength);
        var segmentLength = angleInRadians / segments;
        var results = new List<T>();
        for (var t = doAtCentres ? segmentLength * 0.5f : 0; t < angleInRadians; t += segmentLength)
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
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(arc.transform);
            cube.transform.localScale = new Vector3(1, 1, (radius + 0.6f) * segmentLength);
            cube.transform.position = position;
            cube.transform.rotation = Quaternion.Euler(0, -t * Mathf.Rad2Deg + 180, 0);
            return true;
        });

        return arc;
    }
}
