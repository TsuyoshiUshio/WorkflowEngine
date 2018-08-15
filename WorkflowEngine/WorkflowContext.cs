using System;
using System.Collections.Generic;
using System.Text;

namespace WorkflowEngine
{
    public enum Process
    {
        Started = 0,
        Arrived = 1,
        Completed = 2,
    }

    public enum Event
    {
        Next = 0
    }

    public class WorkflowContext
    {
        public Process CurrentProcess { get; set; } // When I use private, enum serialize not working.
        private Dictionary<Process, Dictionary<Event, Process>> stateMachine;
        public string Message { get; set; }

        public void AppendMessage(string message)
        {
            this.Message = this.Message + message;
        }

        public WorkflowContext()
        {
            this.CurrentProcess = Process.Started;
            this.stateMachine = new Dictionary<Process, Dictionary<Event, Process>>()
            {
                {
                    Process.Started, new Dictionary<Event, Process>()
                    {
                        { Event.Next, Process.Arrived }
                    }
                },
                {
                    Process.Arrived, new Dictionary<Event, Process>()
                    {
                        { Event.Next, Process.Completed }
                    }
                }
            };
            Message = "";
         }


        /// <summary>
        /// Transite
        /// </summary>
        /// <param name=""></param>
        public void Transit(Event e) {
            Dictionary<Event, Process> route;
            this.stateMachine.TryGetValue(this.CurrentProcess, out route);
            Process nextProcess;
            route.TryGetValue(e, out nextProcess);
            this.CurrentProcess = nextProcess;
        }
    }
}
