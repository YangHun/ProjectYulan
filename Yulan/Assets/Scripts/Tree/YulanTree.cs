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
  public int nodes;

  public YulanTree (Vector3 start, int intensity, float length, float angle, Vector4 sunlight, float sun_intensity) {
    this.length = length;
    this.angle = angle;
    this.intensity = intensity;
    this.light = new Vector4 (sunlight.x, sunlight.y, sunlight.z, sun_intensity);
    this.root = new Branch (this, start, length, angle);
    this.nodes = 1;
  }

  public void MakeCompleteTree () {
    this.Branching (this.root);
    Debug.LogFormat ("# of nodes in this tree: {0}", this.nodes);
  }

  private void Branching (Branch parent, int childcount = 2, bool complete = true) {
    if (parent.level >= this.intensity) return;
    int cc = childcount;
    if (!complete) cc = Random.Range(2, childcount);
    for (int i = 0; i < cc; i++) {
      Branch b = new Branch (parent, ( 1 - (float)parent.level / this.intensity ), cc, new Vector3 (this.light.x, this.light.y, this.light.z), this.light.w);
      b.tree.nodes += 1;
      parent.child.Add (b);
      Branching (b, childcount, complete);
      //Debug.LogFormat("branch {0}_{1} is created", b.level, parent.child.Count);
    }
  }

  public void RenderTree () {
    //stem
    GL.Color (Color.white);
    
    Vector3 src = this.root.pos;
    Vector3 dst = this.root.pos + this.root.dir;
    GL.Vertex3 (src.x, src.y, src.z);
    GL.Vertex3 (dst.x, dst.y, dst.z);

    this.Rendering(this.root);
  }

  private void Rendering (Branch parent) {
    if (parent.child.Count < 1) return;
    for (int i = 0; i < parent.child.Count; i++) {
      GL.Color (Color.white);
      Vector3 src = parent.child[i].pos;
      Vector3 dst = parent.child[i].pos + parent.child[i].dir;
      GL.Vertex3 (src.x, src.y, src.z);
      GL.Vertex3 (dst.x, dst.y, dst.z);
      this.Rendering (parent.child[i]);
    }
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
    this.dir = Quaternion.Euler(0.0f, 0.0f,  (-1 * (angle / 2.0f) + (this.angle / (sibling - 1) * (parent.child.Count))) * Random.Range(0.6f, 1.0f)) * (parent.dir);
    this.dir = this.dir.normalized * this.length * this.weight * Random.Range(1.0f, 2.0f);
    if (light != Vector3.zero) this.dir *= Mathf.Pow(Mathf.Cos(Vector3.Angle (this.dir, light * -1) / 2.0f * Mathf.PI / 180.0f), light_intensity);
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