using IroSphere;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static IroSphere.SphereManager;

namespace IroSphere
{

	[CreateAssetMenu(menuName = "IroSphere/Parameter", fileName = "Parameter")]
	public class Parameter : ScriptableObject
	{


		[Header("スフィアの回転、移動、拡縮性能")]
		[SerializeField, Tooltip("角加速度")]
		float rotateSpeed = 200.0f;
		public float RotateSpeed => rotateSpeed;
		
		[SerializeField, Tooltip("角速度ブレーキング性能")]
		float rotateBrake = 3.0f;
		public float RotateBrake => rotateBrake;
		
		[SerializeField, Tooltip("移動速度")]
		float moveSpeed = 2.0f;
		public float MoveSpeed => moveSpeed;

		[SerializeField, Tooltip("拡縮速度")]
		float scaleSpeed = 2.0f;
		public float ScaleSpeed => scaleSpeed;

		[Header("ノード形状タイプ")]
		[SerializeField, Tooltip("形状タイプ")]
		ShapeType shapeType = ShapeType.SPHERE;
		public ShapeType ShapeType => shapeType;

		[Header("初期配置ノードの個数（▲ゲーム実行中変更不可▲）")]

		[SerializeField, DisableEditOnPlay, Range(0, 32), Tooltip("色相方向のノードの数。動作が重たいと感じたらここを下げて下さい")]
		int initNodeNumH = 21;
		public int InitNodeNumH => initNodeNumH;


		[SerializeField, DisableEditOnPlay, Range(0, 10), Tooltip("彩度方向のノードの数。動作が重たいと感じたらここを下げて下さい")]
		int initNodeNumS = 7;
		public int InitNodeNumS => initNodeNumS;

		[SerializeField, DisableEditOnPlay, Range(0, 19), Tooltip("明度方向のノードの数。動作が重たいと感じたらここを下げて下さい")]
		int initNodeNumL = 15;
		public int InitNodeNumL => initNodeNumL;

		[Header("初期配置のノードのサイズ")]

		[SerializeField, Range(0.0f, 1.0f), Tooltip("初期配置ノードのサイズ")]
		float initNodeSize = 0.02f;
		public float InitNodeSize => initNodeSize;

		[SerializeField, Range(0.0f, 1.0f), Tooltip("スフィアの中心方向に行くに従って小さくするかどうか")]
		float initNodeCenterSmall = 1;
		public float InitNodeCenterSmall => initNodeCenterSmall;

		[Header("プレビュー用ノードのサイズ")]

		[SerializeField, Range(0.0f, 1.0f), Tooltip("プレビューノードのサイズ")]
		float previewNodeSize = 0.4f;
		public float PreviewNodeSize => previewNodeSize;


		[Header("クリックで追加するノードのサイズ")]

		[SerializeField, Range(0.0f, 1.0f), Tooltip("クリックして置ける球のサイズ")]
		float additiveNodeSize = 0.2f;
		public float AdditiveNodeSize => additiveNodeSize;

		[Header("クリックで追加するノードの最大数（▲ゲーム実行中変更不可▲）")]
		[SerializeField, DisableEditOnPlay, Range(1, 1000), Tooltip("クリックして置ける球の最大数")]
		int maxAdditiveNodeNum = 200;
		public int MaxAdditiveNodeNum => maxAdditiveNodeNum;



		public SphereManager manager { get; set; }

		private void OnValidate()
		{
			if (!EditorApplication.isPlaying || manager == null)
				return;

			manager.ChangeNodeSize();
		}

	}
}