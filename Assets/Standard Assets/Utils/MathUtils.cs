using UnityEngine;
using System.Collections;

public static class MathUtilsExtension {
	//------------------------------
	//------------ float
	//------------------------------

	public static bool IsEqual(this float first, float second, float accuracy = float.Epsilon) {
		return Mathf.Abs(first - second) < accuracy;
	}

	public static bool IsNotEqual(this float first, float second, float accuracy = float.Epsilon) {
		return !IsEqual(first, second, accuracy);
	}

	public static float Sqr(this float number) {
		return number * number;
	}

	public static float Sqrt(this float number) {
		return Mathf.Sqrt(number);
	}

	public static float Abs(this float number) {
		return Mathf.Abs(number);
	}

	/// Determine the number is between left and right segment
	public static bool IsBetween(this float number, float left, float right) {
		// swap to make sure left < right
		if(left > right) {
			var temp = left;
			left = right;
			right = temp;
		}
		return number >= left && number <= right;
	}
	//------------------------------
	//------------ vector2
	//------------------------------

	public static bool IsEqual(this Vector2 first, Vector2 second, float accuracy = float.Epsilon) {
		return first.x.IsEqual(second.x, accuracy)
		&& first.y.IsEqual(second.y, accuracy);
	}

	public static bool IsNotEqual(this Vector2 first, Vector2 second, float accuracy = float.Epsilon) {
		return !first.x.IsEqual(second.x, accuracy)
		|| !first.y.IsEqual(second.y, accuracy);
	}

	public static bool IsLengthEqual(this Vector2 first, Vector2 second, float accuracy = float.Epsilon) {
		return first.sqrMagnitude.IsEqual(second.sqrMagnitude, accuracy);
	}

	public static bool IsLonger(this Vector2 vector, float length) {
		return vector.sqrMagnitude > length * length;
	}

	public static bool IsAnyCoordinateBiggerThan(this Vector2 vector, float length) {
		return vector.x > length || vector.y > length;
	}

	public static Vector3 ToVector3(this Vector2 vector, float z = 0f) {
		return new Vector3(vector.x, vector.y, z);
	}

	/// Modify the position by add a (-randomX, randomX) to Xcoor and (-randomY, randomY) to Ycoor
	public static Vector2 RandomTranslate(this Vector2 vector, float randomX, float randomY) {
		var result = new Vector2(vector.x, vector.y);
		result.x += Random.Range(-randomX, randomX);
		result.y += Random.Range(-randomY, randomY);
		return result;
	}
	//------------------------------
	//------------ vector3
	//------------------------------

	public static bool IsEqual(this Vector3 first, Vector3 second, float accuracy = float.Epsilon) {
		return first.x.IsEqual(second.x, accuracy)
		&& first.y.IsEqual(second.y, accuracy)
		&& first.z.IsEqual(second.z, accuracy);
	}

	public static bool IsNotEqual(this Vector3 first, Vector3 second, float accuracy = float.Epsilon) {
		return !first.x.IsEqual(second.x, accuracy)
		|| !first.y.IsEqual(second.y, accuracy)
		|| !first.z.IsEqual(second.z, accuracy);
	}

	public static bool IsLengthEqual(this Vector3 first, Vector3 second, float accuracy = float.Epsilon) {
		return first.sqrMagnitude.IsEqual(second.sqrMagnitude, accuracy);
	}

	public static bool IsLonger(this Vector3 vector, float length) {
		return vector.sqrMagnitude > length * length;
	}

	public static bool IsAnyCoordinateBiggerThan(this Vector3 vector, float length) {
		return vector.x > length || vector.y > length || vector.z > length;
	}

	public static Vector2 XY(this Vector3 vector) {
		return new Vector2(vector.x, vector.y);
	}

	public static float SqrLength(this Vector3[] spline) {
		var result = 0f;
		for(int n = 0; n < spline.Length - 1; n++) {
			result += (spline[n] - spline[n + 1]).sqrMagnitude;
		}
		return result;
	}

	public static float Magnitude(this Vector3[] spline) {
		var result = 0f;
		for(int n = 0; n < spline.Length - 1; n++) {
			result += (spline[n] - spline[n + 1]).magnitude;
		}
		return result;
	}

	public static bool IsLonger(this Vector3[] first, Vector3[] second) {
		// if the two array have the same point count, then we can use SqrLength for better performance
		if(first.Length == second.Length) return first.SqrLength() > second.SqrLength();
		// they not same point count, then we have to use magnitude, a bit longer than SqrMagnitude
		return first.Magnitude() > second.Magnitude();
	}

	/// Just revert the order of the input array, NOT create new one !!! If you want new array ? then use CloneAndRevert() instead.
	/// In case you use this syntax : array1st = array2nd.Revert, then you still have only one array, but 2 reference to it (array1st, array2nd)
	public static Vector3[] Revert(this Vector3[] array) {
		Vector3 temp;
		for(int n = 0, halfLength = array.Length / 2; n < halfLength; n++) {
			var swapIndex = array.Length - 1 - n;
			temp = array[n];
			array[n] = array[swapIndex];
			array[swapIndex] = temp;
		}
		return array;
	}

	public static Vector3[] Clone(this Vector3[] array) {
		var result = new Vector3[array.Length];
		for(int n = 0; n < array.Length; n++) {
			result[n] = array[n];
		}
		return result;
	}

	public static Vector3[] CloneAndRevert(this Vector3[] array) {
		var result = new Vector3[array.Length];
		for(int n = 0; n < array.Length; n++) {
			result[n] = array[array.Length - 1 - n];
		}
		return result;
	}
	//------------------------------
	//------------ Quaternion
	//------------------------------

	public static bool IsAnyCoordinateBiggerThan(this Quaternion vector, float length) {
		return vector.x > length || vector.y > length || vector.z > length;
	}

	public static bool IsNotEqual(this Quaternion first, Quaternion second, float accuracy = float.Epsilon) {
		return !first.x.IsEqual(second.x, accuracy)
		|| !first.y.IsEqual(second.y, accuracy)
		|| !first.z.IsEqual(second.z, accuracy);
	}

	public static bool IsEqual(this Quaternion first, Quaternion second, float accuracy = float.Epsilon) {
		return first.x.IsEqual(second.x, accuracy)
		&& first.y.IsEqual(second.y, accuracy)
		&& first.z.IsEqual(second.z, accuracy);
	}

	public static Vector3 XYZ(this Quaternion quaternion) {
		var result = new Vector3();
		result.x = quaternion.x;
		result.y = quaternion.y;
		result.z = quaternion.z;
		return result;
	}
}