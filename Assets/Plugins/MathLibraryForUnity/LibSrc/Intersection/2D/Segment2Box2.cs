﻿using UnityEngine;

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Segment2 and Box2
		/// </summary>
		public struct Segment2Box2Intr
		{
			/// <summary>
			/// Equals to IntersectionTypes.Point or IntersectionTypes.Segment if intersection occured otherwise IntersectionTypes.Empty
			/// </summary>
			public IntersectionTypes IntersectionType;

			/// <summary>
			/// Number of intersection points.
			/// IntersectionTypes.Empty: 0;
			/// IntersectionTypes.Point: 1;
			/// IntersectionTypes.Segment: 2.
			/// </summary>
			public int Quantity;

			/// <summary>
			/// First intersection point
			/// </summary>
			public Vector2 Point0;

			/// <summary>
			/// Second intersection point
			/// </summary>
			public Vector2 Point1;
		}

		public static partial class Intersection
		{
			/// <summary>
			/// Tests whether segment and box intersect.
			/// Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestSegment2Box2(ref Segment2 segment, ref Box2 box)
			{
				Vector2 diff = segment.Center - box.Center;
				float RHS;

				float AWdU0 = Mathf.Abs(segment.Direction.Dot(box.Axis0));
				float ADdU0 = Mathf.Abs(diff.Dot(box.Axis0));
				RHS         = box.Extents.x + segment.Extent * AWdU0;
				if (ADdU0 > RHS)
				{
					return false;
				}

				float AWdU1 = Mathf.Abs(segment.Direction.Dot(box.Axis1));
				float ADdU1 = Mathf.Abs(diff.Dot(box.Axis1));
				RHS         = box.Extents.y + segment.Extent * AWdU1;
				if (ADdU1 > RHS)
				{
					return false;
				}

				Vector2 perp = segment.Direction.Perp();

				float LHS   = Mathf.Abs(perp.Dot(diff));
				float part0 = Mathf.Abs(perp.Dot(box.Axis0));
				float part1 = Mathf.Abs(perp.Dot(box.Axis1));
				RHS         = box.Extents.x * part0 + box.Extents.y * part1;

				return LHS <= RHS;
			}

			/// <summary>
			/// Tests whether segment and box intersect and finds actual intersection parameters.
			/// Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindSegment2Box2(ref Segment2 segment, ref Box2 box, out Segment2Box2Intr info)
			{
				return DoClipping(
					-segment.Extent, segment.Extent,
					ref segment.Center, ref segment.Direction, ref box, true,
					out info.Quantity, out info.Point0, out info.Point1, out info.IntersectionType);
			}
		}
	}
}