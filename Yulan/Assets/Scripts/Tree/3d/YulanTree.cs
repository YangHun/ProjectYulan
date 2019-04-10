using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevY.Yulan.Unity {
public class YulanTree : MonoBehaviour
{
  public int intensity;
  public float angle;
  public float length;
  
  public Branch root;
  public List<Branch> branches = new List<Branch>();

  public int smoothstep;

  public Vector3 sun;
  public float sunIntensity;

  public List<GameObject> objs = new List<GameObject>();

  public Transform wind;

  protected Coroutine shaking;
  protected WaitForSeconds wait = new WaitForSeconds (1.0f);

  #region Messages

  public void Shaking(Transform wind, float time) {
    this.wind = wind;
    this.shaking = StartCoroutine (ShakeTree(time));
  }

  private IEnumerator ShakeTree(float duration) {
    float timer = 0.0f;
    float angle = Vector3.Angle (wind.forward, this.root.dir);

    angle = Mathf.Sin (angle);
    while (true) {
      if (timer > duration) timer = 0.0f;
      
      for (int i = 0; i < this.root.child.Count; i++ ){

        float a = (timer < duration / 2.0f)? angle * (duration / 2.0f) : (-1) * angle * (duration / 2.0f ) ; 

        a *= Time.fixedDeltaTime;

        this.root.child[i].transform.Rotate(wind.right, a);
        yield return null;

      }
    
      timer += Time.fixedDeltaTime;

    }
  }

  #endregion


  #region Creation and Rendering
  public static YulanTree Create (Transform parent, Vector3 start, int intensity, float length, float angle, int smoothstep, Transform cam, Vector3 sun, float sunIntensity) {
    
    GameObject o = new GameObject("YulanTree");
    o.transform.SetParent (parent, false);
    
    YulanTree yulan = o.AddComponent<YulanTree>();

    yulan.intensity = intensity;
    yulan.length = length;
    yulan.angle = angle;

    yulan.sun = sun;
    yulan.sunIntensity = sunIntensity;

    yulan.transform.SetParent (parent, false);
    yulan.smoothstep = smoothstep;

    yulan.root = Branch.Create(yulan, parent.transform.up, length, angle, cam.right);
    yulan.branches.Add(yulan.root);

    return yulan;
  }

  public void MakeTree(int child = 2, int sprig = 2) {
    this.Branching (this.root, child, sprig);
    Debug.Log ("# of branches:"+this.branches.Count);
  }

  private void Branching (Branch parent, int childcount = 2, int sprigcount = 2) {
    if (parent.level >= this.intensity) return;
    int cc = childcount;
    float w = 1 - ((float)parent.level /(this.intensity+1));
    for (int i = 0; i < cc; i++) {
      Branch b = Branch.Create (parent, cc, w );
      parent.child.Add (b);
      this.branches.Add (b);

      if (b.level > 0) this.Sprigging (b, w, childcount, sprigcount);
      Branching (b, childcount, sprigcount);
    }

  }

  private void Sprigging (Branch b, float weight, int childcount = 2, int sprigcount = 2) {
    if (b.level > this.intensity - 1 || b.level < 2) return;
    for (int i = 0; i < sprigcount; i++) {
      Branch s = Sprig.Create (b, sprigcount, weight / 2.0f);
      b.sprig.Add(s);
      this.branches.Add(s);
      Branching (s, 2, sprigcount);
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

    float w = 0.025f;    


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
    Vector3 src = b.transform.position;
    Vector3 dst = (b.transform.rotation * b.dir) + src ;

    float start = b.weight * w * (1 - ((float)b.level / (this.intensity+2) ) ) ;
    float end = b.weight * w * (1 - ((float)(b.level + 1) / (this.intensity+2) ));;

    for (int i = 1; i < b.smoothsteps.Length; i++) {
      Vector3 s = src + b.transform.rotation *(Vector3)(this.transform.localToWorldMatrix * (b.smoothsteps[i - 1]));
      Vector3 d = src + b.transform.rotation *(Vector3)(this.transform.localToWorldMatrix * (b.smoothsteps[i]));

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

  #endregion
}
}