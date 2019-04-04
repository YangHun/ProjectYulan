using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevY.Yulan.World{
public class TreeCreator3 : MonoBehaviour
{
  public int intensity =6;
  public float angle = 60.0f;
  public float length = 1;

  public int child = 2;
  public int sprig = 3;

  public Material mat;

  YulanTree tree;

  // Start is called before the first frame update
  void Start()
  {
    tree = new YulanTree(Vector3.zero, this.intensity, this.length, this.angle);

    tree.MakeTree(this.child, this.sprig);
  }

  void OnRenderObject() {
    if (tree==null) return;
    mat.SetPass(0);
    GL.PushMatrix();
    GL.MultMatrix (transform.localToWorldMatrix);

    tree.RenderTree();
    //if (showFlower) tree.RenderFlower();
    
    GL.PopMatrix();
  }

}
}
