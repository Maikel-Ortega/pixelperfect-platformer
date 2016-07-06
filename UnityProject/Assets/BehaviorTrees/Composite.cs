using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

namespace BehaviourTrees
{
	public class Composite: Behaviour
	{
		protected List<IBehaviour> Children {get; set;}
		protected Composite()
		{
			Children = new List<IBehaviour>();
		}
	}
}
