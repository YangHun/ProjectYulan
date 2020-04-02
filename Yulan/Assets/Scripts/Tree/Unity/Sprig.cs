using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevY.Yulan.Unity{
public class Sprig : Branch
{
  new public static Sprig Create (Branch parent, int childcount, float weight) {
    
    GameObject o = new GameObject();
    Sprig s = o.AddComponent<Sprig>();
    o.transform.SetParent(parent.transform, false);
    
    s.parent = parent;
    s.level = parent.level + 2;
    s.child = new List<Branch>();
    s.leaf = new List<LeafBud>();
    s.angle = parent.angle * 2.0f;
    s.length = parent.length * 2.0f;
    s.tree = parent.tree;
    s.color = Color.white;

    s.smoothsteps = new Vector3[s.tree.smoothstep];
    
    s.pos = parent.pos + parent.transform.rotation * parent.smoothsteps [ (int) ( (float)(parent.sprig.Count + 1) / (childcount + 1) * (parent.smoothsteps.Length) ) ];

    
    s.dir = parent.dir;
    //this.dir = Quaternion.Euler(0.0f, 0.0f, ((angle / childcount)) ) * (parent.dir);
    s.dir = s.WorldDir (s, childcount);


    s.weight = weight;
    if (s.tree.sunIntensity > 0) {
      s.weight *= (0.5f + Mathf.Pow (Mathf.Cos (Vector3.Angle (s.dir, s.tree.sun * (-1f)) / 2.0f * Mathf.PI / 180.0f), s.tree.sunIntensity ));
    }
    s.dir = s.dir.normalized * s.length * s.weight;

    o.transform.position = s.pos;
    o.transform.LookAt (s.pos + s.dir);

    
    s.smoothsteps = s.CalcSmoothStep (ref s.smoothsteps, s.transform.worldToLocalMatrix * s.transform.forward * s.dir.magnitude, s.parent.dir, 1);

    o.name = string.Format ("{0}_{1}_sprig",s.level, s.parent.sprig.Count);
    s.AddCollider(s);

    return s;
  }

  protected Vector3 WorldDir (Sprig b, int childcount) {
    Vector3 result = Vector3.zero;

    Vector3 normal = Vector3.Cross(b.parent.dir, b.parent.parent.dir);
    Vector3 target = b.dir;

    result = Quaternion.AngleAxis ( (b.parent.sprig.Count % 2 == 0? 1 : -1) * this.angle / 2, normal * -1) * target;

    return result.normalized;
  }
}
}