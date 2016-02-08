// ====================================================================================================================
// -== UniRPG ==-
// www.plyoung.com
// Copyright (c) 2013 by Leslie Young
// ====================================================================================================================

using UnityEngine;
using System.Collections.Generic;

namespace UniRPG {

[AddComponentMenu("")]
public class AnimationAction : Action
{
	[System.Serializable]
	public class AniInfo
	{
		public StringValue clipName = new StringValue();
		public NumericValue speed = new NumericValue(1f);
		public WrapMode wrapMode = WrapMode.Default;
		public bool crossFade = false;
		public bool reversed = false;
	}

	public StringValue clipName = new StringValue();
	public NumericValue speed = new NumericValue(1f);
	public WrapMode wrapMode = WrapMode.Default;
	public bool crossFade = false;
	public bool reversed = false;
	
	public bool useRandomClips = false;
	public List<AniInfo> aniList = new List<AniInfo>(0);

	public enum DoWhat { Stop, Play }
	public DoWhat doWhat = DoWhat.Play;

	public override void CopyTo(Action act)
	{
		base.CopyTo(act);
		AnimationAction a = act as AnimationAction;
		a.clipName = this.clipName.Copy();
		a.speed = this.speed.Copy();
		a.wrapMode = this.wrapMode;
		a.crossFade = this.crossFade;
		a.reversed = this.reversed;
		a.doWhat = this.doWhat;
	}

	public override ReturnType Execute(GameObject self, GameObject targeted, GameObject selfTargetedBy, GameObject equipTarget, GameObject helper)
	{
		GameObject obj = DetermineTarget(self, targeted, selfTargetedBy, equipTarget, helper);
		if (obj == null)
		{
			Debug.LogError("Animation Action Error: The subject did not exist.");
			return ReturnType.Done;
		}

		if (obj.GetComponent<Animation>())
		{
			if (doWhat == DoWhat.Stop)
			{
				obj.GetComponent<Animation>().Stop();
			}
			else
			{
				if (useRandomClips)
				{
					if (aniList.Count > 0)
					{
						int i = 0;
						if (aniList.Count > 1) i = Random.Range(0, aniList.Count);
						AnimationState ani = obj.GetComponent<Animation>()[aniList[i].clipName.Value(self, targeted, selfTargetedBy, equipTarget, helper)];
						if (ani)
						{
							if (aniList[i].reversed)
							{
								ani.time = ani.length;
								ani.speed = -1 * aniList[i].speed.Value(self, targeted, selfTargetedBy, equipTarget, helper);
							}
							else
							{
								ani.time = 0;
								ani.speed = aniList[i].speed.Value(self, targeted, selfTargetedBy, equipTarget, helper);
							}
							ani.wrapMode = aniList[i].wrapMode;
							if (aniList[i].crossFade) obj.GetComponent<Animation>().CrossFade(aniList[i].clipName.Value(self, targeted, selfTargetedBy, equipTarget, helper));
							else obj.GetComponent<Animation>().Play(aniList[i].clipName.Value(self, targeted, selfTargetedBy, equipTarget, helper));
						}
						else Debug.LogError(string.Format("Animation Action Error: The subject did not have a clip named [{0}]", clipName.Value(self, targeted, selfTargetedBy, equipTarget, helper)));
					}
					else Debug.LogError("Animation Action Error: There are no clips defined");
				}

				else
				{
					AnimationState ani = obj.GetComponent<Animation>()[clipName.Value(self, targeted, selfTargetedBy, equipTarget, helper)];
					if (ani)
					{
						if (reversed)
						{
							ani.time = ani.length;
							ani.speed = -1 * speed.Value(self, targeted, selfTargetedBy, equipTarget, helper);
						}
						else
						{
							ani.time = 0;
							ani.speed = speed.Value(self, targeted, selfTargetedBy, equipTarget, helper);
						}
						ani.wrapMode = wrapMode;
						if (crossFade) obj.GetComponent<Animation>().CrossFade(clipName.Value(self, targeted, selfTargetedBy, equipTarget, helper));
						else obj.GetComponent<Animation>().Play(clipName.Value(self, targeted, selfTargetedBy, equipTarget, helper));
					}
					else Debug.LogError(string.Format("Animation Action Error: The subject did not have a clip named [{0}]", clipName.Value(self, targeted, selfTargetedBy, equipTarget, helper)));
				}
			}
		}
		else Debug.LogError("Animation Action Error: The subject did not have any Animation Component.");

		return ReturnType.Done; // this action is done
	}

	// ================================================================================================================
} }