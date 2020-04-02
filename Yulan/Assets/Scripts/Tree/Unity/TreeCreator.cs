using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevY.Yulan.Unity {
public class TreeCreator : MonoBehaviour
{

  public int intensity =5;
  public float angle = 45.0f;
  public float length = 1;

  public int child = 3;
  public int sprig = 1;

  public Material mat;

  public Material leaf_mat;
  public Material joint_mat;

  public Camera cam;

  public Transform sun;
  public float sunIntensity = 8;

  public Transform wind;

  public Sprite leaf;

  YulanTree tree;

  // Start is called before the first frame update
  void Start()
  {
    tree = YulanTree.Create(this.transform, Vector3.zero, this.intensity, this.length, this.angle, 7, this.cam.transform, this.sun.forward, this.sunIntensity, this.leaf);

    tree.MakeTree(this.child, this.sprig);


    tree.Shaking (wind, 0.5f, 50f);
    
    Debug.Log ("cam.right:"+this.cam.transform.right);

    //StartCoroutine (tree.RenderLine(this.transform, mat));

  }

  void OnRenderObject() {
    if (tree==null) return;
    
    
    
    
    GL.PushMatrix();
    //GL.MultMatrix (transform.localToWorldMatrix);
    tree.RenderTree(this.cam, this.mat);
    
    tree.RenderJoint(this.cam, this.joint_mat, this.mat.GetColor("_Color"));

    tree.RenderLeaf (this.leaf_mat, leaf.texture, Color.white);

     //if (showFlower) tree.RenderFlower();
    
    GL.PopMatrix();
    
  }
}
}