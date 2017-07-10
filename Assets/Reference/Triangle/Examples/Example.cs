
namespace TriangleNet.Examples
{
    using Geometry;
    using IO;

    public class Exanple
    {
        /// <summary>
        /// Using contours :
        /// A contour may constitute an inner or an outer boundary of a polygon. There are two methods for adding a Contour to a polygon:
        ///     The Add ( Contour contour, bool hole = false ) method will add the contour and make it a hole, if the boolean hole parameter is set to true.
        ///     The Add ( Contour contour, Point hole ) method will add the contour as a hole.The hole parameter must be a point inside the contour.
        /// </summary>
        /// 
        public static void UseContours ( )
        {
            var p = new Polygon();

            // Add the outer box contour with boundary marker 1.
            p.Add ( new Contour ( new Vertex [ 4 ]
            {
        new Vertex(0.0, 0.0, 1),
        new Vertex(3.0, 0.0, 1),
        new Vertex(3.0, 3.0, 1),
        new Vertex(0.0, 3.0, 1)
            }, 1 ) );

            // Add the inner box contour with boundary marker 2.
            p.Add ( new Contour ( new Vertex [ 4 ]
            {
        new Vertex(1.0, 1.0, 2),
        new Vertex(2.0, 1.0, 2),
        new Vertex(2.0, 2.0, 2),
        new Vertex(1.0, 2.0, 2)
            }, 2 )
            , new Point ( 1.5, 1.5 ) ); // Make it a hole.
        }

        /// <summary>
        /// Using segments :
        ///     The Add ( ISegment segment, bool insert = false ) method will add the segment.Additionally, both endpoints will be added, if the boolean insert parameter is set to true.
        ///     The Add ( ISegment segment, int index ) method will add the segment and one of its endpoints,
        /// </summary>
        public static void UseSegments ( )
        {
            var p = new Polygon();

            var v = new Vertex[4]
    {
        new Vertex(0.0, 0.0, 1),
        new Vertex(3.0, 0.0, 1),
        new Vertex(3.0, 3.0, 1),
        new Vertex(0.0, 3.0, 1)
    };

            // Add segments of the outer box.
            p.Add ( new Segment ( v [ 0 ], v [ 1 ], 1 ), 0 );
            p.Add ( new Segment ( v [ 1 ], v [ 2 ], 1 ), 0 );
            p.Add ( new Segment ( v [ 2 ], v [ 3 ], 1 ), 0 );
            p.Add ( new Segment ( v [ 3 ], v [ 0 ], 1 ), 0 );

            v = new Vertex [ 4 ]
            {
        new Vertex(1.0, 1.0, 2),
        new Vertex(2.0, 1.0, 2),
        new Vertex(2.0, 2.0, 2),
        new Vertex(1.0, 2.0, 2)
            };

            // Add segments of the inner box.
            p.Add ( new Segment ( v [ 0 ], v [ 1 ], 2 ), 0 );
            p.Add ( new Segment ( v [ 1 ], v [ 2 ], 2 ), 0 );
            p.Add ( new Segment ( v [ 2 ], v [ 3 ], 2 ), 0 );
            p.Add ( new Segment ( v [ 3 ], v [ 0 ], 2 ), 0 );

            // Add the hole.
            p.Holes.Add ( new Point ( 1.5, 1.5 ) );
        }

        ///
        ///Using a.poly file
        ///

        public static void UsePolyFile ( )
        {
            // Load polygon from file.
            var polygon = FileProcessor.Read("box.poly");
        }
    }
}
