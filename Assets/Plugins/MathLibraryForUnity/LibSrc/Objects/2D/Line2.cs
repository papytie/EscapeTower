﻿using UnityEngine;

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// The line is represented as P+t*D where P is the line origin, D is a
		/// unit-length direction vector, and t is any real number.  The user must
		/// ensure that D is indeed unit length.
		/// </summary>
		public struct Line2
		{
			/// <summary>
			/// Line origin
			/// </summary>
			public Vector2 Center;

			/// <summary>
			/// Line direction. Must be unit length!
			/// </summary>
			public Vector2 Direction;

			
			/// <summary>
			/// Creates the line
			/// </summary>
			/// <param name="center">Line origin</param>
			/// <param name="direction">Line direction. Must be unit length!</param>
			public Line2(ref Vector2 center, ref Vector2 direction)
			{
				Center = center;
				Direction = direction;
			}

			/// <summary>
			/// Creates the line
			/// </summary>
			/// <param name="center">Line origin</param>
			/// <param name="direction">Line direction. Must be unit length!</param>
			public Line2(Vector2 center, Vector2 direction)
			{
				Center = center;
				Direction = direction;
			}

			/// <summary>
			/// Creates the line. Origin is p0, Direction is Normalized(p1-p0).
			/// </summary>
			/// <param name="p0">First point</param>
			/// <param name="p1">Second point</param>
			public static Line2 CreateFromTwoPoints(ref Vector2 p0, ref Vector2 p1)
			{
				Line2 result;
				result.Center = p0;
				result.Direction = (p1 - p0).normalized;
				return result;
			}

			/// <summary>
			/// Creates the line. Origin is p0, Direction is Normalized(p1-p0).
			/// </summary>
			/// <param name="p0">First point</param>
			/// <param name="p1">Second point</param>
			public static Line2 CreateFromTwoPoints(Vector2 p0, Vector2 p1)
			{
				Line2 result;
				result.Center = p0;
				result.Direction = (p1 - p0).normalized;
				return result;
			}

			/// <summary>
			/// Creates the line which is perpendicular to given line and goes through given point.
			/// </summary>
			public static Line2 CreatePerpToLineTrhoughPoint(Line2 line, Vector2 point)
			{
				Line2 result;
				result.Center = point;
				result.Direction = line.Direction.Perp();
				return result;
			}

			/// <summary>
			/// Creates the line which is perpendicular to segment [point0,point1] and line origin goes through middle of the segment.
			/// </summary>
			public static Line2 CreateBetweenAndEquidistantToPoints(Vector2 point0, Vector2 point1)
			{
				Line2 result;
				result.Center.x = (point0.x + point1.x) * .5f;
				result.Center.y = (point0.y + point1.y) * .5f;
				result.Direction.x = point1.y - point0.y;
				result.Direction.y = point0.x - point1.x;
				return result;
			}

			/// <summary>
			/// Creates the line which is parallel to given line on the specified distance from given line.
			/// </summary>
			public static Line2 CreateParallelToGivenLineAtGivenDistance(Line2 line, float distance)
			{
				Line2 result;
				result.Direction = line.Direction;
				result.Center = line.Center + distance * new Vector2(line.Direction.y, -line.Direction.x);
				return result;
			}

			
			/// <summary>
			/// Evaluates line using P+t*D formula, where P is the line origin, D is a
			/// unit-length direction vector, t is parameter.
			/// </summary>
			/// <param name="t">Evaluation parameter</param>
			public Vector2 Eval(float t)
			{
				return Center + Direction * t;
			}

			/// <summary>
			/// Returns signed distance to a point. Where positive distance is on the right of the line,
			/// zero is on the line, negative on the left side of the line.
			/// </summary>
			public float SignedDistanceTo(Vector2 point)
			{
				return (point - Center).DotPerp(Direction);
			}

			/// <summary>
			/// Returns distance to a point, distance is >= 0f.
			/// </summary>
			public float DistanceTo(Vector2 point)
			{
				return Distance.Point2Line2(ref point, ref this);
			}

			/// <summary>
			/// Determines on which side of the line a point is. Returns +1 if a point
			/// is to the right of the line, 0 if it's on the line, -1 if it's on the left.
			/// </summary>
			public int QuerySide(Vector2 point, float epsilon = Mathfex.ZeroTolerance)
			{
				float signedDistance = (point - Center).DotPerp(Direction);
				if (signedDistance < -epsilon)
				{
					return -1;
				}
				else if (signedDistance > epsilon)
				{
					return 1;
				}
				return 0;
			}

			/// <summary>
			/// Returns true if a point is on the negative side of the line, false otherwise.
			/// </summary>
			public bool QuerySideNegative(Vector2 point, float epsilon = Mathfex.ZeroTolerance)
			{
				float signedDistance = (point - Center).DotPerp(Direction);
				return signedDistance <= epsilon;
			}

			/// <summary>
			/// Returns true if a point is on the positive side of the line, false otherwise.
			/// </summary>
			public bool QuerySidePositive(Vector2 point, float epsilon = Mathfex.ZeroTolerance)
			{
				float signedDistance = (point - Center).DotPerp(Direction);
				return signedDistance >= -epsilon;
			}

			/// <summary>
			/// Determines on which side of the line a box is. Returns +1 if a box
			/// is to the right of the line, 0 if it's intersecting the line, -1 if it's on the left.
			/// </summary>
			public int QuerySide(ref Box2 box, float epsilon = Mathfex.ZeroTolerance)
			{
				float tmp0 = box.Extents.x * (box.Axis0.DotPerp(Direction));
				float tmp1 = box.Extents.y * (box.Axis1.DotPerp(Direction));

				float radius = Mathf.Abs(tmp0) + Mathf.Abs(tmp1);
				float signedDistance = (box.Center - Center).DotPerp(Direction);
				return signedDistance < -radius + epsilon ? -1 : (signedDistance > radius - epsilon ? 1 : 0);
			}

			/// <summary>
			/// Returns true if a box is on the negative side of the line, false otherwise.
			/// </summary>
			public bool QuerySideNegative(ref Box2 box, float epsilon = Mathfex.ZeroTolerance)
			{
				float tmp0 = box.Extents.x * (box.Axis0.DotPerp(Direction));
				float tmp1 = box.Extents.y * (box.Axis1.DotPerp(Direction));

				float radius = Mathf.Abs(tmp0) + Mathf.Abs(tmp1);
				float signedDistance = (box.Center - Center).DotPerp(Direction);
				return signedDistance <= -radius + epsilon;
			}

			/// <summary>
			/// Returns true if a box is on the positive side of the line, false otherwise.
			/// </summary>
			public bool QuerySidePositive(ref Box2 box, float epsilon = Mathfex.ZeroTolerance)
			{
				float tmp0 = box.Extents.x * (box.Axis0.DotPerp(Direction));
				float tmp1 = box.Extents.y * (box.Axis1.DotPerp(Direction));

				float radius = Mathf.Abs(tmp0) + Mathf.Abs(tmp1);
				float signedDistance = (box.Center - Center).DotPerp(Direction);
				return signedDistance >= radius - epsilon;
			}

			/// <summary>
			/// Determines on which side of the line a box is. Returns +1 if a box
			/// is to the right of the line, 0 if it's intersecting the line, -1 if it's on the left.
			/// </summary>
			public int QuerySide(ref AAB2 box, float epsilon = Mathfex.ZeroTolerance)
			{
				Vector2 normal;
				normal.x = Direction.y;
				normal.y = -Direction.x;
				Vector2 dMin, dMax;

				if (normal.x >= 0.0f)
				{
					dMin.x = box.Min.x;
					dMax.x = box.Max.x;
				}
				else
				{
					dMin.x = box.Max.x;
					dMax.x = box.Min.x;
				}

				if (normal.y >= 0.0f)
				{
					dMin.y = box.Min.y;
					dMax.y = box.Max.y;
				}
				else
				{
					dMin.y = box.Max.y;
					dMax.y = box.Min.y;
				}

				// Check if minimal point on diagonal is on positive side of plane
				if (normal.Dot(dMin - Center) > -epsilon)
				{
					return 1;
				}
				else if (normal.Dot(dMax - Center) < epsilon)
				{
					return -1;
				}
				return 0;
			}

			/// <summary>
			/// Returns true if a box is on the negative side of the line, false otherwise.
			/// </summary>
			public bool QuerySideNegative(ref AAB2 box, float epsilon = Mathfex.ZeroTolerance)
			{
				Vector2 normal;
				normal.x = Direction.y;
				normal.y = -Direction.x;
				Vector2 dMax;

				if (normal.x >= 0.0f)
				{
					dMax.x = box.Max.x;
				}
				else
				{
					dMax.x = box.Min.x;
				}

				if (normal.y >= 0.0f)
				{
					dMax.y = box.Max.y;
				}
				else
				{
					dMax.y = box.Min.y;
				}

				return normal.Dot(dMax - Center) <= epsilon;
			}

			/// <summary>
			/// Returns true if a box is on the positive side of the line, false otherwise.
			/// </summary>
			public bool QuerySidePositive(ref AAB2 box, float epsilon = Mathfex.ZeroTolerance)
			{
				Vector2 normal;
				normal.x = Direction.y;
				normal.y = -Direction.x;
				Vector2 dMin;

				if (normal.x >= 0.0f)
				{
					dMin.x = box.Min.x;
				}
				else
				{
					dMin.x = box.Max.x;
				}

				if (normal.y >= 0.0f)
				{
					dMin.y = box.Min.y;
				}
				else
				{
					dMin.y = box.Max.y;
				}

				return normal.Dot(dMin - Center) >= -epsilon;
			}

			/// <summary>
			/// Determines on which side of the line a circle is. Returns +1 if a circle
			/// is to the right of the line, 0 if it's intersecting the line, -1 if it's on the left.
			/// </summary>
			public int QuerySide(ref Circle2 circle, float epsilon = Mathfex.ZeroTolerance)
			{
				float signedDistance = (circle.Center - Center).DotPerp(Direction);
				return signedDistance > circle.Radius - epsilon ? 1 : (signedDistance < -circle.Radius + epsilon ? -1 : 0);
			}

			/// <summary>
			/// Returns true if a circle is on the negative side of the line, false otherwise.
			/// </summary>
			public bool QuerySideNegative(ref Circle2 circle, float epsilon = Mathfex.ZeroTolerance)
			{
				float signedDistance = (circle.Center - Center).DotPerp(Direction);
				return signedDistance <= -circle.Radius + epsilon;
			}

			/// <summary>
			/// Returns true if a circle is on the positive side of the line, false otherwise.
			/// </summary>
			public bool QuerySidePositive(ref Circle2 circle, float epsilon = Mathfex.ZeroTolerance)
			{
				float signedDistance = (circle.Center - Center).DotPerp(Direction);
				return signedDistance >= circle.Radius - epsilon;
			}

			/// <summary>
			/// Returns projected point
			/// </summary>
			public Vector2 Project(Vector2 point)
			{
				Vector2 result;
				Distance.SqrPoint2Line2(ref point, ref this, out result);
				return result;
			}

			/// <summary>
			/// Returns angle between this line's direction and another line's direction as: arccos(dot(this.Direction,another.Direction))
			/// If acuteAngleDesired is true, then in resulting angle is > pi/2, then result is transformed to be pi-angle.
			/// </summary>
			public float AngleBetweenTwoLines(Line2 anotherLine, bool acuteAngleDesired = false)
			{
				float angle = Mathf.Acos(this.Direction.Dot(anotherLine.Direction));
				if (acuteAngleDesired &&
					angle > Mathfex.HalfPi)
				{
					return Mathf.PI - angle;
				}
				else
				{
					return angle;
				}
			}

			/// <summary>
			/// Returns string representation.
			/// </summary>
			public override string ToString()
			{
				return string.Format("[Origin: {0} Direction: {1}]", Center.ToStringEx(), Direction.ToStringEx());
			}
		}
	}
}
