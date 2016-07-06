using UnityEngine;
using System;
using System.Collections;

namespace BehaviourTrees
{
	public interface IBehaviour 
	{
		
		Status Status{get; set;}
		Status Tick();

		Action 			Initialize{set;}
		Func<Status> 	Update{set; }
		Action<Status> 	Terminate {set;}

	}
}