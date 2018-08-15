using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowEngine;

namespace WorkflowEngine.Test
{
    [TestClass]
    public class WorkflowContextTest
    {
        [TestMethod]
        public void TestWorkflowContext()
        {
            var context = new WorkflowContext();
            Assert.AreEqual(Process.Started, context.CurrentProcess);
            context.Transit(Event.Next);
            Assert.AreEqual(Process.Arrived, context.CurrentProcess);

        }
    }
}
