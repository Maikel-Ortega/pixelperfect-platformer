using UnityEngine;
using System;
using System.Collections;

namespace BehaviourTrees
{
	public class Behaviour : IBehaviour
	{
		public Func<Status> 	Update 		{protected get; set;}
		public Action 			Initialize 	{ protected get; set; }
		public Action<Status> 	Terminate 	{protected get; set;}
		public Status 			Status 		{	get; set; }	


		public Status 	Tick ()
		{
			if( Status == Status.ERROR && Initialize != null) 	//If Status == ERROR we still arent Initialized
			{
				Initialize();
			}

			Status = Update();					

			if(Status != Status.RUNNING && Terminate != null)	//If status != Running, then is either Success or Failure, so we have to Terminate()
			{
				Terminate(Status);
			}

			return Status;
		}
	}
}
