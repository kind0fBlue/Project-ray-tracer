using System;

namespace RayTracer
{
    /// <summary>
    /// Class to represent an (infinite) plane in a scene.
    /// </summary>
    public class Sphere : SceneEntity
    {
        private Vector3 center;
        private double radius;
        private Material material;

        /// <summary>
        /// Construct a sphere given its center point and a radius.
        /// </summary>
        /// <param name="center">Center of the sphere</param>
        /// <param name="radius">Radius of the spher</param>
        /// <param name="material">Material assigned to the sphere</param>
        public Sphere(Vector3 center, double radius, Material material)
        {
            this.center = center;
            this.radius = radius;
            this.material = material;
        }

        /// <summary>
        /// Determine if a ray intersects with the sphere, and if so, return hit data.
        /// </summary>
        /// <param name="ray">Ray to check</param>
        /// <returns>Hit data (or null if no intersection)</returns>
        public RayHit Intersect(Ray ray)
        {
            // Write your code here...
            double t0, t1;

            /*
            Vector3 L = center - ray.Origin; 
            double tca = L.Dot(ray.Direction);

            // if (tca < 0) is behind ray;
            if (tca < 0) {
                return null;
            }

            double d2 = L.Dot(L) - tca * tca; 
            if (Math.Abs(d2) > (radius*radius)) {     //overshoot
                return null; 
            }
            

            double thc = Math.Sqrt(radius*radius - Math.Abs(d2)); 
            t0 = tca - thc; 
            t1 = tca + thc;

            
            if (t0 > t1) {
                var tempswap = t0;
                t0 = t1;
                t1 = tempswap;
            } 
 
            if (t0 < 0) { 
                t0 = t1;  //if t0 is negative, let's use t1 instead 
                if (t0 < 0) return null;  //both t0 and t1 are negative 
            } 
            
 
            var t = t0;

            Vector3 position = ray.Origin + t*ray.Direction;
            Vector3 incident = ray.Direction;
            Vector3 normal = (center-position).Normalized();

            return new RayHit(position, normal, incident, this.material);
            */

            Vector3 L = ray.Origin - center;
            double a = ray.Direction.Dot(ray.Direction);
            double b = 2 * ray.Direction.Dot(L);
            double c = L.Dot(L) - radius*radius;
            double discr = b * b - 4 * a * c;
            if (discr < 0) {
                return null;
            }
            else if (discr == 0d) {
                t0 = - 0.5 * b / a;
                t1 = - 0.5 * b / a;

            }
            else {
                double q;
                if (b > 0) {
                    q = -0.5 * (b + Math.Sqrt(discr));
                }
                else {
                    q = -0.5 * (b - Math.Sqrt(discr));

                }
                t0 = q/a;
                t1 = c/q;

                if (t0 > t1) {
                    double temp = t0;
                    t0 = t1;
                    t1 = temp;

                }

                
             
            }

                if (t0 < 0) { 
                t0 = t1;  //if t0 is negative, let's use t1 instead 
                if (t0 < 0) return null;  //both t0 and t1 are negative 
                }
                Vector3 intersection = ray.Origin + t0*ray.Direction;
                Vector3 normal = (intersection - center).Normalized();
                return new RayHit(intersection, normal, ray.Direction, this.material);
                
            

        }

        /// <summary>
        /// The material of the sphere.
        /// </summary>
        public Material Material { get { return this.material; } }

      
    }

}
