using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriggerObjectDT : MonoBehaviour {
	public bool isList = false;
	public List<TriggerDT> triggersList = new List<TriggerDT> ();
	public bool[] triggerActiveList;
	public List<TriggerDT> localTriggersItems;
}
