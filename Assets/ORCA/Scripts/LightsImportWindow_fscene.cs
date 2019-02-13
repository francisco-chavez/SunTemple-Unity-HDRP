
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class LightsImportWindow_fscene 
	: EditorWindow
{

	#region Attributes

	private static string _filePath = string.Empty;

	#endregion


	#region Unity Messages

	private void OnEnable()
	{
		if (string.IsNullOrEmpty(_filePath))
			_filePath = Application.dataPath;
	}

	private void OnGUI()
	{
		GUILayout.Label("File Select:", EditorStyles.boldLabel);
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Fscene path:", GUILayout.ExpandWidth(false));
		EditorGUILayout.TextField(_filePath, GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal();

		bool openFileBrowswer = GUILayout.Button("Browse...", GUILayout.ExpandWidth(false));

		if (openFileBrowswer)
		{
			var directory = Application.dataPath;

			if (Directory.Exists(_filePath))
			{
				directory = _filePath;
			}
			else if (File.Exists(_filePath))
			{
				var fileName = Path.GetFileName(_filePath);
				directory = _filePath.Substring(0, _filePath.Length - fileName.Length);
			}

			var pathT = EditorUtility.OpenFilePanel("Select fscene", directory, "fscene");

			_filePath = string.IsNullOrEmpty(pathT)
					  ? _filePath
					  : pathT;
		}

		// Do some basic checking to make sure there is an fscene before giving the User 
		// the option of importing from it.
		if (!File.Exists(_filePath))
			return;
		if (!_filePath.EndsWith(".fscene", System.StringComparison.OrdinalIgnoreCase))
			return;

		GUILayout.Space(10);
		bool importLights = GUILayout.Button("Import Lights");

		if (importLights)
			ImportLights(_filePath);
	}

	// Ok, not really one of the Unity Message methods, but with the integration 
	// provided by the MenuItemAttribute, it might as well be one.
	[MenuItem("Window/ORCA/Import Lights")]
	static void Init()
	{
		var window = EditorWindow.GetWindow<LightsImportWindow_fscene>();
		window.Show();
	}

	#endregion


	#region Methods

	private void ImportLights(string fscenePath)
	{
		using (var streamReader = new StreamReader(fscenePath))
		{
			JObject fscene = JObject.Parse(streamReader.ReadToEnd());

			var jLights = (JArray) fscene["lights"];

			var root = new GameObject("Lights:");

			foreach (var l in jLights)
			{
				var lightName	= l["name"].ToString();
				var lightType	= l["type"].ToString();
				var rawColor	= l["intensity"];
				var direction	= l["direction"];

				if (lightType == "dir_light" || lightType == "point_light")
				{

					GameObject lightObject = new GameObject(lightName, new System.Type[] { typeof(Light) });
					Light light = lightObject.GetComponent<Light>();
					light.transform.SetParent(root.transform);
					light.transform.forward = new Vector3(-float.Parse(direction[0].ToString()), 
														  +float.Parse(direction[1].ToString()), 
														  +float.Parse(direction[2].ToString()));
					light.color = new Color(float.Parse(rawColor[0].ToString()),
											float.Parse(rawColor[1].ToString()),
											float.Parse(rawColor[2].ToString()));
					light.lightmapBakeType = LightmapBakeType.Mixed;


					if (lightType == "dir_light")
					{
						light.transform.localPosition = new Vector3(0.0f, 30.0f, 0.0f);
						light.type = LightType.Directional;
						light.shadows = LightShadows.Soft;
					}
					else
					{
						light.type = LightType.Spot;
						light.shadows = LightShadows.Hard;

						var rawPos = l["pos"];


						light.transform.localPosition = new Vector3(-float.Parse(rawPos[0].ToString()),
																	+float.Parse(rawPos[1].ToString()),
																	+float.Parse(rawPos[2].ToString()));
						light.spotAngle = Mathf.Min(145.0f, 
													float.Parse(l["opening_angle"].ToString()));
					}
				}
			}
		}
	}

	#endregion

}
