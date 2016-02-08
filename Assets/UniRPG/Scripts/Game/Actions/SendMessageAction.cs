// ====================================================================================================================
// -== UniRPG ==-
// www.plyoung.com
// Copyright (c) 2013 by Leslie Young
// ====================================================================================================================

using UnityEngine;
using System.Collections;

namespace UniRPG {

[AddComponentMenu("")]
public class SendMessageAction : Action
{
	public bool sendToTaggedObjects = false;
	public string tagToUse = "";
	public string functionName = "";
	public int paramType = 0; // 0: none, 1:text, 2:numeric, 3:vector4, 4:GameObject, 5:object
	public StringValue param1 = new StringValue();
	public NumericValue param2 = new NumericValue();
	public ObjectValue param5 = new ObjectValue();
	public Vector4 param3 = Vector4.zero;
	public GameObject param4 = null;

	public override void CopyTo(Action act)
	{
		base.CopyTo(act);
		SendMessageAction a = act as SendMessageAction;
		a.sendToTaggedObjects = this.sendToTaggedObjects;
		a.tagToUse = this.tagToUse;
		a.functionName = this.functionName;
		a.paramType = this.paramType;
		a.param1 = this.param1.Copy();
		a.param2 = this.param2.Copy();
		a.param3 = this.param3;
		a.param4 = this.param4;
		a.param5 = this.param5;
	}

	public override ReturnType Execute(GameObject self, GameObject targeted, GameObject selfTargetedBy, GameObject equipTarget, GameObject helper)
	{
		if (sendToTaggedObjects)
		{
			if (string.IsNullOrEmpty(tagToUse))
			{
				Debug.LogError("SendMessage Action Error: The Tag is not set.");
			}
			else
			{
				GameObject[] objs = GameObject.FindGameObjectsWithTag(tagToUse);
				for (int i = 0; i < objs.Length; i++)
				{
					SendAMessageTo(objs[i], self, targeted, selfTargetedBy, equipTarget, helper);
				}
			}
		}
		else
		{
			GameObject obj = DetermineTarget(self, targeted, selfTargetedBy, equipTarget, helper);
			if (obj != null)
			{
				SendAMessageTo(obj, self, targeted, selfTargetedBy, equipTarget, helper);
			}
			else Debug.LogError("SendMessage Action Error: The subject did not exist.");
		}
		return ReturnType.Done;
	}

	private void SendAMessageTo(GameObject obj, GameObject self, GameObject targeted, GameObject selfTargetedBy, GameObject equipTarget, GameObject helper)
	{
		switch (paramType)
		{
			case 0: obj.SendMessage(functionName, SendMessageOptions.DontRequireReceiver); break;
			case 1: obj.SendMessage(functionName, param1.Value(self, targeted, selfTargetedBy, equipTarget, helper), SendMessageOptions.DontRequireReceiver); break;
			case 2: obj.SendMessage(functionName, param2.Value(self, targeted, selfTargetedBy, equipTarget, helper), SendMessageOptions.DontRequireReceiver); break;
			case 3: obj.SendMessage(functionName, param3, SendMessageOptions.DontRequireReceiver); break;
			case 4: obj.SendMessage(functionName, param4, SendMessageOptions.DontRequireReceiver); break;
			case 5: obj.SendMessage(functionName, param5.Value, SendMessageOptions.DontRequireReceiver); break;
		}
	}

	// ================================================================================================================
} }