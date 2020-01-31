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
        CreateCubeArc(track.transform, width + 1, angle);
        CreateCubeArc(track.transform, 1, angle);
    }

    private static void CreateCubeArc(Transform parent, float radius, float angle)
    {
        var arc = new GameObject("Arc");
        arc.transform.parent = parent;
        var segmentLength = 0.4f;

        var t = -segmentLength;
        do {
            t += segmentLength;
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(arc.transform);
            cube.transform.localScale = new Vector3(1, 1, (radius + 0.6f) * segmentLength);
            cube.transform.position = new Vector3(radius * Mathf.Cos(t), 0, radius * Mathf.Sin(t));
            cube.transform.rotation = Quaternion.Euler(0, -t * Mathf.Rad2Deg + 180, 0);
        } while (t <= angle * Mathf.Deg2Rad);

        var firstCube = arc.transform.GetChild(0);
        var lastCube = arc.transform.GetChild(arc.transform.childCount - 1);
        firstCube.localScale = new Vector3(1, 1, firstCube.localScale.z * 0.5f);
        firstCube.position -= firstCube.forward * firstCube.localScale.z * 0.5f;
        lastCube.localScale = new Vector3(1, 1, lastCube.localScale.z * 0.5f);
        lastCube.position += lastCube.forward * lastCube.localScale.z * 0.5f;
    }
}
