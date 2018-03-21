using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Context
{
    using strange.extensions.context.impl;
    using strange.extensions.context.api;

    using Drawing;
    using Polygon;
    using Modeling;

    using Utils.PointerHandler;

    public class RootContext : MVCSContext
    {
        public RootContext ( MonoBehaviour view ) : base ( view ) { }
        public RootContext ( MonoBehaviour view, ContextStartupFlags flags ) : base ( view, flags ) { }


        protected override void mapBindings ( )
        {
            Debug.Log ( "Binding!" );

            #region Context

            commandBinder.Bind ( ContextEvent.START ).
                To<StartCommand> ( ).
                To<Drawing.StartUpCommand>().
                Once ( );

            #endregion Context

            //#region Drawing

            //injectionBinder.Bind<Drawing.PhotoHelper> ( ).To<Drawing.PhotoHelper> ( ).ToName ( "PhotoHelper" ).ToSingleton ( );

            //// injectionBinder.Bind<Drawing.Context> ( ).To ( rootView.drawingContext ).ToName ( "DrawingContext" ).ToSingleton ( );
            //injectionBinder.Bind<Drawing.IPenModel> ( ).To<Drawing.FreeformPenModel> ( ).ToName ( "FreeformPenModel" ).ToSingleton ( );
            //injectionBinder.Bind<Drawing.IPenModel> ( ).To<Drawing.BaseMagneticPenModel> ( ).ToName ( "MagneticPenModel" ).ToSingleton ( );

            //mediationBinder.Bind<CanvasView> ( ).To<CanvasMediator> ( );
            //mediationBinder.Bind<PenButtonView> ( ).To<PenButtonMediator> ( );

            //commandBinder.Bind ( Drawing.Event.DRAW_START ).To<DrawStartCommand> ( );
            //commandBinder.Bind ( Drawing.Event.DRAW_TO ).To<DrawToCommand> ( );
            //commandBinder.Bind ( Drawing.Event.DRAW_END ).To<DrawEndCommand> ( ).To<UpdateMesh3DByNearestCommand> ( );

            //commandBinder.Bind ( Drawing.Event.SNAP_START ).To<SnapStartCommand> ( );
            //commandBinder.Bind ( Drawing.Event.SNAP_TO ).To<SnapToCommand> ( );
            //commandBinder.Bind ( Drawing.Event.SNAP_END ).To<SnapEndCommand> ( );//.To<UpdateMesh3DByNearestCommand> ( );


            //#endregion Drawing

            #region Polygon

            injectionBinder.Bind<IEntityLookup> ( ).To<EntityLookup> ( ).ToSingleton ( );

            mediationBinder.Bind<RectangleButtonView> ( ).To<RectangleButtonMediator> ( );
            mediationBinder.Bind<PointView> ( ).To<PointMediator> ( );
            mediationBinder.Bind<LineView> ( ).To<LineMediator> ( );
            mediationBinder.Bind<PolygonView> ( ).To<PolygonMediator> ( );

            commandBinder.Bind ( Polygon.Event.GENERATE_PRIMITIVE ).To<CreatePrimitiveCommand> ( ).To<UpdateMesh3DByNearestCommand> ( );
            commandBinder.Bind ( Polygon.Event.UPDATA_POINT_ENTITY ).To<UpdatePointEntityCommand> ( ).To<UpdateMesh3DByNearestCommand> ( );
            commandBinder.Bind ( Polygon.Event.REMOVE_POINT_ENTITY ).To<RemovePointEntityCommand> ( ).To<UpdateMesh3DByNearestCommand> ( );
            commandBinder.Bind ( Polygon.Event.INSERT_POINT_ENTITY ).To<InsertPointEntityCommand> ( ).To<UpdateMesh3DByNearestCommand> ( );


            #endregion Polygon

            #region Shaping

            injectionBinder.Bind<ISewSegmentPairLookup> ( ).To<SewSegmentPairLookup> ( ).ToSingleton();

            mediationBinder.Bind<MenuButtonView> ( ).To<MenuButtonMediator> ( );

            //commandBinder.Bind(Stuffing.Event.STUFF ).To<UpdateMesh2DCommand> ( );

            commandBinder.Bind ( Modeling.Event.SIMPLE_SEW ).To<UpdateMesh3DByRigidbodySpringCommand> ( );

            commandBinder.Bind ( Modeling.Event.TO_EDIT ).To<ToEditCommand> ( );

            commandBinder.Bind ( Modeling.Event.COMPUTER_SHADER ).To<UpdateMesh3DByShaderCommand> ( );

            #endregion Shaping
        }
    }

}