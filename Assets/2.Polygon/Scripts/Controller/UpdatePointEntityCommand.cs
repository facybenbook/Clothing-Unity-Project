using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Clothing.Polygon
{
    using System.Linq;

    using strange.extensions.command.impl;

    using strange.extensions.context.api;
    using strange.extensions.dispatcher.eventdispatcher.api;

    public class UpdatePointEntityCommand : EventCommand
    {

        [Inject]
        public IEntityLookup lookup { set; get; }

        public override void Execute ( )
        {

            var view = evt.data as PointView;

            var poly = view.poly;
            var index = poly.points.IndexOf(view);

            var polyEntity = lookup.libs[poly];
            
            var pointEntity = new PointEntity(view.transform.localPosition, view.outLine.curvePosition);
            polyEntity.Points [ index ] = pointEntity;
        }
    }
}
