using UnityEngine;


namespace IroSphere
{
	/// <summary>
	/// セーブデータ
	/// </summary>
	[CreateAssetMenu(menuName = "IroSphere/SaveData", fileName = "SaveData")]
	public class SaveData : ScriptableObject
	{
		[SerializeField]
		Vector3[] positions;

		public Vector3[] Position { get { return positions; } set { positions = value; } }

	}
}