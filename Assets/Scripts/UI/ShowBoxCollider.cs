using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
 
public class ShowBoxCollider : MonoBehaviour
{
    void OnRenderObject()
    {
        var colliders = gameObject.GetComponents<Collider>();
        if (colliders == null)
        {
            return;
        }
        //创建并设置线条材质
        CreateLineMaterial();
        lineMaterial.SetPass(0);
        GL.PushMatrix();
        //这里无需将矩阵从本地坐标转化为世界左边
       //GL.MultMatrix(transform.localToWorldMatrix);
 
        for (int i = 0; i < colliders.Length; i++)
        {
            var col = colliders[i];
            //获取本物体对象在世界范围内的中心点位置  col.center是本地坐标位置
            var c = col.bounds.center;
            //collider大小
            var size = col.bounds.size;
            float rx = size.x / 2f;
            float ry = size.y / 2f;
            float rz = size.z / 2f;
            //获取collider边界的8个顶点位置
            Vector3 p0, p1, p2, p3;
            Vector3 p4, p5, p6, p7;
            p0 = c + new Vector3(-rx, -ry, rz);
            p1 = c + new Vector3(rx, -ry, rz);
            p2 = c + new Vector3(rx, -ry, -rz);
            p3 = c + new Vector3(-rx, -ry, -rz);
 
            p4 = c + new Vector3(-rx, ry, rz);
            p5 = c + new Vector3(rx, ry, rz);
            p6 = c + new Vector3(rx, ry, -rz);
            p7 = c + new Vector3(-rx, ry, -rz);
 
            //画线
            GL.Begin(GL.LINES);
            GL.Color(Color.white);
            GL.Vertex(p0);
            GL.Vertex(p1);
            GL.End();
 
            GL.Begin(GL.LINES);
            GL.Color(Color.white);
            GL.Vertex(p1);
            GL.Vertex(p2);
            GL.End();
 
            GL.Begin(GL.LINES);
            GL.Color(Color.white);
            GL.Vertex(p2);
            GL.Vertex(p3);
            GL.End();
 
            GL.Begin(GL.LINES);
            GL.Color(Color.white);
            GL.Vertex(p0);
            GL.Vertex(p3);
            GL.End();
 
            GL.Begin(GL.LINES);
            GL.Color(Color.white);
            GL.Vertex(p4);
            GL.Vertex(p5);
            GL.End();
 
            GL.Begin(GL.LINES);
            GL.Color(Color.white);
            GL.Vertex(p5);
            GL.Vertex(p6);
            GL.End();
 
            GL.Begin(GL.LINES);
            GL.Color(Color.white);
            GL.Vertex(p6);
            GL.Vertex(p7);
            GL.End();
 
            GL.Begin(GL.LINES);
            GL.Color(Color.white);
            GL.Vertex(p4);
            GL.Vertex(p7);
            GL.End();
 
            GL.Begin(GL.LINES);
            GL.Color(Color.white);
            GL.Vertex(p0);
            GL.Vertex(p4);
            GL.End();
 
            GL.Begin(GL.LINES);
            GL.Color(Color.white);
            GL.Vertex(p1);
            GL.Vertex(p5);
            GL.End();
 
            GL.Begin(GL.LINES);
            GL.Color(Color.white);
            GL.Vertex(p2);
            GL.Vertex(p6);
            GL.End();
 
            GL.Begin(GL.LINES);
            GL.Color(Color.white);
            GL.Vertex(p3);
            GL.Vertex(p7);
            GL.End();
        }
        GL.PopMatrix();
    }
 
    
    static Material lineMaterial;
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity3d使用该默认的Shader作为线条材质  
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // 开启 alpha blending  
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // 开启背面遮挡  
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes  
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }
}