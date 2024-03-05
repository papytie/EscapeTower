﻿using UnityEngine;

namespace Dest
{
	namespace Math
	{
		public static partial class Distance
		{
			/// <summary>
			/// Returns distance between two segments
			/// </summary>
			public static float Segment3Segment3(ref Segment3 segment0, ref Segment3 segment1)
			{
				Vector3 closestPoint0, closestPoint1;
				return Mathf.Sqrt(SqrSegment3Segment3(ref segment0, ref segment1, out closestPoint0, out closestPoint1));
			}

			/// <summary>
			/// Returns distance between two segments
			/// </summary>
			/// <param name="closestPoint0">Point on segment0 closest to segment1</param>
			/// <param name="closestPoint1">Point on segment1 closest to segment0</param>
			public static float Segment3Segment3(ref Segment3 segment0, ref Segment3 segment1, out Vector3 closestPoint0, out Vector3 closestPoint1)
			{
				return Mathf.Sqrt(SqrSegment3Segment3(ref segment0, ref segment1, out closestPoint0, out closestPoint1));
			}


			/// <summary>
			/// Returns squared distance between two segments
			/// </summary>
			public static float SqrSegment3Segment3(ref Segment3 segment0, ref Segment3 segment1)
			{
				Vector3 closestPoint0, closestPoint1;
				return SqrSegment3Segment3(ref segment0, ref segment1, out closestPoint0, out closestPoint1);
			}

			/// <summary>
			/// Returns squared distance between two segments
			/// </summary>
			/// <param name="closestPoint0">Point on segment0 closest to segment1</param>
			/// <param name="closestPoint1">Point on segment1 closest to segment0</param>
			public static float SqrSegment3Segment3(ref Segment3 segment0, ref Segment3 segment1, out Vector3 closestPoint0, out Vector3 closestPoint1)
			{
				Vector3 diff = segment0.Center - segment1.Center;
				float a01 = -segment0.Direction.Dot(segment1.Direction);
				float b0 = diff.Dot(segment0.Direction);
				float b1 = -diff.Dot(segment1.Direction);
				float c = diff.sqrMagnitude;
				float det = Mathf.Abs((float)1 - a01 * a01);
				float s0, s1, sqrDist, extDet0, extDet1, tmpS0, tmpS1;

				if (det >= Mathfex.ZeroTolerance)
				{
					// Segments are not parallel.
					s0 = a01 * b1 - b0;
					s1 = a01 * b0 - b1;
					extDet0 = segment0.Extent * det;
					extDet1 = segment1.Extent * det;

					if (s0 >= -extDet0)
					{
						if (s0 <= extDet0)
						{
							if (s1 >= -extDet1)
							{
								if (s1 <= extDet1)  // region 0 (interior)
								{
									// Minimum at interior points of segments.
									float invDet = ((float)1) / det;
									s0 *= invDet;
									s1 *= invDet;
									sqrDist = s0 * (s0 + a01 * s1 + ((float)2) * b0) +
										s1 * (a01 * s0 + s1 + ((float)2) * b1) + c;
								}
								else  // region 3 (side)
								{
									s1 = segment1.Extent;
									tmpS0 = -(a01 * s1 + b0);
									if (tmpS0 < -segment0.Extent)
									{
										s0 = -segment0.Extent;
										sqrDist = s0 * (s0 - ((float)2) * tmpS0) +
											s1 * (s1 + ((float)2) * b1) + c;
									}
									else if (tmpS0 <= segment0.Extent)
									{
										s0 = tmpS0;
										sqrDist = -s0 * s0 + s1 * (s1 + ((float)2) * b1) + c;
									}
									else
									{
										s0 = segment0.Extent;
										sqrDist = s0 * (s0 - ((float)2) * tmpS0) +
											s1 * (s1 + ((float)2) * b1) + c;
									}
								}
							}
							else  // region 7 (side)
							{
								s1 = -segment1.Extent;
								tmpS0 = -(a01 * s1 + b0);
								if (tmpS0 < -segment0.Extent)
								{
									s0 = -segment0.Extent;
									sqrDist = s0 * (s0 - ((float)2) * tmpS0) +
										s1 * (s1 + ((float)2) * b1) + c;
								}
								else if (tmpS0 <= segment0.Extent)
								{
									s0 = tmpS0;
									sqrDist = -s0 * s0 + s1 * (s1 + ((float)2) * b1) + c;
								}
								else
								{
									s0 = segment0.Extent;
									sqrDist = s0 * (s0 - ((float)2) * tmpS0) +
										s1 * (s1 + ((float)2) * b1) + c;
								}
							}
						}
						else
						{
							if (s1 >= -extDet1)
							{
								if (s1 <= extDet1)  // region 1 (side)
								{
									s0 = segment0.Extent;
									tmpS1 = -(a01 * s0 + b1);
									if (tmpS1 < -segment1.Extent)
									{
										s1 = -segment1.Extent;
										sqrDist = s1 * (s1 - ((float)2) * tmpS1) +
											s0 * (s0 + ((float)2) * b0) + c;
									}
									else if (tmpS1 <= segment1.Extent)
									{
										s1 = tmpS1;
										sqrDist = -s1 * s1 + s0 * (s0 + ((float)2) * b0) + c;
									}
									else
									{
										s1 = segment1.Extent;
										sqrDist = s1 * (s1 - ((float)2) * tmpS1) +
											s0 * (s0 + ((float)2) * b0) + c;
									}
								}
								else  // region 2 (corner)
								{
									s1 = segment1.Extent;
									tmpS0 = -(a01 * s1 + b0);
									if (tmpS0 < -segment0.Extent)
									{
										s0 = -segment0.Extent;
										sqrDist = s0 * (s0 - ((float)2) * tmpS0) +
											s1 * (s1 + ((float)2) * b1) + c;
									}
									else if (tmpS0 <= segment0.Extent)
									{
										s0 = tmpS0;
										sqrDist = -s0 * s0 + s1 * (s1 + ((float)2) * b1) + c;
									}
									else
									{
										s0 = segment0.Extent;
										tmpS1 = -(a01 * s0 + b1);
										if (tmpS1 < -segment1.Extent)
										{
											s1 = -segment1.Extent;
											sqrDist = s1 * (s1 - ((float)2) * tmpS1) +
												s0 * (s0 + ((float)2) * b0) + c;
										}
										else if (tmpS1 <= segment1.Extent)
										{
											s1 = tmpS1;
											sqrDist = -s1 * s1 + s0 * (s0 + ((float)2) * b0) + c;
										}
										else
										{
											s1 = segment1.Extent;
											sqrDist = s1 * (s1 - ((float)2) * tmpS1) +
												s0 * (s0 + ((float)2) * b0) + c;
										}
									}
								}
							}
							else  // region 8 (corner)
							{
								s1 = -segment1.Extent;
								tmpS0 = -(a01 * s1 + b0);
								if (tmpS0 < -segment0.Extent)
								{
									s0 = -segment0.Extent;
									sqrDist = s0 * (s0 - ((float)2) * tmpS0) +
										s1 * (s1 + ((float)2) * b1) + c;
								}
								else if (tmpS0 <= segment0.Extent)
								{
									s0 = tmpS0;
									sqrDist = -s0 * s0 + s1 * (s1 + ((float)2) * b1) + c;
								}
								else
								{
									s0 = segment0.Extent;
									tmpS1 = -(a01 * s0 + b1);
									if (tmpS1 > segment1.Extent)
									{
										s1 = segment1.Extent;
										sqrDist = s1 * (s1 - ((float)2) * tmpS1) +
											s0 * (s0 + ((float)2) * b0) + c;
									}
									else if (tmpS1 >= -segment1.Extent)
									{
										s1 = tmpS1;
										sqrDist = -s1 * s1 + s0 * (s0 + ((float)2) * b0) + c;
									}
									else
									{
										s1 = -segment1.Extent;
										sqrDist = s1 * (s1 - ((float)2) * tmpS1) +
											s0 * (s0 + ((float)2) * b0) + c;
									}
								}
							}
						}
					}
					else
					{
						if (s1 >= -extDet1)
						{
							if (s1 <= extDet1)  // region 5 (side)
							{
								s0 = -segment0.Extent;
								tmpS1 = -(a01 * s0 + b1);
								if (tmpS1 < -segment1.Extent)
								{
									s1 = -segment1.Extent;
									sqrDist = s1 * (s1 - ((float)2) * tmpS1) +
										s0 * (s0 + ((float)2) * b0) + c;
								}
								else if (tmpS1 <= segment1.Extent)
								{
									s1 = tmpS1;
									sqrDist = -s1 * s1 + s0 * (s0 + ((float)2) * b0) + c;
								}
								else
								{
									s1 = segment1.Extent;
									sqrDist = s1 * (s1 - ((float)2) * tmpS1) +
										s0 * (s0 + ((float)2) * b0) + c;
								}
							}
							else  // region 4 (corner)
							{
								s1 = segment1.Extent;
								tmpS0 = -(a01 * s1 + b0);
								if (tmpS0 > segment0.Extent)
								{
									s0 = segment0.Extent;
									sqrDist = s0 * (s0 - ((float)2) * tmpS0) +
										s1 * (s1 + ((float)2) * b1) + c;
								}
								else if (tmpS0 >= -segment0.Extent)
								{
									s0 = tmpS0;
									sqrDist = -s0 * s0 + s1 * (s1 + ((float)2) * b1) + c;
								}
								else
								{
									s0 = -segment0.Extent;
									tmpS1 = -(a01 * s0 + b1);
									if (tmpS1 < -segment1.Extent)
									{
										s1 = -segment1.Extent;
										sqrDist = s1 * (s1 - ((float)2) * tmpS1) +
											s0 * (s0 + ((float)2) * b0) + c;
									}
									else if (tmpS1 <= segment1.Extent)
									{
										s1 = tmpS1;
										sqrDist = -s1 * s1 + s0 * (s0 + ((float)2) * b0) + c;
									}
									else
									{
										s1 = segment1.Extent;
										sqrDist = s1 * (s1 - ((float)2) * tmpS1) +
											s0 * (s0 + ((float)2) * b0) + c;
									}
								}
							}
						}
						else   // region 6 (corner)
						{
							s1 = -segment1.Extent;
							tmpS0 = -(a01 * s1 + b0);
							if (tmpS0 > segment0.Extent)
							{
								s0 = segment0.Extent;
								sqrDist = s0 * (s0 - ((float)2) * tmpS0) +
									s1 * (s1 + ((float)2) * b1) + c;
							}
							else if (tmpS0 >= -segment0.Extent)
							{
								s0 = tmpS0;
								sqrDist = -s0 * s0 + s1 * (s1 + ((float)2) * b1) + c;
							}
							else
							{
								s0 = -segment0.Extent;
								tmpS1 = -(a01 * s0 + b1);
								if (tmpS1 < -segment1.Extent)
								{
									s1 = -segment1.Extent;
									sqrDist = s1 * (s1 - ((float)2) * tmpS1) +
										s0 * (s0 + ((float)2) * b0) + c;
								}
								else if (tmpS1 <= segment1.Extent)
								{
									s1 = tmpS1;
									sqrDist = -s1 * s1 + s0 * (s0 + ((float)2) * b0) + c;
								}
								else
								{
									s1 = segment1.Extent;
									sqrDist = s1 * (s1 - ((float)2) * tmpS1) +
										s0 * (s0 + ((float)2) * b0) + c;
								}
							}
						}
					}
				}
				else
				{
					// The segments are parallel.  The average b0 term is designed to
					// ensure symmetry of the function.  That is, dist(seg0,seg1) and
					// dist(seg1,seg0) should produce the same number.
					float e0pe1 = segment0.Extent + segment1.Extent;
					float sign = (a01 > (float)0 ? (float)-1 : (float)1);
					float b0Avr = ((float)0.5) * (b0 - sign * b1);
					float lambda = -b0Avr;
					if (lambda < -e0pe1)
					{
						lambda = -e0pe1;
					}
					else if (lambda > e0pe1)
					{
						lambda = e0pe1;
					}

					s1 = -sign * lambda * segment1.Extent / e0pe1;
					s0 = lambda + sign * s1;
					sqrDist = lambda * (lambda + ((float)2) * b0Avr) + c;
				}

				closestPoint0 = segment0.Center + s0 * segment0.Direction;
				closestPoint1 = segment1.Center + s1 * segment1.Direction;

				// Account for numerical round-off errors.
				if (sqrDist < (float)0)
				{
					sqrDist = (float)0;
				}
				return sqrDist;
			}
		}
	}
}