using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;


using TriangleNet.Meshing;
using TriangleNet.Geometry;
using TriangleNet.Tools;
using TriangleNet.IO;
using TriangleNet.Smoothing;

public class ClothWithBones : MonoBehaviour {

    GameObject Root;

    public SphereCollider sphereFirst;
    public SphereCollider sphereSecond;

    // Use this for initialization
    void Start () {

        var p = new Polygon();

        //Add the outer box contour with boundary marker 1.
        p.Add ( new Contour ( new Vertex [ 4 ]
       {
           new Vertex(-3,-3, 1),
           new Vertex(3, -3, 1),
           new Vertex(3, 3, 1),
           new Vertex(-3, 3, 1)
       }, 1 ) );

        var options = new ConstraintOptions() { ConformingDelaunay = true };
        var quality = new QualityOptions() {MinimumAngle = 25F  , MaximumArea = 0.04F};

        var triMesh =  (TriangleNet.Mesh)p.Triangulate(options, quality);

        var smoothing = new SimpleSmoother();
        smoothing.Smooth ( triMesh );

        triMesh.Refine ( quality, true );

        triMesh.Renumber ( );

        var vertices = triMesh.Vertices.Select(v=> new Vector3((float)v.x, (float)v.y, 0)).ToArray();
        var triangles = triMesh.Triangles.SelectMany(t=> t.vertices.Select(v=>v.id)).ToArray();//.Reverse()
        var normals = triMesh.Vertices.Select(v=>transform.forward).ToArray();

        var bounds = triMesh.bounds;
        var l = bounds.Left;
        var b = bounds.Bottom;
        var w = bounds.Width;
        var h = bounds.Height;
        var uvs = triMesh.Vertices.Select(v=> new Vector2( -(float)( (v.x - l) / w),(float)( (v.y - b) / h)) ).ToArray();

        //SkinnedMeshRenderer
        var skinnedRenderer = GetComponent<SkinnedMeshRenderer>();
        var uniMesh = new Mesh ( );

        uniMesh.vertices = vertices;
        uniMesh.triangles = triangles;
        uniMesh.uv = uvs;
        uniMesh.normals = normals;

        
        //Bones

        int boneIndex0 = 0;
        var bindposes = new List<Matrix4x4>();
        var bones = new List<Transform>();

        var fBoneWeights = new List<BoneWeight>();

        Root = new GameObject ( );
        Root.transform.parent = transform;
        Root.transform.localRotation = Quaternion.identity;

        var handler = new GameObject();
        handler.name = "Handler";
        handler.transform.parent = Root.transform;
        handler.transform.localRotation = Quaternion.identity;


        foreach(var vertex in triMesh.Vertices)//.Where(v=>v.y > 2.9F) )
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);//new GameObject();
            
            obj.transform.localScale = Vector3.one * 0.1F;
            var renderer = obj.GetComponent<MeshRenderer> ( );
            if ( renderer )
            {
                renderer.enabled = false;
            }
            var worldPosition = transform.TransformPoint(new Vector3 ( ( float ) vertex.x, ( float ) vertex.y, 0 ));
            obj.transform.position = worldPosition;
            if ( vertex.y > 2.9F )
            {
                obj.name = "HERE";
                obj.transform.parent = handler.transform;

            }
            else
            obj.transform.parent = Root.transform;

            bones.Add ( obj.transform );
            bindposes.Add ( obj.transform.worldToLocalMatrix * transform.localToWorldMatrix );

            var weight = new BoneWeight();

            weight.boneIndex0 = boneIndex0;
            weight.weight0 = 1.0F;
            fBoneWeights.Add ( weight );

            boneIndex0++;
        }

        uniMesh.boneWeights = fBoneWeights.ToArray ( );
        uniMesh.bindposes = bindposes.ToArray ( );

        skinnedRenderer.sharedMesh = uniMesh;
        skinnedRenderer.bones = bones.ToArray ( );
        skinnedRenderer.rootBone = Root.transform;

        //Cloth

        var cloth = GetComponent<Cloth>();
        if ( !cloth )
        {
            cloth = gameObject.AddComponent<Cloth> ( );
        }

        var coes = new List<ClothSkinningCoefficient>();

        foreach ( var v in vertices )
        {
            if ( v.y < 2.9F )
                coes.Add ( new ClothSkinningCoefficient ( ) { maxDistance = 10, collisionSphereDistance = 0.1F } );
            else
                coes.Add ( new ClothSkinningCoefficient ( ) { maxDistance = 0, collisionSphereDistance = 0.1F } );

        }

        cloth.coefficients = coes.ToArray ( );

        ClothSphereColliderPair pair = new ClothSphereColliderPair(sphereFirst, null);
        cloth.sphereColliders = new ClothSphereColliderPair [ ] { pair };

        //cloth.externalAcceleration = new Vector3 ( 0, -10, 0 );

        cloth.stretchingStiffness = 0.9F;
        cloth.bendingStiffness = 0.1F;

        cloth.collisionMassScale = 0.1F;
        cloth.friction = 1F;

        cloth.SetEnabledFading ( true );
        cloth.sleepThreshold = 0.1F;

        cloth.damping = 0.2F;


    }

}
