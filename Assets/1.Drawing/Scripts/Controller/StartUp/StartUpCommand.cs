using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Drawing
{
    using strange.extensions.mediation.api;
    using strange.extensions.command.impl;
    public class StartUpCommand : EventCommand
    {
        [Inject]
        public IMediationBinder mediationBinder { set; get; }

        public override void Execute()
        {
            #region Drawing

            injectionBinder.Bind<Drawing.PhotoHelper>().To<Drawing.PhotoHelper>().ToName("PhotoHelper").ToSingleton();

            // injectionBinder.Bind<Drawing.Context> ( ).To ( rootView.drawingContext ).ToName ( "DrawingContext" ).ToSingleton ( );
            injectionBinder.Bind<Drawing.IPenModel>().To<Drawing.FreeformPenModel>().ToName("FreeformPenModel").ToSingleton();
            injectionBinder.Bind<Drawing.IPenModel>().To<Drawing.BaseMagneticPenModel>().ToName("MagneticPenModel").ToSingleton();

            mediationBinder.Bind<CanvasView>().To<CanvasMediator>();
            mediationBinder.Bind<PenButtonView>().To<PenButtonMediator>();

            commandBinder.Bind(Drawing.Event.DRAW_START).To<DrawStartCommand>();
            commandBinder.Bind(Drawing.Event.DRAW_TO).To<DrawToCommand>();
            commandBinder.Bind(Drawing.Event.DRAW_END).To<DrawEndCommand>().To<Modeling.UpdateMesh3DByNearestCommand>();

            commandBinder.Bind(Drawing.Event.SNAP_START).To<SnapStartCommand>();
            commandBinder.Bind(Drawing.Event.SNAP_TO).To<SnapToCommand>();
            commandBinder.Bind(Drawing.Event.SNAP_END).To<SnapEndCommand>();//.To<UpdateMesh3DByNearestCommand> ( );


            #endregion Drawing
        }
    }
}
