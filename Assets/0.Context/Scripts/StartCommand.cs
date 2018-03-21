using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Context
{
    using strange.extensions.context.api;
    using strange.extensions.command.impl;
    //using Utils.Extensions;

    public class StartCommand : EventCommand
    {

        [Inject ( ContextKeys.CONTEXT_VIEW )]
        public GameObject contextView { get; set; }

        public override void Execute ( )
        {
            // GameObject go = new GameObject();
            // go.name = "ExampleView";
            //go.transform.parent = contextView.transform;

            //var canvas = GameObject.Find("Canvas");
            //if ( canvas )
            //{
            //    GameObject rectObj = new GameObject();
            //    rectObj.name = "EFFECTIVE AREA";
            //    var rect = rectObj.AddComponent<RectTransform>();
            //    rect.SetParent ( canvas.transform, true );
            //    rect.anchorMin = Vector2.zero;
            //    rect.anchorMax = Vector2.one;
            //    rect.pivot = Vector2.one * 0.5F;
            //    rect.offsetMin = Vector2.one * 16F;
            //    rect.offsetMax = - Vector2.one * 16F;

            //    injectionBinder.Bind<RectTransform> ( ).To ( rect ).ToName ( "EffectiveArea" ).ToSingleton ( );
            //}
            var rootView = contextView.GetComponent<RootView>();
            if ( rootView )
            {
                //var rectArea = effectiveArea.GetComponent<RectTransform>();
                injectionBinder.Bind<RectTransform> ( ).To ( rootView.effectiveArea ).ToName ( "EffectiveArea" ).ToSingleton ( );

                injectionBinder.Bind<AnimationCurve> ( ).To ( rootView.stuffCurve ).ToName ( "StuffCurve" ).ToSingleton ( );
                injectionBinder.Bind<AnimationCurve> ( ).To ( rootView.emptyCurve ).ToName ( "EmptyCurve" ).ToSingleton ( );

               
            }

            var pointAsset = Resources.Load("Prefabs/Point Entity") as GameObject;
            var pointPrefab = pointAsset.GetComponent<Polygon.PointView> ( );
            pointPrefab.CreatePool ( );
            injectionBinder.Bind<Polygon.PointView> ( ).To ( pointPrefab ).ToName ( "PointPrefab" ).ToSingleton ( );


            var lineAsset = Resources.Load("Prefabs/Line Entity") as GameObject;
            var linePrefab = lineAsset.GetComponent<Polygon.LineView> ( );
            linePrefab.CreatePool ( );
            injectionBinder.Bind<Polygon.LineView> ( ).To ( linePrefab ).ToName ( "LinePrefab" ).ToSingleton ( );


        }
    }
}
