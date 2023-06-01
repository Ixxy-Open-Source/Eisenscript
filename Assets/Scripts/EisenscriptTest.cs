using System.IO;
using UnityEngine;
using Eisenscript;

public class EisenscriptTest : MonoBehaviour
{
    [TextArea(40, 10)] public string script;
    public Transform prefab;

    void Start()
    {
        TextReader input = new StringReader(script);
        var builder = new SSBuilder();
        Vector3 pos = Vector3.zero;
        Quaternion rot = Quaternion.identity;
        Vector3 scale = Vector3.one;
        builder.DrawEvent += (sender, args) =>
        {
            var thing = Instantiate(prefab);
            Vector3 t = args.Matrix.GetColumn(3);
            Quaternion r = args.Matrix.rotation;
            Vector3 s = args.Matrix.lossyScale;
            pos = t;
            rot *= r;
            scale.Scale(s);
            thing.position = pos;
            thing.rotation = rot;
            thing.localScale = scale;
            var mr = thing.GetComponent<Renderer>();
            mr.material.SetColor("_Color", args.Rgba);
        };
        var errors = builder.Build(input);
        prefab.gameObject.SetActive(false);
        foreach (var error in errors)
        {
            Debug.LogError($"Error: {error.Message}");
        }
    }
}
