using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static IroSphere.SphereManager;
using System.IO;

namespace IroSphere
{

	public class Sphere
	{
		public GameObject Root { get; private set; }
		public GameObject initNodesParent { get; private set; }
		public GameObject additiveNodesParent { get; private set; }

		SphereManager manager;

		public Vector3 moveTargetPos { get; set; }
		Vector3 velocity;

		float SeparateMoveSpeed = 0.2f;

		public List<GameObject> InitNodes { get; private set; } = new List<GameObject>();
		public List<GameObject> AdditiveNodes { get; private set; } = new List<GameObject>();

		GameObject PreviewNode;
		Material previewMaterial;
		GameObject grid;

		public Sphere(Transform parent, int i, SphereManager sphereManager)
		{
			i++;
			this.manager = sphereManager;
			this.Root = new GameObject("Sphere" + i);
			this.Root.transform.parent = parent;
			this.Root.transform.localPosition = Vector3.zero;
			this.Root.transform.localRotation = Quaternion.identity; //Quaternion.AngleAxis(30.0f, Vector3.right);
			this.Root.transform.localScale = Vector3.one;

			initNodesParent = new GameObject("InitNodes" + i);
			initNodesParent.transform.parent = this.Root.transform;
			initNodesParent.transform.localPosition = Vector3.zero;
			initNodesParent.transform.localRotation = Quaternion.identity;
			initNodesParent.transform.localScale = Vector3.one;

			additiveNodesParent = new GameObject("AdditiveNodes" + i);
			additiveNodesParent.transform.parent = this.Root.transform;
			additiveNodesParent.transform.localPosition = Vector3.zero;
			additiveNodesParent.transform.localRotation = Quaternion.identity;
			additiveNodesParent.transform.localScale = Vector3.one;
			this.manager = sphereManager;

			grid = GameObject.Instantiate(sphereManager.Grid, Root.transform);
			grid.SetActive(true);
		}


		/// <summary>
		/// 回転
		/// </summary>
		/// <param name="rotation"></param>
		public void SetRotation(Quaternion rotation)
		{
			Root.transform.localRotation = rotation;
		}


		/// <summary>
		/// 移動
		/// </summary>
		public void UpdateMove()
		{
			Root.transform.localPosition = Vector3.SmoothDamp(Root.transform.localPosition, moveTargetPos, ref velocity, SeparateMoveSpeed, float.MaxValue, Time.deltaTime);
		}



		/// <summary>
		/// ノードの作成
		/// </summary>
		/// <param name="position"></param>
		/// <param name="size"></param>
		/// <param name="nodeType"></param>
		/// <returns></returns>
		public GameObject CreateNode(Vector3 position, float size, NodeType nodeType, Material material)
		{
			var primitiveType = manager.Param.ShapeType == ShapeType.SPHERE ? PrimitiveType.Sphere : PrimitiveType.Cube;
			GameObject nodeObj = GameObject.CreatePrimitive(primitiveType);

			GameObject.Destroy(nodeObj.GetComponent<Collider>());


			nodeObj.GetComponent<MeshRenderer>().material = material;
			HSL hsl = HSL.PositionToHSL(position);
			Color color = hsl.ToRgb();

			var m = nodeObj.GetComponent<MeshRenderer>().material;
			m.color = color;

			switch (nodeType)
			{
				case NodeType.INIT:
					nodeObj.name = "Init" + nodeObj.name;
					nodeObj.transform.parent = initNodesParent.transform;
					InitNodes.Add(nodeObj);
					break;
				case NodeType.ADDITIVE:
					nodeObj.name = "Additive" + nodeObj.name;
					nodeObj.transform.parent = additiveNodesParent.transform;
					AdditiveNodes.Add(nodeObj);
					break;
				case NodeType.PREVIEW:
					nodeObj.name = "Preview" + nodeObj.name;
					nodeObj.transform.parent = Root.transform;
					PreviewNode = nodeObj;
					break;
			}

			nodeObj.transform.localPosition = position;

			nodeObj.transform.localRotation = PositionToRotation(position);
			nodeObj.transform.localScale = Vector3.one * size;


			return nodeObj;
		}

		/// <summary>
		/// 一度配置したノードのシェイプを一括変更
		/// </summary>
		/// <param name="mesh"></param>
		public void ChangeSphapeType(Mesh mesh)
		{
			if(previewMaterial!=null)
				PreviewNode.GetComponent<MeshFilter>().mesh = mesh;

			for(int i = 0; i < AdditiveNodes.Count; i++)
			{
				AdditiveNodes[i].GetComponent<MeshFilter>().mesh = mesh;
			}
			for(int i=0;i<InitNodes.Count;i++)
			{
				InitNodes[i].GetComponent<MeshFilter>().mesh = mesh;
			}
		}

		/// <summary>
		/// プレビューノード作成
		/// </summary>
		public void CreatePreviewNode()
		{
			//球を作った瞬間見えちゃうので、一旦カメラの後ろに隠した位置に生成しています
			Transform cameraTrs = Camera.main.transform;
			GameObject obj = CreateNode(cameraTrs.position - cameraTrs.forward, manager.Param.PreviewNodeSize, NodeType.PREVIEW, manager.PreviewMaterial);
			previewMaterial = obj.GetComponent<Renderer>().material;
		}

		/// <summary>
		/// 位置座標から回転値を求める
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		Quaternion PositionToRotation(Vector3 position)
		{
			//回転決定。中心縦軸はLookRotation出来ないので除外
			Vector3 p = position;
			p.y = 0.0f;
			if (p.sqrMagnitude > float.Epsilon)
				return Quaternion.LookRotation(Vector3.zero - position, Vector3.up);
			return Quaternion.identity;
		}

		/// <summary>
		/// 追加するノード作成
		/// </summary>
		/// <returns>ノード作成成功したかどうか</returns>
		public bool CreateAdditiveNode()
		{
			if (manager.Param.MaxAdditiveNodeNum <= AdditiveNodes.Count)
				return false;
			CreateNode(PreviewNode.transform.localPosition, manager.Param.AdditiveNodeSize, NodeType.ADDITIVE, manager.Material);
			return true;
		}

		/// <summary>
		/// プレビューノード更新
		/// </summary>
		/// <param name="color"></param>
		/// <param name="isInImage"></param>
		public void UpdatePreviewNode(Color color, bool isInImage)
		{

			if (!isInImage)
			{
				//画像の外にマウスカーソルがある時は、サイズを0にして隠す
				PreviewNode.transform.localScale = Vector3.zero;
			}
			else
			{
				previewMaterial.color = color;
				HSL hsl = HSL.RGBToHSL(color);
				PreviewNode.transform.localPosition = hsl.ToPosition();
				PreviewNode.transform.localRotation = PositionToRotation(PreviewNode.transform.localPosition);
				PreviewNode.transform.localScale = Vector3.one * manager.Param.PreviewNodeSize;

			}
		}


		/// <summary>
		/// プレビューノードの削除
		/// </summary>
		public void DeletePreviewNode()
		{
			GameObject.Destroy(PreviewNode);
			PreviewNode = null;
			previewMaterial = null;
		}

		/// <summary>
		/// Undo　最後に作ったノードから順番に削除
		/// </summary>
		public void UndoAdditiveNode()
		{
			if (AdditiveNodes.Count == 0)
				return;
			GameObject.Destroy(AdditiveNodes[AdditiveNodes.Count - 1]);
			AdditiveNodes.RemoveAt(AdditiveNodes.Count - 1);
		}

		/// <summary>
		/// 全ての追加ノードの削除
		/// </summary>
		public void ClearAllAdditiveNode()
		{
			foreach (GameObject g in AdditiveNodes)
			{
				GameObject.Destroy(g);
			}
			AdditiveNodes.Clear();
		}

		/// <summary>
		/// 実行中のノードサイズ変更
		/// </summary>
		public void ChangeNodeSize(NodeType nodeType, float size)
		{
			switch (nodeType)
			{
				case NodeType.ADDITIVE:
					if (AdditiveNodes == null || AdditiveNodes.Count == 0)
						return;

					for (int i = 0; i < AdditiveNodes.Count; i++)
					{
						AdditiveNodes[i].transform.localScale = Vector3.one * size;
					}
					break;

				case NodeType.INIT:
					if (InitNodes == null || InitNodes.Count == 0)
						return;

					for (int i = 0; i < InitNodes.Count; i++)
					{
						float t = InitNodes[i].transform.localPosition.magnitude;
						float centerSmall = Mathf.Lerp(1.0f, t, manager.Param.InitNodeCenterSmall);
						InitNodes[i].transform.localScale = Vector3.one * size * centerSmall;
					}
					break;

				case NodeType.PREVIEW:
					if (PreviewNode == null)
						return;
					PreviewNode.transform.localScale = Vector3.one * size;
					break;
			}
		}

		/// <summary>
		/// グリッドの表示
		/// </summary>
		public void ShowGrid()
		{
			grid.SetActive(!grid.activeSelf);
		}

		/// <summary>
		/// 現在表示中のノードをファイルに保存
		/// </summary>
		public bool Save()
		{
			if (AdditiveNodes.Count == 0)
				return false;

			//ファイルパス生成
			var path = "Assets/SaveData/";
			var pathCSV = "Assets/SaveDataCSV/";

			//フォルダなかった作成
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			if (!Directory.Exists(pathCSV))
				Directory.CreateDirectory(pathCSV);


			DateTime dt = DateTime.Now;
			string now = dt.Year.ToString("d4") + dt.Month.ToString("d2") + dt.Day.ToString("d2") + dt.Hour.ToString("d2") + dt.Minute.ToString("d2") + dt.Second.ToString("d2");
			var fileName = manager.Picture.name + "_" + now;

			//ScriptableObject作成
			var saveData = ScriptableObject.CreateInstance<SaveData>();

			//CSVデータ作成
			StreamWriter csvSw;
			FileInfo csvFI = new FileInfo(pathCSV + fileName + ".csv");
			csvSw = csvFI.AppendText();
			csvSw.WriteLine("PosX,PosY,PosZ,H(0.0-1.0),S(0.0-1.0),L(0.0-1.0),R(0-255),G(0-255),B(0-255),RGB(16)");

			Vector3[] positions = new Vector3[AdditiveNodes.Count];
			for (int i = 0; i < positions.Length; i++)
			{
				positions[i] = AdditiveNodes[i].transform.localPosition;

				HSL hsl = HSL.PositionToHSL(positions[i]);
				Color rgb = hsl.ToRgb();

				csvSw.WriteLine(positions[i].x + "," + positions[i].y + "," + positions[i].z + "," +
					hsl.h + "," + hsl.s + "," + hsl.l + "," +
					(int)(rgb.r * 255.0f) + "," + (int)(rgb.g * 255.0f) + "," + (int)(rgb.b * 255.0f)+","+ Utility.ColorTo16(rgb));


			}

			//ScriptableObject保存
			saveData.Position = positions;
			AssetDatabase.CreateAsset(saveData, path + fileName + ".asset");

			//CSV保存
			csvSw.Flush();
			csvSw.Close();
			return true;
		}
	}
}