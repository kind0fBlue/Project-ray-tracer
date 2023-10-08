using System;

namespace RayTracer
{
    /// <summary>
    /// Class to represent a triangle in a scene represented by three vertices.
    /// </summary>
    public class Triangle : SceneEntity
    {
        private Vector3 v0, v1, v2;
        private Material material;

        /// <summary>
        /// Construct a triangle object given three vertices.
        /// </summary>
        /// <param name="v0">First vertex position</param>
        /// <param name="v1">Second vertex position</param>
        /// <param name="v2">Third vertex position</param>
        /// <param name="material">Material assigned to the triangle</param>
        public Triangle(Vector3 v0, Vector3 v1, Vector3 v2, Material material)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
            this.material = material;
        }

        /// <summary>
        /// Determine if a ray intersects with the triangle, and if so, return hit data.
        /// </summary>
        /// <param name="ray">Ray to check</param>
        /// <returns>Hit data (or null if no intersection)</returns>
        public RayHit Intersect(Ray ray)
        {
            // Write your code here...
            Vector3 v0v1 = v1-v0;
            Vector3 v0v2 = v2-v0;

            Vector3 N = v0v1.Cross(v0v2); //Normal!!

            double NdotRayDir = N.Dot(ray.Direction); //ray parallel
            if (Math.Abs(NdotRayDir) < Double.Epsilon) {  
                return null;
            }

            double d = - N.Dot(v0); 
            double t = -(N.Dot(ray.Origin) + d) / NdotRayDir;

            if (t <= 0) {     //traingle behind ray
                return null;
            }

            Vector3 P = ray.Origin + t * ray.Direction;   // Intersection!!

            //Inside outside test
            Vector3 C;

            Vector3 ed0 = v1 - v0; 
            Vector3 vecp0 = P - v0; 
            C = ed0.Cross(vecp0); 
            if (N.Dot(C) < 0) {
                return null; 
            }

            Vector3 ed1 = v2 - v1; 
            Vector3 vecp1 = P - v1; 
            C = ed1.Cross(vecp1); 
            if (N.Dot(C) < 0) {
                return null; 
            }

            Vector3 ed2 = v0 - v2; 
            Vector3 vecp2 = P - v2; 
            C = ed2.Cross(vecp2); 
            if (N.Dot(C) < 0) {
                return null; 
            }

            
            return new RayHit(P, N, ray.Direction, this.material);
        }

        /// <summary>
        /// The material of the triangle.
        /// </summary>
        public Material Material { get { return this.material; } }

        public Vector3 getV0() {
            return this.v0;
        }

        public Vector3 getV1() {
            return this.v1;
        }

        public Vector3 getV2() {
            return this.v2;
        }

        
    }

}
