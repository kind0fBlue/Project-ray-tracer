using System;
using System.Collections.Generic;

namespace RayTracer
{
    /// <summary>
    /// Class to represent a ray traced scene, including the objects,
    /// light sources, and associated rendering logic.
    /// </summary>
    public class Scene
    {
        private SceneOptions options;
        private ISet<SceneEntity> entities;
        private ISet<PointLight> lights;

        float fieldOfView = 60f;
        private double _imagePlaneHeight;
        private double _imagePlaneWidth;


        /// <summary>
        /// Construct a new scene with provided options.
        /// </summary>
        /// <param name="options">Options data</param>
        public Scene(SceneOptions options = new SceneOptions())
        {
            this.options = options;
            this.entities = new HashSet<SceneEntity>();
            this.lights = new HashSet<PointLight>();
        }

        /// <summary>
        /// Add an entity to the scene that should be rendered.
        /// </summary>
        /// <param name="entity">Entity object</param>
        public void AddEntity(SceneEntity entity)
        {
            this.entities.Add(entity);
        }

        /// <summary>
        /// Add a point light to the scene that should be computed.
        /// </summary>
        /// <param name="light">Light structure</param>
        public void AddPointLight(PointLight light)
        {
            this.lights.Add(light);
        }

        /// <summary>
        /// Render the scene to an output image. This is where the bulk
        /// of your ray tracing logic should go... though you may wish to
        /// break it down into multiple functions as it gets more complex!
        /// </summary>
        /// <param name="outputImage">Image to store render output</param>
        public void Render(Image outputImage)
        {
            ComputeWorldImageBounds(outputImage);
            // Begin writing your code here...

                                

            
            for (var y = 0; y < outputImage.Height; y++) {
                for (var x = 0; x < outputImage.Width; x++) {
                    Color sumColor = new Color(0, 0, 0);
                    for (var z = 0; z < options.AAMultiplier; z ++){
                        for (var h = 0; h < options.AAMultiplier; h ++){

                            var ray = PixelRay((x*options.AAMultiplier +z+0.5f)/options.AAMultiplier, (y*options.AAMultiplier +h+0.5f)/options.AAMultiplier, outputImage, options.AAMultiplier);
                            
                            sumColor += castRay(ray.Origin, ray.Direction, entities, lights, 0, 10, null, outputImage, x, y);
                            
                        }

                    }
                    outputImage.SetPixel(x, y, sumColor/((options.AAMultiplier)*(options.AAMultiplier)));
                    


                 
                }

                
                

                    

            }

    }

    private void ComputeWorldImageBounds(Image image)
    {
        var aspectRatio = (float)image.Width / image.Height;
        var fovLength = Math.Tan(this.fieldOfView / 2f * Math.PI / 180.0) * 2f;

        // Note: We are using vertical FOV here.
        this._imagePlaneWidth = fovLength;
        this._imagePlaneHeight = this._imagePlaneWidth / aspectRatio;
    }



    private Vector3 NormalizedImageToWorldCoord(double x, double y, int aa)
    {
        return new Vector3(
            this._imagePlaneWidth * (x - 0.5f) ,
            this._imagePlaneHeight * (0.5f - y) ,
            1.0f); // Image plane is 1 unit from camera.
    }

    private Ray PixelRay(double x, double y, Image image, int aa)
    {
        var normX = (x) / (image.Width);
        var normY = (y) / (image.Height);

        var worldPixelCoord = NormalizedImageToWorldCoord(normX, normY, aa);

        return new Ray(new Vector3(0, 0, 0), worldPixelCoord);
    }

    private (Vector3, bool) reflect(Vector3 incident, Vector3 normal) 
    { 
        return (incident - 2 * incident.Dot(normal) * normal, (incident.Dot(normal) > 0f)); 
    }


    private (Vector3, bool) refract(Vector3 incident, Vector3 normal, double ior) { 
        double cosi = incident.Dot(normal);
        double etai = 1; 
        double etat = ior; 
        Vector3 n = normal;
        bool isInside = false; 
        if (cosi < 0) { 
            
            cosi = -cosi; 
        } 
        else {
            Console.WriteLine("is inside");
            isInside = true;
            double temp = etai;
            etai = etat;
            etat = temp;

            n = -normal;
             
        } 
        double eta = etai / etat; 
        double k = 1 - eta * eta * (1 - cosi * cosi); 
     
            
            
            return (eta * incident + (eta * cosi - Math.Sqrt(k)) * n, isInside);
       
        
       
    } 

    private double fresnel(Vector3 incident, Vector3 normal, double ior) 
    { 
        double kr;
        double cosi = incident.Dot(normal);
        double etai = 1, etat = ior; 
        if (cosi > 0) { 

            double temp = etai;
            etai = etat;
            etat = temp; 
        } 

        // Compute sini using Snell's law
        double sint = etai / etat * Math.Sqrt(Math.Max(0f, 1 - cosi * cosi)); 
        // Total internal reflection
        if (sint >= 1) { 
            kr = 1; 
        } 
        else { 
            double cost = Math.Sqrt(Math.Max(0f, 1 - sint * sint)); 
            cosi = Math.Abs(cosi); 
            double Rs = ((etat * cosi) - (etai * cost)) / ((etat * cosi) + (etai * cost)); 
            double Rp = ((etai * cosi) - (etat * cost)) / ((etai * cosi) + (etat * cost)); 
            kr = (Rs * Rs + Rp * Rp) / 2; 
        } 
    // As a consequence of the conservation of energy, transmittance is given by:
    // kt = 1 - kr;
        return kr;
    }

 
    private Vector3 bayCentric(Triangle triangle, Vector3 p) {
        Vector3 v0 = triangle.getV0();
        Vector3 v1 = triangle.getV1();
        Vector3 v2 = triangle.getV2();

        Vector3 v0v1 = v1 - v0; 
        Vector3 v0v2 = v2 - v0;

        Vector3 N = v0v1.Cross(v0v2);
        double area = N.Length() / 2;

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






    private Color castRay(Vector3 origin, Vector3 dir, ISet<SceneEntity> entities, ISet<PointLight> lights, int depth, int maxDepth,
    SceneEntity lastEntity, Image outputImage, int x, int y) {

        Color sumColor = new Color(0, 0, 0);

        if (depth > maxDepth) {
            return sumColor;
        }

        var ray = new Ray(origin, dir.Normalized());
        RayHit nearestHit = null;
        
       
        foreach (SceneEntity entity in entities) {
            
                if (entity != lastEntity) {
                var hit = entity.Intersect(ray);
                if (hit != null && ((hit?.Position-ray.Origin)?.LengthSq() < (nearestHit?.Position-ray.Origin)?.LengthSq() || nearestHit == null)) {

                   
                    
                    if (entity.Material.Type == Material.MaterialType.Reflective) {
                        
                        
                        Vector3 R = reflect(hit.Incident,hit.Normal).Item1;
                        sumColor += castRay(hit.Position + hit.Normal*0.0001, R, entities, lights, depth++, maxDepth,entity ,outputImage, x, y);

                        
                        
                        
                    }
                    
                    
                    
                    
                    else if (entity.Material.Type == Material.MaterialType.Refractive) {

                                                
                       double kr = fresnel(hit.Incident, hit.Normal, entity.Material.RefractiveIndex); 
                       if (kr < 1) {
                            (Vector3, bool) Refr = refract(hit.Incident, hit.Normal, entity.Material.RefractiveIndex);
                            if (Refr.Item1.Z != -30000000000000) {

                                if (Refr.Item2) {
                                    sumColor += castRay(hit.Position + hit.Normal*0.5, Refr.Item1.Normalized(), entities, lights, depth++, maxDepth, entity,outputImage, x, y) * (1-kr);
                                }
                                else {
                                    sumColor += castRay(hit.Position - hit.Normal*0.5, Refr.Item1.Normalized(), entities, lights, depth++, maxDepth, entity,outputImage, x, y) * (1-kr);
                                }
                            
                            }
                       }

                       (Vector3, bool) R = reflect(hit.Incident,hit.Normal);
                        if (R.Item2) {
                                sumColor += castRay(hit.Position + hit.Normal*0.000, R.Item1.Normalized(), entities, lights, depth++, maxDepth, entity,outputImage, x, y) * kr;
                        }
                        else {
                                sumColor += castRay(hit.Position - hit.Normal*0.0001, R.Item1.Normalized(), entities, lights, depth++, maxDepth, entity,outputImage, x, y) * kr;
                        }
                        

                       
                        

                  
                       

                    }
                    else if (entity.Material.Type == Material.MaterialType.Glossy) {
                        sumColor = new Color(0, 0, 0);
                        Color diffuse = new Color(0, 0, 0);
                        foreach (PointLight light in lights) {
                                Vector3 lightRay = hit.Position - light.Position;
                                Vector3 lHat = lightRay.Normalized();
                                Color cl = light.Color;
                                Color cm = entity.Material.Color;

                                // stage 2.2 
                                bool inShadow = false;
                                var reverseOrigin = (hit.Position + hit.Normal*double.Epsilon);
                                var reverseLight = new Ray(reverseOrigin, -lHat);
                                foreach (SceneEntity entity2 in this.entities) {
                                    if (!ReferenceEquals(entity2, entity)) {
                                        var reverseHit = entity2.Intersect(reverseLight);
                                        
                                        if (reverseHit != null) {
                                            var reverseLen = (reverseHit.Position - hit.Position).LengthSq();
                                            if ((lightRay.LengthSq()) > reverseLen) {
                                                inShadow = true;
                                            }
                                        
                                        }
                                    }
                                }
                                 
                                if (!inShadow) {

                                double factor = Math.Max(hit.Normal.Normalized().Dot(-lHat), 0f);
                                diffuse = cl*cm*factor;
                                Vector3 R = reflect(hit.Incident,hit.Normal).Item1;
                                sumColor += (diffuse*1.5 + castRay(hit.Position + hit.Normal*0.0001, R, entities, lights, depth++, maxDepth,entity ,outputImage, x, y)*0.2*cl*factor);
                                }
                        }


                    }
                    
                    
                    else {

                        

                        
                        sumColor = new Color(0, 0, 0);
                        foreach (PointLight light in lights) {
                                Vector3 lightRay = hit.Position - light.Position;
                                Vector3 lHat = lightRay.Normalized();
                                Color cl = light.Color;
                                Color cm = entity.Material.Color;

                                // stage 2.2 
                                bool inShadow = false;
                                var reverseOrigin = (hit.Position + hit.Normal*double.Epsilon);
                                var reverseLight = new Ray(reverseOrigin, -lHat);
                                foreach (SceneEntity entity2 in this.entities) {
                                    if (!ReferenceEquals(entity2, entity)) {
                                        var reverseHit = entity2.Intersect(reverseLight);
                                        
                                        if (reverseHit != null) {
                                            var reverseLen = (reverseHit.Position - hit.Position).LengthSq();
                                            if ((lightRay.LengthSq()) > reverseLen) {
                                                inShadow = true;
                                            }
                                        
                                        }
                                    }
                                }
                                 
                                if (!inShadow) {

                                double factor = Math.Max(hit.Normal.Normalized().Dot(-lHat), 0f);
                                sumColor = sumColor + cl*cm*factor;
                                }
                        }
                                      
                              
                        
                        

                    }
                    
                    nearestHit = hit;
                    
                    
                    

                    
                    
                }
                }
                

        }
        return sumColor;
        

        
        


    }


}}
