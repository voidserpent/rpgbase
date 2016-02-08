using UnityEngine;
using System.Collections;
using UniRPG;

public class MyGlobal : MonoBehaviour 
{
	public string objectVarName = "MyGlobal";
	public Object objectToSave = null;

	void Awake()
	{
		Object.DontDestroyOnLoad(gameObject);
	}

	void Start() 
	{
		UniRPGGlobal.DB.SetGlobalObject(objectVarName, objectToSave == null ? gameObject : objectToSave);
	}
}
