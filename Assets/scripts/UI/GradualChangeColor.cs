using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//颜色渐变
public class GradualChangeColor : BaseMeshEffect {

    public Color colorLeft;
    public Color colorRight;
 
        private static void setColor(List<UIVertex> verts, int index, Color32 c)
        {
            UIVertex vertex = verts[index];
            vertex.color = c;
            verts[index] = vertex;
        }
 
        public void ModifyVertices(List<UIVertex> verts)
        {
            for (int i = 0; i < verts.Count; i += 6)
            {
                setColor(verts, i + 0, colorLeft);
                setColor(verts, i + 1, colorLeft);
                setColor(verts, i + 2, colorRight);
                setColor(verts, i + 3, colorRight);
 
                setColor(verts, i + 4, colorRight);
                setColor(verts, i + 5, colorLeft);
            }
        }
 
        #region implemented abstract members of BaseMeshEffect
 
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!this.IsActive())
            {
                return;
            }
            List<UIVertex> verts = new List<UIVertex>(vh.currentVertCount);
            vh.GetUIVertexStream(verts);
 
            ModifyVertices(verts);
 
            vh.Clear();
            vh.AddUIVertexTriangleStream(verts);
        }
        #endregion
}
