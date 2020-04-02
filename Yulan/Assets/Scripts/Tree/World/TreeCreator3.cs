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

  public Camera cam;

  public Transform sun;
  public float sunIntensity;

  YulanTree tree;

  // Start is called before the first frame update
  void Start()
  {

    
    Debug.Log ("cam.right:"+this.cam.transform.right);

    //StartCoroutine (tree.RenderLine(this.transform, mat));

  }

  public void Generate()
  {
    tree = new YulanTree(this.transform, Vector3.zero, this.intensity, this.length, this.angle, 7, this.cam.transform, this.sun.forward, this.sunIntensity);

    tree.MakeTree(this.child, this.sprig);
  }

  void OnRenderObject() {
    if (tree==null) return;
    mat.SetPass(0);
    
    //GL.MultMatrix (transform.localToWorldMatrix);
    tree.RenderTree(this.cam);
    //if (showFlower) tree.RenderFlower();
    
    
  }

}
}
