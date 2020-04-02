using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevY.Yulan.Unity {
public class LeafBud : MonoBehaviour
{
  public Branch parent;

  public float angle;

  
  public static LeafBud Create (Branch parent, Vector3 pos, float angle, Sprite sprite) {

    GameObject o = new GameObject("leaf");

    o.transform.SetParent (parent.transform, false);
    o.transform.position = pos;
    //SpriteRenderer r = o.AddComponent<SpriteRenderer>();
    //r.sprite = sprite;
    //r.sortingOrder = 10;

    LeafBud l = o.AddComponent<LeafBud>();
    l.parent = parent;

    l.angle = angle;

    Vector3 n = Vector3.Cross (parent.transform.forward, parent.parent.transform.forward);
    o.transform.Rotate(n,angle);

    parent.leaf.Add (l);

    //o.transform.LookAt (parent.transform.up + parent.transform.position);


    return l;
  }

  protected Vector3 WorldDir (LeafBud l, int childcount) {
    Vector3 result = Vector3.zero;

    Vector3 normal = Vector3.Cross(l.parent.dir, l.parent.parent.dir);
    Vector3 target = l.parent.dir;

    result = Quaternion.AngleAxis ( (Random.Range(1,10) % 2 == 0? 1 : -1) * l.angle, normal * -1) * target;

    return result.normalized;
  }

  public void RenderLeaf() {
    Vector3 src = this.transform.position;
    Vector3 dir = this.transform.forward * 2.0f;
    Vector3 normal = Vector3.Cross (this.transform.up, this.parent.transform.forward);

    Vector3 w = Quaternion.AngleAxis(90.0f, normal) * dir.normalized * 0.1f / 2.0f * 0.75f;
    dir *= 0.1f;

    //Debug.LogFormat("{0} / {1} / {2} / {3}", src, dir, normal, w);


    GL.Color (Color.white);
  
    GL.TexCoord2(0.0f, 0.0f);
    GL.Vertex3 (src.x + w.x, src.y + w.y, src.z + w.z);

    GL.TexCoord2(0.0f, 1.0f);
    GL.Vertex3 (src.x + dir.x + w.x, src.y + dir.y + w.y, src.z + dir.z + w.z);
    
    GL.TexCoord2(1.0f, 1.0f);
    GL.Vertex3 (src.x + dir.x - w.x, src.y + dir.y - w.y, src.z + dir.z - w.z);
    
    GL.TexCoord2(1.0f, 0.0f);
    GL.Vertex3 (src.x - w.x, src.y - w.y, src.z - w.z);
  }

}
}