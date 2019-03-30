using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevY.Yulan {

public class YulanTree {
  public Vector4 light;
  public int intensity;
  public float angle;
  public float length;

  public Branch root;
  public List<Branch> branches = new List<Branch>();

  public YulanTree (Vector3 start, int intensity, float length, float angle, Vector4 sunlight, float sun_intensity) {
    this.length = length;
    this.angle = angle;
    this.intensity = intensity;
    this.light = new Vector4 (sunlight.x, sunlight.y, sunlight.z, sun_intensity);
    this.root = new Branch (this, start, length, angle);
    this.branches.Add(root);
    //this.nodes = 1;
  }

  public void MakeCompleteTree () {
    this.Branching (this.root, this.branches);
    //Debug.LogFormat ("# of nodes in this tree: {0}", this.nodes);
    Debug.LogFormat ("# of nodes in this tree: {0}", this.branches.Count);
  }

  private void Branching (Branch parent, List<Branch> branches, int childcount = 2, bool complete = true) {
    if (parent.level >= this.intensity) return;
    int cc = childcount;
    if (!complete) cc = Random.Range(2, childcount);
    for (int i = 0; i < cc; i++) {
      Branch b = new Branch (parent, ( 1 - (float)parent.level / this.intensity ), cc, new Vector3 (this.light.x, this.light.y, this.light.z), this.light.w);
      //b.tree.nodes += 1;
      parent.child.Add (b);
      branches.Add(b);
      Branching (b, branches, childcount, complete);
      //Debug.LogFormat("branch {0}_{1} is created", b.level, parent.child.Count);
    }
  }

  private void Sprigging () {

  }

  public void RenderTree () {
    
    this.Rendering();
    this.Blooming();
    //this.Rendering(this.root);

  }

  private void Rendering () {
    GL.Begin(GL.LINES);
    for (int i = 0; i < this.branches.Count; i++) {
      GL.Color (Color.white);
      Vector3 src = this.branches[i].pos;
      Vector3 dst = this.branches[i].pos + this.branches[i].dir;
      GL.Vertex3 (src.x, src.y, src.z);
      GL.Vertex3 (dst.x, dst.y, dst.z);
    }
    GL.End();
  }

  private void Blooming () {
    float size = 1.0f;
    GL.Begin(GL.TRIANGLES);
    for (int i = this.branches.Count - 1; i >= 0; i--) {
      if (this.branches[i].child.Count > 0) continue;

      GL.Color (Color.yellow);
      Vector3 dst = this.branches[i].pos + this.branches[i].dir;
      Vector3 a = this.branches[i].dir.normalized * this.branches[i].weight * size * Mathf.Sqrt(3.0f) / 2.0f;
      Vector3 b = Quaternion.Euler(0.0f, 0.0f, 90.0f) * this.branches[i].dir.normalized * this.branches[i].weight * size * 0.5f;
      
      GL.Vertex3 (dst.x, dst.y, dst.z);
      GL.Vertex3 (dst.x + a.x + b.x, dst.y + a.y + b.y, dst.z + a.z + b.z);
      GL.Vertex3 (dst.x + a.x - b.x, dst.y + a.y - b.y, dst.z + a.z - b.z);
    }
    GL.End();
  }

}

public class Branch {
  public int level;
  public Branch parent;
  public YulanTree tree;
  public List <Branch> child;
  public Vector3 pos;
  public Vector3 dir;
  public float angle;
  public float length;
  public float weight;

  public Branch (Branch parent, float weight, int sibling, Vector3 light, float light_intensity) {
    this.parent = parent;
    this.level = parent.level + 1;
    this.weight = weight;
    this.child = new List<Branch>();
    this.angle = parent.angle;
    this.length = parent.length;
    
    this.pos = parent.pos + parent.dir;

    //calc angle
    this.dir = Quaternion.Euler(0.0f, 0.0f,  (-1 * (angle / 2.0f) + (this.angle / (sibling - 1) * (parent.child.Count))) * Random.Range(0.6f, 1.0f)) * (parent.dir);
    if (light != Vector3.zero) {
      this.weight *= Mathf.Pow(Mathf.Cos(Vector3.Angle (this.dir, light * -1) / 2.0f * Mathf.PI / 180.0f), light_intensity);
    }
    
    this.dir = this.dir.normalized * this.length * this.weight * Random.Range(1.0f, 2.0f);
    
    
    this.tree = parent.tree;
  }
  
  // root branch
  public Branch (YulanTree tree, Vector3 direction, float length, float angle) {
    this.parent = null;
    this.level = 0;
    this.weight = 1;
    this.child = new List<Branch>();
    this.pos = Vector3.zero;
    this.dir = direction;
    this.length = length;
    this.angle = angle;
    this.tree = tree;
  }


}

}