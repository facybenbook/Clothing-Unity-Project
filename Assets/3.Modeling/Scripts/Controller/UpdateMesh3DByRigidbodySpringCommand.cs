using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Modeling
{
    using System.Linq;

    using strange.extensions.command.impl;
    using strange.extensions.context.api;

    using Polygon;


    using TriangleNet.Geometry;
    using TriangleNet.Meshing;
    using TriangleNet.Smoothing;
    using TriangleNet.Voronoi;
    using TriangleNet.Extensions;

    public class UpdateMesh3DByRigidbodySpringCommand : EventCommand
    {

        [Inject ( ContextKeys.CONTEXT_VIEW )]
        public GameObject contextView { get; set; }

        [Inject]
        public ISewSegmentPairLookup sewPairLookup { set; get; }

        [Inject]
        public IEntityLookup polyLookup { set; get; }

        const float KGSM = 0.2F;


        public override void Execute ( )
        {
            Debug.Log ( "simple sew execute!" );

            foreach ( var viewEntityPair in polyLookup.libs )
            {
                var polyView = viewEntityPair.Key;

                polyView.pointParent.gameObject.SetActive ( false );
                polyView.lineParent.gameObject.SetActive ( false );
                polyView.meshFilter.gameObject.SetActive ( false );

                var polyEntity = viewEntityPair.Value;

                var triMesh = polyEntity.GetTriangleMesh();

                //--------------------------------------------------------
                
                //Mesh Object
                if ( polyView.forwardSkin ) GameObject.Destroy ( polyView.forwardSkin.gameObject );

                var forwardObj = new GameObject("Forward Skin");
                forwardObj.transform.parent = polyView.transform;
                forwardObj.transform.localPosition = new Vector3 ( 0, 0, -0.3F );
                polyView.forwardSkin = forwardObj.AddComponent<SkinnedMeshRenderer> ( );
                polyView.forwardSkin.material = GameObject.Instantiate<Material> ( Resources.Load<Material> ( "Materials/Cloth Material" ) );


                if ( polyView.backSkin ) GameObject.Destroy ( polyView.backSkin.gameObject );


                var backObj = new GameObject("Back Skin");
                backObj.transform.parent = polyView.transform;
                backObj.transform.localPosition = new Vector3 ( 0, 0, 0.3F );
                polyView.backSkin = backObj.AddComponent<SkinnedMeshRenderer> ( );
                polyView.backSkin.material = GameObject.Instantiate<Material> ( Resources.Load<Material> ( "Materials/Cloth Material" ) );


                Dictionary<Point, Rigidbody> forwardRigids, backRigids;
                forwardRigids = UpdateRigidbodys ( polyView.forwardSkin, triMesh, true );
                backRigids = UpdateRigidbodys ( polyView.backSkin, triMesh, false );

                foreach ( var forwardPart in forwardRigids.Where ( pair => pair.Key.Label != 0 ) )
                {
                    var backPart = backRigids.First(pair => pair.Key.Label == forwardPart.Key.Label && pair.Key.ID == forwardPart.Key.ID);
                    SewRigidbody ( forwardPart.Value, backPart.Value );
                }

                contextView.GetComponent<Context.RootView> ( ).StartCoroutine ( WaitForSewing ( forwardRigids, backRigids ) );

            }

        }

        void SewRigidbody ( Rigidbody first, Rigidbody second )
        {
            var spring = first.gameObject.AddComponent<SpringJoint>();
            spring.connectedBody = second;
            spring.minDistance = 0F;
            spring.maxDistance = 0F;
            spring.spring = 8F;
            spring.damper = 0F;
            spring.autoConfigureConnectedAnchor = false;
            spring.enableCollision = false;
            spring.connectedAnchor = spring.anchor = Vector3.zero;
            spring.axis = Vector3.back;
            spring.tolerance = 0.01F;
            spring.enablePreprocessing = false;
        }

        int count = 0;
        int total = 64;
        IEnumerator WaitForSewing ( Dictionary<Point, Rigidbody> forwardRigids, Dictionary<Point, Rigidbody> backRigids )
        {
            count = total;
            while ( count > 0 )
            {
                var allForwardPoints = forwardRigids.Values.Select(r=>r.position);
                var allBackPoints = backRigids.Values.Select(r => r.position);
                var bounds = allForwardPoints.Union(allBackPoints).Bounds();
                var center = bounds.center;
                var factor = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z) * 0.25F;

                foreach ( var pointRigidbodyPair in
                forwardRigids.Union ( backRigids) )
                //forwardRigids.Where ( pair => pair.Key.Label == 0 ).Union ( backRigids.Where ( pair => pair.Key.Label == 0 ) ) )
                {
                    var rigidbody = pointRigidbodyPair.Value;
                    var vector = (rigidbody.position - center);
                    var dir = vector.normalized;
                    var dis = vector.magnitude;
                    float t = (float)count / total;
                    rigidbody.AddForce ( dir * t * factor * 0.1F / dis );
                }

                count--;
                yield return new WaitForFixedUpdate ( );
            }

            yield return null;
        }
        
        Dictionary<Point, Rigidbody> UpdateRigidbodys ( SkinnedMeshRenderer skineedRenderer, TriangleNet.Mesh triMesh, bool isForward )
        {
            var vertexLookup = new Dictionary<Point, Rigidbody> ( );

            var skinnedRoot = skineedRenderer.transform;
            ///-------------------------------------------------------------------------
            ///

            int boneIndex0 = 0;
            var bindposes = new List<Matrix4x4>();
            var bones = new List<Transform>();

            var fBoneWeights = new List<BoneWeight>();


            var voronoi = new StandardVoronoi(triMesh);

            var triAreas = triMesh.Triangles.Sum(t => t.Area());
            Debug.Log ( "Tri Areas : " + triAreas );

            foreach ( var face in voronoi.Faces )
            {
                var origins = face.GetAllVertices();

                if ( origins.Count ( ) > 0 )
                {
                    var center = face.generator;

                    var obj = GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Sphere);//new GameObject();
                    obj.name = "V_" + center.Label + "_" + center.ID;
                    obj.transform.localScale = Vector3.one * 0.005F;
                    var renderer = obj.GetComponent<MeshRenderer> ( );
                    if ( renderer )
                    {
                        renderer.enabled = false;
                    }

                    var worldPosition = skinnedRoot.transform.TransformPoint(new Vector3 ( ( float ) center.x, ( float ) center.y, 0 ));
                    obj.transform.position = worldPosition;
                    obj.transform.parent = skinnedRoot;// Root.transform;
                    bones.Add ( obj.transform );
                    bindposes.Add ( obj.transform.worldToLocalMatrix * skinnedRoot.localToWorldMatrix );

                    var weight = new BoneWeight();

                    weight.boneIndex0 = boneIndex0;
                    weight.weight0 = 1.0F;
                    fBoneWeights.Add ( weight );

                    var rigidbody = obj.AddComponent<Rigidbody>();
                    rigidbody.useGravity = false;
                    rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                    rigidbody.mass = origins.Area ( ) * KGSM;
                    rigidbody.drag = 4;
                    rigidbody.angularDrag = 2;

                   // if ( center.Label == 0 )
                        rigidbody.constraints ^= RigidbodyConstraints.FreezeRotation;

                    vertexLookup.Add ( center, rigidbody );

                    boneIndex0++;
                }

            }

            foreach ( var edge in triMesh.Edges )
            {
                var v0 =   triMesh.Vertices.ElementAt(edge.P0);
                var v1 =  triMesh.Vertices.ElementAt(edge.P1);
                bool isBounds = v0.Label != 0 && v1.Label != 0;
                var pt_0 = new Vector2((float) v0.x, (float)v0.y);
                var pt_1 = new Vector2((float) v1.x, (float)v1.y);
                var distance = Vector2.Distance(pt_0, pt_1);

                var rigid_0 =vertexLookup[v0];
                var rigid_1 = vertexLookup[v1];

                var spring = rigid_0.gameObject.AddComponent<SpringJoint>();
                spring.connectedBody = rigid_1;
                spring.minDistance = distance * .96F;
                spring.maxDistance = distance * 1.00F;
                spring.spring = isBounds ? 4F : 8F;
                spring.damper = 0F;
                spring.autoConfigureConnectedAnchor = false;
                spring.enableCollision = false;
                spring.connectedAnchor = spring.anchor = Vector3.zero;
                spring.axis = Vector3.back;
                spring.tolerance = 0.01F;
                spring.enablePreprocessing = false;
            }

            var vertices = triMesh.Vertices.Select(v=>new Vector3((float)v.x, (float)v.y, 0)).ToArray();

            var triangles = isForward ?
                triMesh.Triangles.SelectMany(t=> t.vertices.Reverse().Select(v=>v.id)).ToArray() :
                triMesh.Triangles.SelectMany(t=> t.vertices.Select(v=>v.id)).ToArray();//.Reverse()

            var normals = triMesh.Vertices.Select( v => isForward ? Vector3.back : Vector3.forward);

            var bounds = triMesh.bounds;
            var l = bounds.Left;
            var b = bounds.Bottom;
            var w = bounds.Width;
            var h = bounds.Height;

            float dir = isForward ? 1F : -1F;
            var uvs = triMesh.Vertices.Select(v=> new Vector2( dir * (float)((v.x)),(float)( (v.y)))* 0.2F ).ToArray();

            var uniMesh = new Mesh();
            uniMesh.vertices = vertices;
            uniMesh.triangles = triangles;
            uniMesh.uv = uvs;
            uniMesh.normals = normals.ToArray ( );

            uniMesh.boneWeights = fBoneWeights.ToArray ( );
            uniMesh.bindposes = bindposes.ToArray ( );

            skineedRenderer.sharedMesh = uniMesh;
            skineedRenderer.bones = bones.ToArray ( );
            skineedRenderer.rootBone = skinnedRoot;

            return vertexLookup;
        }

    }

}
