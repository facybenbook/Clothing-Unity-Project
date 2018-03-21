using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Polygon
{

    public class PrimitiveCreator
    {

        public static IPolygonEntity Create ( PrimitiveType type )
        {
            switch ( type )
            {
                case PrimitiveType.SQUARE:
                    return CreateSquare ( 2F );
                case PrimitiveType.CIRCLE:
                    return CreateCircle ( 2F );
                case PrimitiveType.RECTANGLE:
                default:
                    return null;
            }
        }

        public static IPolygonEntity CreateSquare ( float size )
        {
            if ( size < 0 ) size = 0.5f;

            //var halfSize = size * 0.5F;

            return new PolygonEntity (
                new IPointEntity [ ] {
                    //new PointEntity ( size, size, 0, size),
                    //new PointEntity ( -size, size , -size, 0),
                    //new PointEntity ( -size, -size , 0, -size),
                    //new PointEntity ( size, -size, size, 0)

                    new PointEntity ( size, size, size, size),
                    new PointEntity ( -size, size , -size, size),
                    new PointEntity ( -size, -size , -size, -size),
                    new PointEntity ( size, -size, size, -size)
                } );

        }

        const float circle = 0.91421356f;//0.948557f;// 

        public static IPolygonEntity CreateCircle ( float size )
        {
            return new PolygonEntity (
                new IPointEntity [ ] {
                    new PointEntity ( 0, size*1.414F , -size, size ),
                    new PointEntity ( -size*1.414F, 0, -size, -size ),
                    new PointEntity ( 0, -size*1.414F, size, -size ),
                    new PointEntity ( size*1.414F, 0, size, size )
                    //new PointEntity ( 0, size , -size * circle , size* circle ),
                    //new PointEntity ( -size, 0, -size* circle, -size * circle ),
                    //new PointEntity ( 0, -size, size * circle, -size* circle),
                    //new PointEntity ( size, 0, size* circle, size * circle )
                } );
        }

    }
}