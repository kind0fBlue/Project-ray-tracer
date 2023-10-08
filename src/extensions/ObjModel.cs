using System.IO;
using System;
using StbImageWriteSharp;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RayTracer
{
    /// <summary>
    /// Add-on option C. You should implement your solution in this class template.
    /// </summary>
    public class ObjModel : SceneEntity
    {
        private Material material;
        private List<Vector3> vertices = new List<Vector3>();
        private List<Vector3> normals = new List<Vector3>();

        private List<(Triangle, List<Vector3>)> faces = new List<(Triangle, List<Vector3>)>();

        /// <summary>
        /// Construct a new OBJ model.
        /// </summary>
        /// <param name="objFilePath">File path of .obj</param>
        /// <param name="offset">Vector each vertex should be offset by</param>
        /// <param name="scale">Uniform scale applied to each vertex</param>
        /// <param name="material">Material applied to the model</param>
        public ObjModel(string objFilePath, Vector3 offset, double scale, Material material)
        {
            this.material = material;

            

            // Here's some code to get you started reading the file...
            string[] lines = File.ReadAllLines(objFilePath);
            for (int i = 0; i < lines.Length; i++)
            {
                // The current line is lines[i]
                string[] words = lines[i].Split(' ');
                if (words[0] == "v"){
                    vertices.Add((new Vector3(Convert.ToDouble(words[1]), Convert.ToDouble(words[2]), Convert.ToDouble(words[3])))*scale+offset);
                    
                }
                if (words[0] == "vn"){
                    normals.Add(new Vector3(Convert.ToDouble(words[1]), Convert.ToDouble(words[2]), Convert.ToDouble(words[3])));
                    
                }
                if (words[0] == "f") {
                    List<Vector3> verts = new List<Vector3>();
                    List<Vector3> norms = new List<Vector3>();
                    for(int a = 1; a < 4; a++){
                        String[] index = words[a].Split('/');
                        
                        
                        
                        
                        verts.Add(vertices[Convert.ToInt16(index[0])-1]);
                        norms.Add(normals[Convert.ToInt16(index[0])-1]);
                        
                        
                    }

                    
                    Triangle tri = new Triangle(verts[0], verts[1], verts[2], this.material);
                    faces.Add((tri, norms));
                    

                }
            }

            

            


            

        }

        /// <summary>
        /// Given a ray, determine whether the ray hits the object
        /// and if so, return relevant hit data (otherwise null).
        /// </summary>
        /// <param name="ray">Ray data</param>
        /// <returns>Ray hit data, or null if no hit</returns>
        public RayHit Intersect(Ray ray)
        {
            // Write your code here...
            RayHit nearestHit = null;
            foreach ((Triangle, List<Vector3>) face in faces) {
                Triangle triangle = face.Item1;
                List<Vector3> norms = face.Item2;
                Vector3 avgNormal = (norms[0] + norms[1] + norms[2])/3;

                RayHit hit = triangle.Intersect(ray);
                if (hit != null && ((hit?.Position-ray.Origin)?.LengthSq() < (nearestHit?.Position-ray.Origin)?.LengthSq() || nearestHit == null)) {
                    hit.setNormal(avgNormal);
                    nearestHit = hit;
                    
                }

            }

            if (nearestHit != null) {
                return nearestHit;
            }
            else {
                return null;
            }


                
    }
        

        /// <summary>
        /// The material attached to this object.
        /// </summary>
        public Material Material { get { return this.material; } }

        private Vector3 bayCentric(Triangle triangle, Vector3 p, Ray ray) {
        Vector3 v0 = triangle.getV0();
        Vector3 v1 = triangle.getV1();
        Vector3 v2 = triangle.getV2();

        Vector3 v0v1 = v1 - v0; 
        Vector3 v0v2 = v2 - v0;

        Vector3 N = v0v1.Cross(v0v2);
        double area = N.Length() / 2;
        double denom = N.Dot(N);

        double NdotRayDirection = N.Dot(ray.Direction); 
        if (Math.Abs(NdotRayDirection) < Double.Epsilon) {  //almost 0 
            return default;
        }

        double d = N.Dot(v0);

        Vector3 edge1 = v2 - v1; 
        Vector3 vp1 = p - v1;
        Vector3 C = edge1.Cross(vp1); 
        double u = (C.Length() / 2) / area;

        Vector3 edge2 = v0 - v2; 
        Vector3 vp2 = p - v2;
        Vector3 C2 = edge2.Cross(vp2); 
        double v = (C2.Length() / 2) / area;  
    



        return new Vector3 (u, v, 1-u-v);
        }
    }

}
