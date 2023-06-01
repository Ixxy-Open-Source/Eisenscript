using System.IO;
using UnityEngine;
using Eisenscript;

public class EisenscriptTest : MonoBehaviour
{

    public Transform prefab;

    void Start()
    {
        TextReader input = new StringReader(@"
set maxdepth 40

r0

rule r0 {
  3 * { rz 120 } r1
  3 * { rz 120 } r2
}

rule r1 {
  { x 1.3 rx 1.57 rz 6 ry 3 s 0.99 hue 0.4 sat 0.99 } r1
  { s 4 } sphere
}

rule r2 {
  { x -1.3 rz 6 ry 3 s 0.99 hue 0.4 sat 0.99 } r2
  { s 4 } sphere
}
");
        var builder = new SSBuilder();
        Vector3 pos = Vector3.zero;
        Quaternion rot = Quaternion.identity;
        Vector3 scale = Vector3.one;
        builder.DrawEvent += (sender, args) =>
        {
            var thing = Instantiate(prefab);
            args.Matrix.Decompose(out Vector3 t, out Quaternion r, out Vector3 s);
            pos += t;
            rot *= r;
            scale += s;
            thing.position = pos;
            thing.rotation = rot;
            thing.localScale = scale;
            Debug.Log($"Sender: {sender} args: {args.Type} {pos} {rot.eulerAngles} {scale} :: {args.Rgba}");
        };
        var errors = builder.Build(input);
        foreach (var error in errors)
        {
            Debug.LogError($"Error: {error.Message}");
        }
    }
}
