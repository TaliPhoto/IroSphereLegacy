using System;
using UnityEngine;

/// <summary>
/// HSL色空間用のクラス
/// </summary>
/// 
namespace IroSphere
{
	public class HSL
	{
		//HSLのパラメーター。全て0～1の値になります。
		public float h { get; private set; } = 0.0f;
		public float s { get; private set; } = 0.0f;
		public float l { get; private set; } = 0.0f;

		public HSL()
		{
			h = 0.0f;
			s = 0.0f;
			l = 0.0f;
		}
		public HSL(float h, float s, float l)
		{
			this.h = h;
			this.s = s;
			this.l = l;
		}

		/// <summary>
		/// 空間座標からHSLを作成
		/// 座標は半径1の球の中になくてはならない
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public static HSL PositionToHSL(Vector3 position)
		{
			if (position.sqrMagnitude >= 1.0f)
				position = position.normalized;

			HSL hsl = new HSL();
			hsl.l = position.y * 0.5f + 0.5f;
			hsl.h = Mathf.Atan2(position.x, position.z) / Mathf.PI * 0.5f + 0.5f;

			//Nan回避
			if (hsl.l >= 1.0f)
				hsl.s = 0.0f;
			else
				hsl.s = new Vector2(position.x, position.z).magnitude / Mathf.Sqrt(1 - position.y * position.y);

			return hsl;
		}

		/// <summary>
		/// HSLから位置座標を割り出す
		/// </summary>
		/// <returns></returns>
		public Vector3 ToPosition()
		{
			Vector3 position = Vector3.zero;

			float angle = (h - 0.5f) * 2.0f * -180.0f + 90.0f;
			angle *= Mathf.Deg2Rad;
			position.x = Mathf.Cos(angle);
			position.z = Mathf.Sin(angle);


			float tmpY = (l - 0.5f) * 2.0f;

			position *= Mathf.Sqrt(1.0f - tmpY * tmpY) * position.magnitude * s;
			position.y = tmpY;

			return position;
		}

		/// <summary>
		/// HSLからRGBに変換
		/// </summary>
		/// <returns></returns>
		public Color ToRgb()
		{
			Color color = Color.white;
			float min, max;
			if (l < 0.5f)
			{
				max = l + l * s;
				min = l - l * s;
			}
			else
			{
				max = l + (1.0f - l) * s;
				min = l - (1.0f - l) * s;
			}

			if (h < 0.1666f)
			{
				color.r = max;
				color.g = A(0.0f);
				color.b = min;
			}
			else if (h < 0.3333f)
			{
				color.r = B(0.3333f);
				color.g = max;
				color.b = min;
			}
			else if (h < 0.5f)
			{
				color.r = min;
				color.g = max;
				color.b = A(0.3333f);
			}
			else if (h < 0.6666f)
			{
				color.r = min;
				color.g = B(0.6666f);
				color.b = max;
			}
			else if (h < 0.8333f)
			{
				color.r = A(0.6666f);
				color.g = min;
				color.b = max;
			}
			else
			{
				color.r = max;
				color.g = min;
				color.b = B(1.0f);

			}
			return color;

			float A(float a)
			{
				return ((h - a) / 0.1666f) * (max - min) + min;
			}

			float B(float a)
			{
				return ((a - h) / 0.1666f) * (max - min) + min;
			}
			
		}

		/// <summary>
		/// RGBからHSLに変換
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>

		public static HSL RGBToHSL(Color color)
		{
			HSL hsl = new HSL();
			float max = MathF.Max(color.r, MathF.Max(color.g, color.b));
			float min = MathF.Min(color.r, MathF.Min(color.g, color.b));

			hsl.l = (max + min) * 0.5f;

			if (Utility.IsEqual(max, min))
			{
				hsl.h = 0.0f;
				hsl.s = 0.0f;
			}
			else
			{
				if (Utility.IsEqual(max, color.r))
				{
					hsl.h = 0.1666f * ((color.g - color.b) / (max - min));
				}
				else if (Utility.IsEqual(max, color.g))
				{
					hsl.h = 0.1666f * ((color.b - color.r) / (max - min)) + 0.3333f;
				}
				else
				{
					hsl.h = 0.1666f * ((color.r - color.g) / (max - min)) + 0.6666f;
				}

				if (hsl.h < 0.0f)
				{
					hsl.h += 1.0f;
				}

				if (hsl.l < 0.5f)
				{
					hsl.s = (max - min) / (max + min);
				}
				else
				{
					hsl.s = (max - min) / (2.0f - max - min);
				}


			}

			return hsl;

		}
	}
}