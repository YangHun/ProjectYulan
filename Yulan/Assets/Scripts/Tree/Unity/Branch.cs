using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevY.Yulan.Unity {
public class Branch : MonoBehaviour
{
  public int level;
  public Branch parent;
  public YulanTree tree;
  public List <Branch> child;
  public List <Branch> sprig;
  public Vector3 pos;
  public Vector3 dir;
  public Color color;

  public float length;
  public float angle;
  public float weight;

  public float world_angle;

  public Vector3[] smoothsteps;


  private Vector3 normal; //for trunk



#region function
  private float shaketimer;
  private bool shaking;
  public void Shake (Transform dir, float duration, float intensity) {
    if (shaking) StopAllCoroutines ();
    StartCoroutine (_Shake(dir, 1.0f, 1.0f));

  }
  private IEnumerator _Shake (Transform wind, float duration, float intensity) {
    if (!this.shaking) this.shaketimer = 0.0f;
    this.shaking = true;
    float angle = 30.0f;
    while (shaketimer < duration) {
      
      shaketimer += Time.fixedDeltaTime;


      this.transform.Rotate (wind.right, angle / 2.0f / duration * Time.fixedDeltaTime * (this.shaketimer > duration / 2.0f? -1 : 1) );

      
      yield return null;
    }
    yield return null;

    // if end
    this.shaketimer = 0.0f;
    this.shaking = false;
  }
  
#endregion



#region create and render
  public static Branch Create (Branch parent, int childcount, float weight) {
    
    GameObject o = new GameObject ();
    Branch b = o.AddComponent<Branch>();
    o.transform.SetParent(parent.transform, false);

    b.parent = parent;
    b.level = parent.level + 1;
    b.child = new List<Branch>();
    b.sprig = new List<Branch>();
    b.angle = parent.angle;
    b.length = parent.length;
    b.tree = parent.tree;
    //this.color = parent.color;
    b.color= Color.white;
    b.smoothsteps = new Vector3[b.tree.smoothstep + 1];

    b.pos = parent.transform.position + parent.dir;

    //this.dir = Quaternion.Euler(0.0f, 0.0f,  (-1 * (angle / 2.0f) + (this.angle / (childcount - 1) * parent.child.Count))) * (parent.dir) * weight;
    b.dir = Quaternion.Euler(0.0f, 0.0f,  (-1 * (b.angle / 2.0f)) ) * (b.parent.dir);

    b.dir = b.WorldDir (b, childcount);

    b.weight = weight;
    if (b.tree.sunIntensity > 0) {
      b.weight *= (0.5f + Mathf.Pow (Mathf.Cos (Vector3.Angle (b.dir, b.tree.sun * (-1f)) / 2.0f * Mathf.PI / 180.0f), b.tree.sunIntensity ));
    }
    b.dir = b.dir.normalized * b.length * b.weight;
    
    
    o.transform.position = b.pos;
    o.transform.LookAt (b.pos + b.dir);

    b.smoothsteps = b.CalcSmoothStep (ref b.smoothsteps, b.transform.worldToLocalMatrix * b.transform.forward * b.dir.magnitude, b.parent.dir, 1);
    //this.dir = Quaternuion


    b.AddCollider(b);


    o.name = string.Format ("{0}_{1}_branch",b.level, b.parent.child.Count);


    return b;

  }

  protected float SmoothStep (float value, int level) {
    switch (level) {
      case 0: return value;
      case 1: return ( (-2) * Mathf.Pow(value, 3) + (3 * Mathf.Pow(value,2)));
      case 2: return ( (6 * Mathf.Pow(value, 5) - 15 * Mathf.Pow(value, 4) + 10 * Mathf.Pow (value,3)));
    }
    return Mathf.Infinity * (-1);
  }


  protected Vector3[] CalcSmoothStep (ref Vector3[] array, Vector3 dir, Vector3 normal, int smoothstep) {
    Vector3 alpha = dir.magnitude * Mathf.Cos (Vector3.Angle(dir, normal)) * normal;
    Vector3 beta = dir - alpha;
    for (int i = 0; i < array.Length; i++) {
      float dt = ((float)i / (array.Length - 1));
      array[i] = dt * beta + SmoothStep (dt, smoothstep) * alpha;
    }
    return array;
  }

  protected Vector3 WorldDir (Branch b, int childcount) {
    Vector3 result = Vector3.zero;

    Vector3 normal = b.parent.dir.normalized;
    Vector3 target = b.dir;

    if (b.level == 0) return b.parent.dir.normalized;
    if (childcount == 1) return target; 


    if (b.parent.child.Count < 1) {
      b.world_angle = Random.Range (0.0f, 119.0f);
    }
    else {
      b.world_angle = b.parent.child[b.parent.child.Count-1].world_angle + 360.0f / childcount;
    }

    result = Quaternion.AngleAxis (b.world_angle, normal) * target;
    
    return result.normalized;
  }

  protected void AddCollider (Branch b) {
    BoxCollider collider = b.gameObject.AddComponent<BoxCollider>();
    collider.center = Vector3.forward * b.length * b.weight / 2.0f;
    collider.size = new Vector3 ( 0.05f, 0.05f, b.length * b.weight);
  }

  // trunk
  public static Branch Create (YulanTree tree, Vector3 direction, float length, float angle, Vector3 normal) {
    
    GameObject o = new GameObject("0_root");
    Branch b = o.AddComponent<Branch>();

    o.transform.SetParent(tree.transform, false);

    b.level = 0;
    b.parent = null;
    b.child = new List<Branch>();
    b.sprig = new List<Branch>();
    b.pos = tree.transform.position;
    b.dir = direction;
    b.length = length;
    b.tree = tree;
    b.angle = angle;
    b.color = Color.white;
    b.weight = 1;
    b.normal = normal;

    if (b.tree.sunIntensity > 0) {
      b.weight *= (0.5f + Mathf.Pow (Mathf.Cos (Vector3.Angle (b.dir, b.tree.sun * (-1f)) / 2.0f * Mathf.PI / 180.0f), b.tree.sunIntensity ));
    }
    o.transform.position = b.pos;
    o.transform.LookAt (b.pos + b.dir);
    
    b.smoothsteps = new Vector3[b.tree.smoothstep + 1];
    b.smoothsteps = b.CalcSmoothStep (ref b.smoothsteps, Vector3.forward, b.normal, 1);



    return b;
  }
#endregion
}
}