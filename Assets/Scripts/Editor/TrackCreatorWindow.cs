using System.Collections;
using System.Collections.Generic;
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
        var trackGround = new GameObject("Ground", typeof(MeshRenderer), typeof(MeshFilter));
        trackGround.transform.SetParent(track.transform);
        var groundMeshFilter = trackGround.GetComponent<MeshFilter>();
        groundMeshFilter.mesh = new Mesh();
        CreateCubeArc(track.transform, width + 1, angle * Mathf.Deg2Rad);
        CreateCubeArc(track.transform, 1, angle * Mathf.Deg2Rad);
    }

    private static void CreateCubeArc(Transform parent, float radius, float angleInRadians)
    {
        var desiredSegmentLength = 0.4f;
        var segments = Mathf.Ceil(angleInRadians / desiredSegmentLength);
        var segmentLength = angleInRadians / segments;

        var arc = new GameObject("Arc");
        arc.transform.parent = parent;

        for (var t = segmentLength * 0.5f; t < angleInRadians; t+= segmentLength)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(arc.transform);
            cube.transform.localScale = new Vector3(1, 1, (radius + 0.6f) * segmentLength);
            cube.transform.position = new Vector3(radius * Mathf.Cos(t), 0, radius * Mathf.Sin(t));
            cube.transform.rotation = Quaternion.Euler(0, -t * Mathf.Rad2Deg + 180, 0);
        }
    }
}
