// -== UniRPG ==-
// www.plyoung.com
// Copyright (c) 2013 by Leslie Young
// ====================================================================================================================

using UnityEngine;
using System.Collections.Generic;

namespace UniRPG {

// ====================================================================================================================

public delegate void UniRPGBasicEventHandler(object sender);
public delegate void UniRPGArgsEventHandler(object sender, object[] args);

// ====================================================================================================================

[System.Serializable]
public class BagSlot
{
	public RPGItem item = null;	// the item in the slot
	public int stack = 0;		// the number of copies of this item in the slot
}

public class ActionSlot
{	// Not to be confused with Actions. This is the slot where Items or Skills are palced. 
	// Normally a action bar is presneted to the player

	private RPGItem item = null;
	private RPGSkill skill = null;

	public bool IsEmpty { get { return item == null && skill == null; } }

	public bool IsSkill { get { return skill != null; } }

	public bool IsItem { get { return item != null; } }

	public RPGSkill Skill { get { return skill; } }

	public RPGItem Item { get { return item; } }

	public void SetAsSkill(RPGSkill skill)
	{
		this.item = null;
		this.skill = skill;
	}

	public void SetAsItem(RPGItem item)
	{
		this.item = item;
		this.skill = null;
	}

	public Texture2D GetIcon(int id, Texture2D defaultIcon)
	{
		if (item != null) return item.icon[id];
		if (skill != null) return skill.icon[id];
		return defaultIcon;
	}

	public void Clear()
	{
		item = null;
		skill = null;
	}
}

// ====================================================================================================================
// These are used when defining a global variable

[System.Serializable]
public class NumericVar
{
	public string name;
	public float val;
}

[System.Serializable]
public class StringVar
{
	public string name;
	public string val;
}

[System.Serializable]
public class ObjectVar
{
	public string name;
	public UnityEngine.Object val;
}

// ====================================================================================================================
// These are used where the designer can choose to either use a global var or direct value
// if the name of the variable is null then the val will be used

[System.Serializable]
public class NumericValue
{
	public string numericVarName; // note, do not depend on this being null since unity serialiser will make this an empty string if null
	[SerializeField] private float val;

	public string customVarName;
	public ActionTarget customVarSubject = new ActionTarget();
	public bool isCustom = false;

	private int cachedIdx = -1;

	public NumericValue()
	{
		this.val = 0f;
	}

	public NumericValue(float val)
	{
		this.val = val;
	}
	
	public float Val
	{
		set
		{
			this.val = value;
			this.isCustom = false;
			this.customVarName = null;
			this.numericVarName = null;
			this.cachedIdx = -1;
		}

		get
		{
			return this.val;
		}
	}

	public float Value(GameObject self, GameObject targeted, GameObject selfTargetedBy, GameObject equipTarget, GameObject helper)
	{
		if (isCustom)
		{
			UniqueMonoBehaviour t = null;
			GameObject obj = Action.DetermineTarget(customVarSubject, self, targeted, selfTargetedBy, equipTarget, helper);
			if (obj != null) t = obj.GetComponent<UniqueMonoBehaviour>();
			return GetValue(UniRPGGlobal.DB, t);
		}
		else
		{
			return GetValue(UniRPGGlobal.DB, null);
		}
	}

	public void SetValue(Database db, float value, UniqueMonoBehaviour customVarTarget)
	{
		this.val = value;
		if (db != null)
		{
			if (cachedIdx >= 0)
			{
				db.numericVars[cachedIdx].val = value;
			}
			else
			{
				if (isCustom)
				{
					if (customVarTarget != null)
					{
						customVarTarget.SetCustomVariable(customVarName, value.ToString());
					}
					else Debug.LogError("No target specified while trying to access a custom variable.");
				}

				else if (!string.IsNullOrEmpty(numericVarName))
				{
					for (int i = 0; i < db.numericVars.Count; i++)
					{
						if (db.numericVars[i].name.Equals(numericVarName))
						{
							cachedIdx = i;
							db.numericVars[i].val = value;
							break;
						}
					}
				}
			}
		}
		else
		{
			this.isCustom = false;
			this.customVarName = null;
			this.numericVarName = null;
			this.cachedIdx = -1;
		}
	}

	public float GetValue(Database db, UniqueMonoBehaviour customVarTarget)
	{
		if (cachedIdx >= 0) return db.numericVars[cachedIdx].val;

		float ret = val;

		if (isCustom)
		{
			if (customVarTarget != null)
			{
				string s = customVarTarget.GetCustomVariable(customVarName);
				if (!float.TryParse(s, out ret)) ret = 0f;
			}
			else Debug.LogError("No target specified while trying to access a custom variable.");
		}

		else if (!string.IsNullOrEmpty(numericVarName) && db != null)
		{
			for (int i = 0; i < db.numericVars.Count; i++)
			{
				if (db.numericVars[i].name.Equals(numericVarName))
				{
					cachedIdx = i;
					ret = db.numericVars[i].val;
					break;
				}
			}
		}

		return ret;
	}

	public string GetValOrName()
	{
		if (isCustom) return "custom: " + customVarName;
		if (!string.IsNullOrEmpty(numericVarName)) return "global: " + numericVarName;
		return val.ToString();
	}

	public NumericValue Copy()
	{
		NumericValue n = new NumericValue();
		n.isCustom = this.isCustom;
		n.customVarSubject = this.customVarSubject.Copy();
		n.numericVarName = this.numericVarName;
		n.customVarName = this.customVarName;
		n.val = this.val;
		return n;
	}
}

[System.Serializable]
public class StringValue
{
	public string stringVarName; // note, do not depend on this being null since unity serialiser will make this an empty string if null
	[SerializeField] private string val;

	public string customVarName;
	public ActionTarget customVarSubject = new ActionTarget();
	public bool isCustom = false;

	private int cachedIdx = -1;

	public StringValue()
	{
		this.val = "";
	}

	public StringValue(string val)
	{
		this.val = val;
	}

	public string Val
	{
		set
		{
			this.val = value;
			this.isCustom = false;
			this.customVarName = null;
			this.stringVarName = null;
			this.cachedIdx = -1;
		}

		get
		{
			return this.val;
		}
	}

	public string Value(GameObject self, GameObject targeted, GameObject selfTargetedBy, GameObject equipTarget, GameObject helper)
	{
		if (isCustom)
		{
			UniqueMonoBehaviour t = null;
			GameObject obj = Action.DetermineTarget(customVarSubject, self, targeted, selfTargetedBy, equipTarget, helper);
			if (obj != null) t = obj.GetComponent<UniqueMonoBehaviour>();
			return GetValue(UniRPGGlobal.DB, t);
		}
		else
		{
			return GetValue(UniRPGGlobal.DB, null);
		}
	}

	public void SetValue(Database db, string value, UniqueMonoBehaviour customVarTarget)
	{
		this.val = value;
		if (db != null)
		{
			if (cachedIdx >= 0)
			{
				db.stringVars[cachedIdx].val = value;
			}
			else
			{
				if (isCustom)
				{
					if (customVarTarget != null)
					{
						customVarTarget.SetCustomVariable(customVarName, value);
					}
					else Debug.LogError("No target specified while trying to access a custom variable.");
				}

				else if (!string.IsNullOrEmpty(stringVarName))
				{
					for (int i = 0; i < db.stringVars.Count; i++)
					{
						if (db.stringVars[i].name.Equals(stringVarName))
						{
							cachedIdx = i;
							db.stringVars[i].val = value;
							break;
						}
					}
				}
			}
		}
		else
		{
			this.isCustom = false;
			this.customVarName = null;
			this.stringVarName = null;
			this.cachedIdx = -1;
		}
	}

	public string GetValue(Database db, UniqueMonoBehaviour customVarTarget)
	{
		if (cachedIdx >= 0) return db.stringVars[cachedIdx].val;
		
		string ret = val;

		if (isCustom)
		{
			if (customVarTarget != null)
			{
				ret = customVarTarget.GetCustomVariable(customVarName);
			}
			else Debug.LogError("No target specified while trying to access a custom variable.");
		}

		else if (!string.IsNullOrEmpty(stringVarName) && db != null)
		{
			for (int i = 0; i < db.stringVars.Count; i++)
			{
				if (db.stringVars[i].name.Equals(stringVarName))
				{
					cachedIdx = i;
					ret = db.stringVars[i].val;
					break;
				}
			}
		}

		return ret;
	}

	public string GetValOrName()
	{
		if (isCustom) return "custom: " + customVarName;
		if (!string.IsNullOrEmpty(stringVarName)) return "global: " + stringVarName;
		return val.ToString();
	}

	public StringValue Copy()
	{
		StringValue n = new StringValue();
		n.isCustom = this.isCustom;
		n.customVarSubject = this.customVarSubject.Copy();
		n.stringVarName = this.stringVarName;
		n.customVarName = this.customVarName;
		n.val = this.val;
		return n;
	}
}

[System.Serializable]
public class ObjectValue
{
	public string objectVarName; // note, do not depend on this being null since unity serialiser will make this an empty string if null
	[SerializeField] private UnityEngine.Object val = null;

	private int cachedIdx = -1;

	public ObjectValue()
	{
		this.val = null;
	}

	public ObjectValue(UnityEngine.Object val)
	{
		this.val = val;
	}

	/// <summary>should only be used at runtime and not be called in editor scripts</summary>
	public UnityEngine.Object Value { get { return GetValue(UniRPGGlobal.DB); } set { SetValue(UniRPGGlobal.DB, value); } }

	/// <summary>this will set the value and break any global variable link. can be used by editor scripts</summary>
	public UnityEngine.Object SetAsValue { set { SetValue(null, value); } }

	/// <summary>
	/// set the value. will set the global var's value if on is linked and only if db is not null
	/// if db is null then this will assume you want to break the link with the gobal variable if one is set
	/// </summary>
	public void SetValue(Database db, UnityEngine.Object value)
	{
		if (db != null)
		{
			if (cachedIdx >= 0)
			{
				db.objectVars[cachedIdx].val = value;
				this.val = null; // dont link here if using the global
			}
			else
			{
				if (!string.IsNullOrEmpty(objectVarName))
				{
					for (int i = 0; i < db.stringVars.Count; i++)
					{
						if (db.numericVars[i].name.Equals(objectVarName))
						{
							cachedIdx = i;
							db.objectVars[i].val = value;
							this.val = null; // dont link here if using the global
							break;
						}
					}
				}
			}
		}
		else
		{
			this.val = value;
			this.objectVarName = null;
			this.cachedIdx = -1;
		}
	}

	/// <summary>get the value. returns the global var's value if this is linked with one</summary>
	public UnityEngine.Object GetValue(Database db)
	{
		if (cachedIdx >= 0) return db.objectVars[cachedIdx].val;
		UnityEngine.Object ret = val;
		if (!string.IsNullOrEmpty(objectVarName) && db != null)
		{
			for (int i = 0; i < db.objectVars.Count; i++)
			{
				if (db.objectVars[i].name.Equals(objectVarName))
				{
					cachedIdx = i;
					ret = db.objectVars[i].val;
					break;
				}
			}
		}
		return ret;
	}

	/// <summary>Will return the global var name if a var is linked, else the value. usefull for editor scripts</summary>
	public string GetValOrName()
	{
		if (!string.IsNullOrEmpty(objectVarName)) return objectVarName;
		if (val == null) return "";
		return val.ToString();
	}

	public ObjectValue Copy()
	{
		ObjectValue n = new ObjectValue();
		n.objectVarName = this.objectVarName;
		n.val = this.val;
		return n;
	}
}

// ====================================================================================================================
// GUID, used where I need to give something a unique identifier

[System.Serializable]
public class GUID: System.IEquatable<GUID>
{
	[SerializeField] private string savedIdent = string.Empty;
	private System.Guid _ident = System.Guid.Empty;

	public System.Guid Value
	{
		get
		{
			if (_ident == System.Guid.Empty)
			{
				if (!string.IsNullOrEmpty(savedIdent))
				{
					try
					{
						_ident = new System.Guid(savedIdent);
					}
					catch
					{
						_ident = System.Guid.Empty;
						savedIdent = _ident.ToString("N");
					}
				}
			}
			return _ident;
		}

		set
		{
			_ident = value;
			savedIdent = _ident.ToString("N");
		}
	}

	public bool IsEmpty
	{
		get
		{
			if (_ident == System.Guid.Empty)
			{
				if (!string.IsNullOrEmpty(savedIdent))
				{
					try
					{
						_ident = new System.Guid(savedIdent);
					}
					catch
					{
						_ident = System.Guid.Empty;
						savedIdent = _ident.ToString("N");
						return true;
					}
				}
				if (_ident == System.Guid.Empty) return true;
			}
			return false;
		}
	}

	/// <summary>creates empty id. note that id might not be empty after it is deserialised if this GUI was saved before</summary>
	public GUID() { }

	/// <summary>created id with given ident</summary>
	public GUID(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			_ident = System.Guid.Empty;
			savedIdent = _ident.ToString("N");
		}
		else
		{
			try
			{
				savedIdent = id;
				_ident = new System.Guid(savedIdent);
			}
			catch
			{
				_ident = System.Guid.Empty;
				savedIdent = _ident.ToString("N");
			}
		}
	}

	/// <summary>Creates and return a new (not empty) id</summary>
	public static GUID Create()
	{
		GUID id = new GUID();
		id.GenerateId();
		return id;
	}

	/// <summary>Creates and return new id if the passed id is null or empty, else return passed id</summary>
	public static GUID Create(GUID id)
	{
		if (id == null)
		{
			id = new GUID();
			id.GenerateId();
		}
		else
		{
			if (id.IsEmpty) id.GenerateId();
		}
		return id;
	}
	
	/// <summary>Creates a new value for this guid</summary>
	private void GenerateId()
	{
		_ident = System.Guid.NewGuid();
		savedIdent = _ident.ToString("N");
	}

	/// <summary>Returns a copy of this guid</summary>
	public GUID Copy()
	{
		GUID id = new GUID();
		id.Value = this.Value;
		return id;
	}

	public static bool operator ==(GUID a, GUID b)
	{
		// If both are null, or both are same instance, return true
		if (System.Object.ReferenceEquals(a, b)) return true;

		// If one is null, but not both, return false
		if (((object)a == null) || ((object)b == null)) return false;

		// Return true if the fields match
		return a.Value == b.Value;
	}

	public static bool operator !=(GUID a, GUID b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		// If parameter is null return false
		if (obj == null) return false;

		// If parameter cannot be cast to Point return false
		GUID p = obj as GUID;
		if ((System.Object)p == null) return false;

		// Return true if the fields match
		return Value == p.Value;
	}

	public bool Equals(GUID guid)
	{
		// If parameter is null return false
		if ((object)guid == null) return false;

		// Return true if the fields match
		return Value == guid.Value;
	}

	public override int GetHashCode()
	{
		//extract an integer from the beginning of the Guid
		byte[] _bytes = Value.ToByteArray();
		return ((int)_bytes[0]) | ((int)_bytes[1] << 8) | ((int)_bytes[2] << 16) | ((int)_bytes[3] << 24);
	}

	/// <summary>The string representation of the GUID's value. This string is valid for use with new GUID(string someStringID)</summary>
	public override string ToString()
	{
		return savedIdent;
	}

	/// <summary>Will return true if the list contains the gived guid</summary>
	public static bool ListContains(List<GUID> list, GUID id)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == id) return true;
		}
		return false;
	}
}


// ====================================================================================================================
}