using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevY.Yulan.World{
public class YulanTree
{
  public int intensity;
  public float angle;
  public float length;
  
  public Branch root;
  public Transform worldTransform;
  public List<Branch> branches = new List<Branch>();

  public int smoothstep;


  public YulanTree (Transform parent, Vector3 start, int intensity, float length, float angle, int smoothstep) {
    this.intensity = intensity;
    this.length = length;
    this.angle = angle;

    this.worldTransform = parent;
    this.smoothstep = smoothstep;

    this.root = new Branch(this, Vector3.up, length, angle);
    this.branches.Add(root);
  }

  public void MakeTree(int child = 2, int sprig = 2) {
    this.Branching (this.root, child, sprig);
    Debug.Log ("# of branches:"+this.branches.Count);
  }

  private void Branching (Branch parent, int childcount = 2, int sprigcount = 2) {
    if (parent.level >= this.intensity) return;
    int cc = parent.level >= 2? Random.Range (childcount - 1, childcount):childcount;
    float w = 1 - ((float)parent.level /(this.intensity + 1));
    for (int i = 0; i < cc; i++) {
      Branch b = new Branch (parent, cc, w );
      parent.child.Add (b);
      this.branches.Add (b);
      if (b.level > 0) this.Sprigging (b, w, childcount, sprigcount);
      Branching (b, childcount, sprigcount);
    }

  }

  private void Sprigging (Branch b, float weight, int childcount = 2, int sprigcount = 2) {
    if (b.level > this.intensity - 1 || b.level < 2) return;
    for (int i = 0; i < sprigcount; i++) {
      Branch s = new Sprig (b, sprigcount, weight / 1.5f);
      b.sprig.Add(s);
      this.branches.Add(s);
      Branching (s, childcount - 1, sprigcount);
    }
  }

  public IEnumerator RenderLine (Transform root, Material mat) {
    for (int i = 0; i < this.branches.Count; i++) {
      GameObject obj = new GameObject();
      obj.transform.SetParent (root, false);
      LineRenderer l =obj.AddComponent<LineRenderer>();
      l.useWorldSpace = false;
      l.material = mat;
      l.SetPosition(0, this.branches[i].pos);
      l.SetPosition(1, this.branches[i].pos+this.branches[i].dir);
      l.startWidth = 0.1f * ( 1 - this.branches[i].level / (this.intensity+1.0f) ) ;
      yield return null;
    }
  }

  public void RenderTree(Camera cam) {
    
    if (cam == null) return;

    float w = 0.01f;    


    GL.PushMatrix();
    GL.Begin (GL.QUADS);

    
    for (int i = 0; i < this.branches.Count; i++){
      
      this.RenderSmoothStep (this.branches[i], cam, w * 2 );            
      /*
      GL.Color (this.branches[i].color);
      Vector3 src = (Vector3)(this.worldTransform.localToWorldMatrix * (this.branches[i].pos)) + this.worldTransform.position;
      Vector3 dst = (Vector3)(this.worldTransform.localToWorldMatrix * (this.branches[i].dir)) + src ;
      Vector3 width = Quaternion.AngleAxis(90.0f, cam.transform.forward) * Vector3.ProjectOnPlane (dst - src, cam.transform.forward).normalized * w;

      GL.Vertex3 (src.x - width.x, src.y - width.y , src.z - width.z);
      GL.Vertex3 (dst.x - width.x, dst.y - width.y , dst.z - width.z);
      GL.Vertex3 (dst.x + width.x, dst.y + width.y , dst.z + width.z);
      GL.Vertex3 (src.x + width.x, src.y + width.y , src.z + width.z);
*/
    }
    GL.End();
    //lines
    /*
        GL.Begin(GL.LINES);
    for (int i = 0; i < this.branches.Count; i++) {
      GL.Color (this.branches[i].color);
      Vector3 src = this.branches[i].pos;
      Vector3 dst = this.branches[i].pos + this.branches[i].dir;
      GL.Vertex3 (src.x, src.y, src.z);
      GL.Vertex3 (dst.x, dst.y, dst.z);
    }
    GL.End();
 */
 
    GL.PopMatrix();
 }

  private void RenderSmoothStep (Branch b, Camera cam, float w) {
    Vector3 src = (Vector3)(this.worldTransform.localToWorldMatrix * (b.pos)) + this.worldTransform.position;
    Vector3 dst = (Vector3)(this.worldTransform.localToWorldMatrix * (b.dir)) + src ;

    float start = w * (1 - ((float)b.level / (this.intensity+2) ) ) ;
    float end = w * (1 - ((float)(b.level + 1) / (this.intensity+2) ));;

    for (int i = 1; i < b.smoothsteps.Length; i++) {
      Vector3 s = src + (Vector3)(this.worldTransform.localToWorldMatrix * (b.smoothsteps[i - 1]));
      Vector3 d = src + (Vector3)(this.worldTransform.localToWorldMatrix * (b.smoothsteps[i]));

      Vector3 width = Quaternion.AngleAxis(90.0f, cam.transform.forward) * Vector3.ProjectOnPlane ( d - s , cam.transform.forward).normalized; 
      
      
      Color c = b.color;
      //Color c = b.color - (Color.blue) * ((float)i/level);
      //c.a = 1.0f;
      GL.Color (c);
      GL.Vertex3 (s.x - width.x * Mathf.Lerp (start, end, (float) (i - 1) / b.smoothsteps.Length), 
                  s.y - width.y * Mathf.Lerp (start, end, (float) (i - 1) / b.smoothsteps.Length),
                  s.z - width.z * Mathf.Lerp (start, end, (float) (i - 1) / b.smoothsteps.Length));
      GL.Vertex3 (d.x - width.x * Mathf.Lerp (start, end, (float) (i) / b.smoothsteps.Length),
                  d.y - width.y * Mathf.Lerp (start, end, (float) (i) / b.smoothsteps.Length),
                  d.z - width.z * Mathf.Lerp (start, end, (float) (i) / b.smoothsteps.Length));
      GL.Vertex3 (d.x + width.x * Mathf.Lerp (start, end, (float) (i - 1) / b.smoothsteps.Length),
                  d.y + width.y * Mathf.Lerp (start, end, (float) (i - 1) / b.smoothsteps.Length),
                  d.z + width.z * Mathf.Lerp (start, end, (float) (i - 1) / b.smoothsteps.Length));
      GL.Vertex3 (s.x + width.x * Mathf.Lerp (start, end, (float) (i) / b.smoothsteps.Length),
                  s.y + width.y * Mathf.Lerp (start, end, (float) (i) / b.smoothsteps.Length),
                  s.z + width.z * Mathf.Lerp (start, end, (float) (i) / b.smoothsteps.Length));
    }
    
  }

  private void RenderSmoothStep_0 (Branch b, Camera cam, float w, int level) {
    Vector3 src = (Vector3)(this.worldTransform.localToWorldMatrix * (b.pos)) + this.worldTransform.position;
    Vector3 dst = (Vector3)(this.worldTransform.localToWorldMatrix * (b.dir)) + src ;

    Vector3 normal;
    if (b.parent != null) normal = (Vector3)(this.worldTransform.localToWorldMatrix * (b.parent.dir.normalized));
    else normal = Vector3.forward;

    Vector3 alpha = (dst - src).magnitude * Mathf.Cos (Vector3.Angle (dst- src, normal)) * normal;
    Vector3 beta = (dst - src) - alpha;
    
    //Debug.Log (alpha + " + " + beta + " =  " + (alpha + beta) );

    for (int i = 1; i <= level ; i++) {
      Vector3 s = src + ((float) (i-1) / level) * beta + (3 * Mathf.Pow ((float) (i-1) / level, 2) - 2 * Mathf.Pow ((float) (i-1) / level, 3) ) * alpha;
      Vector3 d = src + ((float) (i) / level) * beta + (3 * Mathf.Pow ((float) (i) / level, 2) - 2 * Mathf.Pow ((float) (i) / level, 3) ) * alpha;

      //Debug.LogFormat ("{0} th : s = {1}, d = {2}", i, s, d);

      Vector3 width = Quaternion.AngleAxis(90.0f, cam.transform.forward) * Vector3.ProjectOnPlane ( d - s , cam.transform.forward).normalized * w; 
      Color c = b.color;
      //Color c = b.color - (Color.blue) * ((float)i/level);
      //c.a = 1.0f;
      GL.Color (c);
      GL.Vertex3 (s.x - width.x, s.y - width.y , s.z - width.z);
      GL.Vertex3 (d.x - width.x, d.y - width.y , d.z - width.z);
      GL.Vertex3 (d.x + width.x, d.y + width.y , d.z + width.z);
      GL.Vertex3 (s.x + width.x, s.y + width.y , s.z + width.z);
      
    }
    
  }

  private void RenderSmoothStep_1 (Branch b, Camera cam, float w, int level) {
    Vector3 src = (Vector3)(this.worldTransform.localToWorldMatrix * (b.pos)) + this.worldTransform.position;
    Vector3 dst = (Vector3)(this.worldTransform.localToWorldMatrix * (b.dir)) + src ;

    Vector3[] dt = new Vector3[level];
    dt[0] = src;
    
    for (int i = 1; i < level; i++) {
      Vector3 d = (dst - src) * ( i / (float)(level - 1));
      dt[i] = new Vector3 (d.x, 3 * Mathf.Pow (d.y, 2) - 2 * Mathf.Pow(d.y, 3) , d.z) + src;
      //Debug.LogFormat ("{0},{1},{2},{3}", t, i, dt[i-1], dt[i]);

      
      Vector3 width = Quaternion.AngleAxis(90.0f, cam.transform.forward) * Vector3.ProjectOnPlane (dt[i] - dt[i-1], cam.transform.forward).normalized * w;


      GL.Color (b.color);
      GL.Vertex3 (dt[i-1].x - width.x, dt[i-1].y - width.y , dt[i-1].z - width.z);
      GL.Vertex3 (dt[i].x - width.x, dt[i].y - width.y , dt[i].z - width.z);
      GL.Vertex3 (dt[i].x + width.x, dt[i].y + width.y , dt[i].z + width.z);
      GL.Vertex3 (dt[i-1].x + width.x, dt[i-1].y + width.y , dt[i-1].z + width.z);

    }

  }

}

public class Branch {
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

  public float world_angle;

  public Vector3[] smoothsteps;

  public Branch (Branch parent, int childcount, float weight) {
    this.parent = parent;
    this.level = parent.level + 1;
    this.child = new List<Branch>();
    this.sprig = new List<Branch>();
    this.angle = parent.angle * 1.2f;
    this.length = parent.length;
    this.tree = parent.tree;
    //this.color = parent.color;
    this.color= Color.white;
    this.smoothsteps = new Vector3[this.tree.smoothstep + 1];

    this.pos = parent.pos + parent.dir;

    //this.dir = Quaternion.Euler(0.0f, 0.0f,  (-1 * (angle / 2.0f) + (this.angle / (childcount - 1) * parent.child.Count))) * (parent.dir) * weight;
    this.dir = Quaternion.Euler(0.0f, 0.0f,  (-1 * (angle / 2.0f)) ) * (parent.dir);

    this.dir = this.WorldDir (this, childcount) * weight;

    this.smoothsteps = CalcSmoothStep (ref this.smoothsteps, this.dir, this.parent.dir, 1);
    //this.dir = Quaternuion

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

  // trunk
  public Branch (YulanTree tree, Vector3 direction, float length, float angle ) {
    this.level = 0;
    this.parent = null;
    this.child = new List<Branch>();
    this.sprig = new List<Branch>();
    this.pos = Vector3.zero;
    this.dir = direction;
    this.length = length;
    this.tree = tree;
    this.angle = angle;
    this.color = Color.white;


    this.smoothsteps = new Vector3[this.tree.smoothstep + 1];
    this.smoothsteps = CalcSmoothStep (ref this.smoothsteps, this.dir, Vector3.forward, 1);

  }
}


public class Sprig : Branch {
  public Sprig (Branch parent, int childcount, float weight) : base (parent, childcount, weight) {
    this.parent = parent;
    this.level = parent.level + 1;
    this.child = new List<Branch>();
    this.angle = parent.angle * 1.5f;
    this.length = parent.length;
    this.tree = parent.tree;
    this.color = Color.cyan;

    this.smoothsteps = new Vector3[this.tree.smoothstep];
    
    this.pos = parent.pos + parent.smoothsteps [ (int) ( (float)(parent.sprig.Count + 1) / (childcount + 1) * (parent.smoothsteps.Length) ) ];

    
    this.dir = parent.dir;
    //this.dir = Quaternion.Euler(0.0f, 0.0f, ((angle / childcount)) ) * (parent.dir);
    this.dir = this.WorldDir (this, childcount)  * weight;

    this.smoothsteps = CalcSmoothStep (ref this.smoothsteps, this.dir, this.parent.dir, 2);

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