using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TrackCreatorWindow : EditorWindow
{
    [MenuItem("Window/Track Creator")]
    public static void OpenWindow()
    {
        var window = GetWindow<TrackCreatorWindow>();
        window.Show();
    }

    private void OnGUI()
    {
        this.titleContent = new GUIContent("Track Creator");
        if (GUILayout.Button("Generate"))
        {
            CreateTrack(10, 90);
        };

    }

    public static void CreateTrack(float width, float angle)
    {
        var track = new GameObject("Track");
        var trackGround = new GameObject("Ground", typeof(MeshRenderer), typeof(MeshFilter));
        trackGround.transform.SetParent(track.transform);
        var groundMeshFilter = trackGround.GetComponent<MeshFilter>();
        groundMeshFilter.mesh = new Mesh();
        CreateCubeArc(track.transform, 5, 90);
        CreateCubeArc(track.transform, 1, 90);
    }

    private static void CreateCubeArc(Transform parent, float radius, float angle)
    {
        var segmentLength = 0.4f;

        var t = -segmentLength;
        do {
            t += segmentLength;
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(parent);
            cube.transform.localScale = new Vector3(1, 1, (radius + 0.6f) * segmentLength);
            cube.transform.position = new Vector3(radius * Mathf.Cos(t), 0, radius * Mathf.Sin(t));
            cube.transform.rotation = Quaternion.Euler(0, -t * Mathf.Rad2Deg + 180, 0);
        } while (t <= angle * Mathf.Deg2Rad);
    }
}
