using UnityEngine;
using UnityEditor;


namespace XiSound.Events.Editor
{
	/// <summary>
	/// The sound event editor tool
	/// </summary>
	[CustomEditor(typeof(BaseAudioEvent), true)]
	public class AudioEventEditor : UnityEditor.Editor
	{

		[SerializeField] private AudioSource _previewer;

		public void OnEnable()
		{
			_previewer = EditorUtility.CreateGameObjectWithHideFlags("Audio preview", HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();
		}

		public void OnDisable()
		{
			DestroyImmediate(_previewer.gameObject);
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
			if (GUILayout.Button("Preview"))
			{
				((BaseAudioEvent) target).Play(_previewer);
			}
			EditorGUI.EndDisabledGroup();
		}
	}
}
