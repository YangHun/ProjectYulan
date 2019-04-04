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
  public List<Branch> branches = new List<Branch>();

  public YulanTree (Vector3 start, int intensity, float length, float angle) {
    this.intensity = intensity;
    this.length = length;
    this.angle = angle;

    this.root = new Branch(this, Vector3.up, length, angle);
    this.branches.Add(root);
  }

  public void MakeTree(int child = 2, int sprig = 2) {
    this.Branching (this.root, child, sprig);
  }

  private void Branching (Branch parent, int childcount = 2, int sprigcount = 2) {
    if (parent.level >= this.intensity) return;
    int cc = Random.Range(1,childcount);

    for (int i = 0; i < cc; i++) {
      Branch b = new Branch (parent, cc, 1 - ((float)parent.level /(this.intensity + 1) / 2.0f) );
      parent.child.Add (b);
      this.branches.Add (b);
      if (b.level > 0) this.Sprigging (b, childcount, sprigcount);
      Branching (b, childcount, sprigcount);
    }

  }

  private void Sprigging (Branch b, int childcount = 2, int sprigcount = 2) {
    if (b.level >= this.intensity) return;
    for (int i = 0; i < sprigcount; i++) {
      Branch s = new Sprig (b, sprigcount, 1 - ((float)b.level /(this.intensity + 1)) );
      b.sprig.Add(s);
      this.branches.Add(s);
      Branching (s, childcount, sprigcount);
      Sprigging (s, childcount, sprigcount);
    }
  }

  public void RenderTree() {
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

}

public class Branch {
  public int level;
  public Branch parent;
  public YulanTree tree;
  public List <Branch> child;
  public List <Branch> sprig;
  public Vector3 pos;
  public Vector3 dir;

  public float length;
  public float angle;

  public float world_angle;

  public Branch (Branch parent, int childcount, float weight) {
    this.parent = parent;
    this.level = parent.level + 1;
    this.child = new List<Branch>();
    this.sprig = new List<Branch>();
    this.angle = parent.angle * 1.2f;
    this.length = parent.length;
    this.tree = parent.tree;

    this.pos = parent.pos + parent.dir;

    //this.dir = Quaternion.Euler(0.0f, 0.0f,  (-1 * (angle / 2.0f) + (this.angle / (childcount - 1) * parent.child.Count))) * (parent.dir) * weight;
    this.dir = Quaternion.Euler(0.0f, 0.0f,  (-1 * (angle / 2.0f)) ) * (parent.dir);

    this.dir = this.WorldDir (this, childcount) * weight;


    //this.dir = Quaternuion

  }

  protected Vector3 WorldDir (Branch b, int childcount) {
    Vector3 result = Vector3.zero;

    Vector3 normal = b.parent.dir.normalized;
    Vector3 target = b.dir;

    if (childcount == 1)  return b.parent.dir.normalized;



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


  }
}


public class Sprig : Branch {
  public Sprig (Branch parent, int childcount, float weight) : base (parent, childcount, weight) {
    this.parent = parent;
    this.level = parent.level + 1;
    this.child = new List<Branch>();
    this.angle = parent.angle;
    this.length = parent.length;
    this.tree = parent.tree;

    this.pos = parent.pos + parent.dir * (((float)parent.sprig.Count + 1) / (childcount + 1));


    this.dir = Quaternion.Euler(0.0f, 0.0f,  ((angle / childcount)) ) * (parent.dir);
    this.dir = this.WorldDir (this, childcount)  * weight;


  }
}

}