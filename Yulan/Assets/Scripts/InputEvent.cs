using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevY.Yulan.Unity {
public class InputEvent : MonoBehaviour
{
  public Camera cam;
  public void TouchBranch (RaycastHit hit) {
    Branch b = hit.collider.GetComponent<Branch>();
    if (b == null) return;
    b.Shake (cam.transform, 0.5f, 1.0f);
  }

}
}