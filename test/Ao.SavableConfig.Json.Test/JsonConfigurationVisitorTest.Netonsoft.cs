using Ao.SavableConfig.Saver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Ao.SavableConfig.Json.Test
{
    [TestClass]
    public class JsonConfigurationVisitorTest
    {
        [TestMethod]
        public void RaiseModifyProperty_MustModified()
        {
            var origin = JObject.FromObject(new
            {
                Name = "jho"
            });
            var path = new string[] { "Name" };
            JValue val = new JValue("pite");
            var visitor = new JsonConfigurationVisitor(path, origin, val);
            visitor.VisitWrite();
            var modified = origin["Name"].ToString();
            Assert.AreEqual(val.ToString(), modified);
        }
        [TestMethod]
        public void RaiseAddProperty_MustAdded()
        {
            var origin = JObject.FromObject(new { });
            var path = new string[] { "Name" };
            JValue val = new JValue("pite");
            var visitor = new JsonConfigurationVisitor(path, origin, val);
            visitor.VisitWrite();
            var modified = origin["Name"].ToString();
            Assert.AreEqual(val.ToString(), modified);
        }
        [TestMethod]
        public void RaiseAddObject_MustAdded()
        {
            var origin = JObject.FromObject(new { });
            var path = new string[] { "Name", "Text" };
            JValue val = new JValue("pite");
            var visitor = new JsonConfigurationVisitor(path, origin, val);
            visitor.VisitWrite();
            var modified = origin["Name"]["Text"].ToString();
            Assert.AreEqual(val.ToString(), modified);
        }
        [TestMethod]
        public void RaiseAddObjectWithArray_MustAdded()
        {
            var origin = JObject.FromObject(new { });
            var path = new string[] { "Name", "Text", "2", "Hello" };
            JValue val = new JValue("pite");
            var visitor = new JsonConfigurationVisitor(path, origin, val);
            visitor.VisitWrite();
            var modified = origin["Name"]["Text"][2]["Hello"].ToString();
            Assert.AreEqual(val.ToString(), modified);
        }
        [TestMethod]
        public void RaiseAddObjectWithIgnoreAdd_MustNotAdded()
        {
            var origin = JObject.FromObject(new { });
            var path = new string[] { "Name", "Text" };
            JValue val = new JValue("pite");
            var visitor = new JsonConfigurationVisitor(path, origin, val);
            visitor.IgnoreAdd = true;
            visitor.VisitWrite();
            Assert.AreEqual(0, origin.Count);
        }
        [TestMethod]
        public void RaiseModifyObject_WhenArray_NothingToDo()
        {
            var origin = JObject.FromObject(new
            {
                Name = new string[0]
            });
            var path = new string[] { "Name", "Text" };
            JValue val = new JValue("pite");
            var visitor = new JsonConfigurationVisitor(path, origin, val);
            visitor.VisitWrite();
            var modified = origin["Name"];
            Assert.IsInstanceOfType(modified, typeof(JArray));
        }
        [TestMethod]
        public void RaiseModifyArray_MustModified()
        {
            var origin = JObject.FromObject(new
            {
                Ids = new int[] { 1, 2, 3, 4, 5 }
            });
            var path = new string[] { "Ids", "2" };
            JValue val = new JValue("a");
            var visitor = new JsonConfigurationVisitor(path, origin, val);
            visitor.VisitWrite();
            var modified = origin["Ids"][2].ToString();
            Assert.AreEqual(val.ToString(), modified);
        }
        [TestMethod]
        public void RaiseAddArray_MustAdded()
        {
            var origin = JObject.FromObject(new
            {
                Ids = new int[] { 1, 2 }
            });
            var path = new string[] { "Ids", "4" };
            JValue val = new JValue("a");
            var visitor = new JsonConfigurationVisitor(path, origin, val);
            visitor.VisitWrite();
            var modified = origin["Ids"][4].ToString();
            Assert.AreEqual(val.ToString(), modified);
        }
    }
}
