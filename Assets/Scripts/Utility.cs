using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
	/// <summary>
	/// 2つのfloatの差が十分小さいかどうかの判定
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <returns></returns>
	public static bool IsEqual(float a, float b)
	{
		//1/256した値以下は誤差として切り捨て
		return MathF.Abs(a - b) <= 0.004f;
	}

	/// <summary>
	/// カラーをPhotoshopなどのパレットで扱える16進数形式に変換する
	/// </summary>
	/// <param name="color"></param>
	/// <returns></returns>
	public static string ColorTo16(Color color)
	{
		return ((int)(color.r * 255.0f)).ToString("x2") + ((int)(color.g * 255.0f)).ToString("x2") + ((int)(color.b * 255.0f)).ToString("x2");
	}

}
