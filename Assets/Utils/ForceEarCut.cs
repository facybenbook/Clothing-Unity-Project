/*******************************************************************************
 * Copyright 2011 See AUTHORS file.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 ******************************************************************************/
 
/** A simple implementation of the ear cutting algorithm to triangulate simple polygons without holes. For more information:
 * <ul>
 * <li><a href="http://cgm.cs.mcgill.ca/~godfried/teaching/cg-projects/97/Ian/algorithm2.html">http://cgm.cs.mcgill.ca/~godfried/
 * teaching/cg-projects/97/Ian/algorithm2.html</a></li>
 * <li><a
 * href="http://www.geometrictools.com/Documentation/TriangulationByEarClipping.pdf">http://www.geometrictools.com/Documentation
 * /TriangulationByEarClipping.pdf</a></li>
 * </ul>
 * If the input polygon is not simple (self-intersects), there will be output but it is of unspecified quality (garbage in,
 * garbage out).
 * @author badlogicgames@gmail.com
 * @author Nicolas Gramlich (optimizations, collinear edge support)
 * @author Eric Spitz
 * @author Thomas ten Cate (bugfixes, optimizations)
 * @author Nathan Sweet (rewrite, return indices, no allocation, optimizations) */

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ForceEarCut {
    private const int CONCAVE = -1;
    private const int TANGENTIAL = 0;
    private const int CONVEX = 1;

	public static List<int> ComputeTriangles (List<Vector2> vertices) {

		return ComputeTriangles(vertices, 0, vertices.Count);
	}
	
	public static List<int> ComputeTriangles (List<Vector2> vertices, int offset, int count) {
        
		List<int> indices  = new List<int> (count);
		for(int i = offset, n = offset + count; i < n; i++){
			indices.Add(i);
		}
		if (!AreVerticesClockwise(vertices, offset, count)){
			indices.Reverse();
		}

		List<int> vertexTypes = new List<int>(count);
        for (int i = 0, n = count; i < n; ++i)
			vertexTypes.Add(ClassifyVertex(vertices, i, indices));
		return Triangulate(vertices, vertexTypes, indices);
    }
	
	private static List<int> Triangulate (List<Vector2> vertices, List<int> vertexTypes, List<int> indices) {

		int vertexCount = indices.Count;
		List<int> triangles  = new List<int> (Math.Max(0, vertexCount - 2) * 3);

		while (vertexCount > 3) {
			int earTipIndex = FindEarTip(vertices, vertexTypes, indices);
			CutEarTip(earTipIndex, indices, vertexTypes, triangles);

			vertexCount--;
			int previousIndex = PreviousIndex(earTipIndex, vertexCount);
            int nextIndex = earTipIndex == vertexCount ? 0 : earTipIndex;
			vertexTypes[previousIndex] = ClassifyVertex(vertices, previousIndex, indices);
			vertexTypes[nextIndex] = ClassifyVertex(vertices, nextIndex, indices);
        }

		if (vertexCount == 3) {
	        triangles.Add(indices[0]);
	        triangles.Add(indices[1]);
	        triangles.Add(indices[2]);
        }

		return triangles;
    }

    /** @return {@link #CONCAVE}, {@link #TANGENTIAL} or {@link #CONVEX} */
	private static int ClassifyVertex (List<Vector2> vertices, int index, List<int> indices) {
		int vertexCount = indices.Count;

		int previous = indices[PreviousIndex(index, vertexCount)];
        int current = indices[index];// * 2;
		int next = indices[NextIndex(index, vertexCount)];
		return ComputeSpannedAreaSign(vertices[previous],  vertices[current], vertices[next]);
    }

	private static int FindEarTip ( List<Vector2> vertices, List<int> vertexTypes, List<int> indices) {

		int vertexCount = indices.Count;
        for (int i = 0; i < vertexCount; i++)
			if (IsEarTip(vertices, i, vertexTypes, indices)) 
				return i;

        // Desperate mode: if no vertex is an ear tip, we are dealing with a degenerate polygon (e.g. nearly collinear).
        // Note that the input was not necessarily degenerate, but we could have made it so by clipping some valid ears.

        // Idea taken from Martin Held, "FIST: Fast industrial-strength triangulation of polygons", Algorithmica (1998),
        // http://citeseerx.ist.psu.edu/viewdoc/summary?doi=10.1.1.115.291

        // Return a convex or tangential vertex if one exists.
        for (int i = 0; i < vertexCount; i++)
            if (vertexTypes[i] != CONCAVE) 
				return i;
        return 0; // If all vertices are concave, just return the first one.
    }

	private static bool IsEarTip ( List<Vector2> vertices, int earTipIndex, List<int> vertexTypes, List<int> indices) {
//        List<int> vertexTypes = this.mVertexTypes;//int[] vertexTypes = this.vertexTypes.items;
//		int vertexCount = vertices.Count;
		int vertexCount = indices.Count;
        if (vertexTypes[earTipIndex] == CONCAVE) return false;

		int previousIndex = PreviousIndex(earTipIndex, vertexCount);
		int nextIndex = NextIndex(earTipIndex, vertexCount);
//        List<int> indices = this.mIndices;//short[] indices = this.indices;
        int p1 = indices[previousIndex];// * 2;
        int p2 = indices[earTipIndex];// * 2;
        int p3 = indices[nextIndex];// * 2;

        // Check if any point is inside the triangle formed by previous, current and next vertices.
        // Only consider vertices that are not part of this triangle, or else we'll always find one inside.
		for (int i = NextIndex(nextIndex, vertexCount); i != previousIndex; i = NextIndex(i, vertexCount)) {
                // Concave vertices can obviously be inside the candidate ear, but so can tangential vertices
                // if they coincide with one of the triangle's vertices.
            if (vertexTypes[i] != CONVEX) {
                int v = indices[i];// * 2;
                // Because the polygon has clockwise winding order, the area sign will be positive if the point is strictly inside.
                // It will be 0 on the edge, which we want to include as well.
				if (ComputeSpannedAreaSign(vertices[p1], vertices[p2], vertices[v]) >= 0 && 
				    ComputeSpannedAreaSign(vertices[p2], vertices[p3], vertices[v]) >= 0 && 
				    ComputeSpannedAreaSign(vertices[p3], vertices[p1], vertices[v]) >= 0) 

					return false;
            }
        }
        return true;
    }

	private static void CutEarTip (int earTipIndex, List<int> indices, List<int> vertexTypes, List<int> triangles) {

		int vertexCount = indices.Count;

		triangles.Add(indices[PreviousIndex(earTipIndex, vertexCount)]);
        triangles.Add(indices[earTipIndex]);
		triangles.Add(indices[NextIndex(earTipIndex, vertexCount)]);

		indices.RemoveAt(earTipIndex);
        vertexTypes.RemoveAt(earTipIndex);
        //vertexCount--;//vertexCount--;
    }

    private static int PreviousIndex (int index, int count) {
        return (index == 0 ? count : index) - 1;
    }

	private static int NextIndex (int index, int count) {
        return (index + 1) % count;
    }

    public static bool AreVerticesClockwise (List<Vector2> vertices, int offset, int count){//(float[] vertices, int offset, int count) {
        if (count <= 2) return false;
        float area = 0, p1x, p1y, p2x, p2y;
		int i, j, n;
		for (i = offset, j = offset + count -1, n = offset + count; i < n; j = i ++) {
            p1x = vertices[j].x;
            p1y = vertices[j].y;
            p2x = vertices[i].x;
            p2y = vertices[i].y;

            area += p1x * p2y - p2x * p1y;
        }

		return area < 0;
    }

    private static int ComputeSpannedAreaSign (float p1x, float p1y, float p2x, float p2y, float p3x, float p3y) {
        float area = p1x * (p3y - p2y);
        area += p2x * (p1y - p3y);
        area += p3x * (p2y - p1y);
        return (int)Math.Sign(area);//(int)Math.signum(area);
    }

	public static int ComputeSpannedAreaSign (Vector2 p1, Vector2 p2, Vector2 p3){
		return ComputeSpannedAreaSign (p1.x, p1.y, p2.x, p2.y, p3.x, p3.y);
	}
}
