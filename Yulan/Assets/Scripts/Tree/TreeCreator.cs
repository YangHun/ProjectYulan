using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevY.Yulan {
public class TreeCreator : MonoBehaviour
{
  public int intensity = 6;
  public int duration = 1;
  public float angle = 60.0f;
  public float width = 0.1f;
  public Material mat;
  private GameObject seed;
  void Awake() {
    this.seed = new GameObject();
    LineRenderer line = this.seed.AddComponent<LineRenderer>();
    line.material = this.mat;
    line.SetPosition(1, Vector3.up * duration);
    line.useWorldSpace = false;
    line.widthMultiplier = this.width;
    this.seed.name = "Seed";

    this.MakeBinTree(seed, 0);
  }

  void MakeBinTree (GameObject branch, int level) {
    if (level >= intensity) return;

    Vector3 start = branch.GetComponent<LineRenderer>().GetPosition(1);
    float weight = ( 1 - (float)level / intensity );

    GameObject left = new GameObject();
    left.transform.SetParent(branch.transform, false);
    LineRenderer ll = left.AddComponent<LineRenderer>();
    ll.SetPosition(1, Vector3.up * duration * weight * Random.Range(1.0f, 2.0f));
    ll.material = this.mat;
    ll.useWorldSpace = false;
    ll.widthMultiplier = this.width * weight;
    left.transform.localPosition = start;
    left.transform.Rotate(Vector3.forward * this.angle / 2.0f * -1);
    left.name = string.Format ("{0}_left", level);
    
    this.MakeBinTree (left, level + 1);

    GameObject right = new GameObject();
    right.transform.SetParent(branch.transform, false);
    LineRenderer rl = right.AddComponent<LineRenderer>();
    rl.SetPosition(1, Vector3.up * duration * weight * Random.Range(1.0f, 2.0f));
    rl.material = this.mat;
    rl.useWorldSpace = false;
    rl.widthMultiplier = this.width * weight;
    right.transform.localPosition = start;
    right.transform.Rotate(Vector3.forward * this.angle / 2.0f);
    right.name = string.Format ("{0}_right", level);

    this.MakeBinTree (right, level + 1);
  }

}
}