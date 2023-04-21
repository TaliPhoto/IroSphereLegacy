using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Rendering;



/// <summary>
/// 色読み取り用のクラス
/// カメラにアタッチして使用します
/// </summary>

namespace IroSphere
{
	public class GetColor : MonoBehaviour
	{

		[SerializeField]
		SphereManager sphereManager;

		private Texture2D capture = null;

		[SerializeField]
		GameObject imageObj;

		RectTransform imageRectTrs;
		Image image;
		public bool isInImageRect { get; private set; } = false;

		Vector2 imageCornerBottomLeft;
		Vector2 imageCornerTopRight;

		public bool IsRandomRead { get; set; }

		private void Awake()
		{
			capture = new Texture2D(1, 1, TextureFormat.RGB24, false);
			imageRectTrs = imageObj.GetComponent<RectTransform>();
			image = imageObj.GetComponent<Image>();
			sphereManager.getColor = this;
		}
		private void Start()
		{
			RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
		}
		void OnDestroy()
		{
			RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
		}


		//void OnPostRender()

		void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
		{
			UpdateCorners();
			RandomRead();

			Vector2 mousePos = Input.mousePosition;

			isInImageRect = (mousePos.x >= imageCornerBottomLeft.x && imageCornerTopRight.x >= mousePos.x &&
							 mousePos.y >= imageCornerBottomLeft.y && imageCornerTopRight.y >= mousePos.y &&
							 image.enabled);

			bool isInScreen = (mousePos.x >= 0.0f && Screen.width >= mousePos.x &&
							 mousePos.y >= 0.0f && Screen.height >= mousePos.y &&
							 image.enabled);

			//マウスポインタの画像上での位置比率
			Vector2 onImagePosRatio;
			if (isInImageRect)
			{
				onImagePosRatio = new Vector2(
				(mousePos.x - imageCornerBottomLeft.x) / (imageCornerTopRight.x - imageCornerBottomLeft.x),
				(mousePos.y - imageCornerBottomLeft.y) / (imageCornerTopRight.y - imageCornerBottomLeft.y));
			}
			else
			{
				onImagePosRatio = Vector2.zero;
			}

			Color color = isInScreen ? ReadPixels(mousePos) : Color.black;
			sphereManager.UpdatePreviewNode(color, isInImageRect,isInScreen);
			sphereManager.UpdateInformation(isInImageRect, mousePos, color, onImagePosRatio);
		}

		private void Update()
		{
			ShowImage();
		}

		/// <summary>
		/// 画像の表示
		/// </summary>
		void ShowImage()
		{
			if (!Input.GetButtonDown("ShowImage"))
				return;

			image.enabled = !image.enabled;
		}


		/// <summary>
		/// 画面を読み取って色を取得
		/// </summary>
		/// <param name="pos"></param>
		public Color ReadPixels(Vector2 pos)
		{
			capture.ReadPixels(new Rect(pos.x, pos.y, 1, 1), 0, 0);
			return capture.GetPixel((int)pos.x, (int)pos.y);
		}

		
		/// <summary>
		/// 起動直後に画像を読み取ってランダムで球を配置
		/// </summary>
		public void RandomRead()
		{
			if (!IsRandomRead)
				return;

			IsRandomRead = false;

			if (!image.enabled)
				return;

			while (true)
			{
				Vector2 pos = new Vector2(
					Random.Range(imageCornerBottomLeft.x, imageCornerTopRight.x),
					Random.Range(imageCornerBottomLeft.y, imageCornerTopRight.y));
				Color color = ReadPixels(pos);
				sphereManager.UpdatePreviewNode(color, true,false);

				if (!sphereManager.CreateAdditiveNode())
					return;
			}
		}

		public void UpdateCorners()
		{
			var corners = new Vector3[4];

			//画像の範囲外のピクセルを取得してしまうため、一旦ローカル系で座標をだし、１ピクセル中に入れて、その後にワールド系に変換しています

			imageRectTrs.GetLocalCorners(corners);

			corners[0] += new Vector3(1.0f, 1.0f, 0.0f);
			corners[1] += new Vector3(1.0f, -1.0f, 0.0f);
			corners[2] += new Vector3(-1.0f, -1.0f, 0.0f);
			corners[3] += new Vector3(-1.0f, 1.0f, 0.0f);

			for (int i = 0; i < corners.Length; i++)
			{
				corners[i] = imageRectTrs.TransformPoint(corners[i]);
			}


			imageCornerBottomLeft = RectTransformUtility.WorldToScreenPoint(Camera.main, corners[0]);
			imageCornerTopRight = RectTransformUtility.WorldToScreenPoint(Camera.main, corners[2]);

			//ウィンドウサイズによっては画面の外をサンプリングしてしまう事がある為、切り取る
			//Y軸は、画像の拡縮がかかる為、処理しなくても大丈夫
			if(imageCornerBottomLeft.x < 1)
				imageCornerBottomLeft.x = 1;

		}
	}
}