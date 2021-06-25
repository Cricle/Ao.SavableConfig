using Ao.SavableConfig.Saver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json.Node;
using System.Text.Json;

namespace Ao.SavableConfig.Json.Test
{
    [TestClass]
    public class JsonConfigurationVisitorTest
    {
        private JsonNode CreateObject(object value)
        {
            var text = JsonSerializer.Serialize(value);
            return JsonObject.Parse(text);
        }
        [TestMethod]
        public void RaiseModifyProperty_MustModified()
        {
            var origin = CreateObject(new
            {
                Name = "jho"
            });
            var path = new string[] { "Name" };
            JsonNode val = ("pite");
            var visitor = new JsonConfigurationVisitor(path, origin, val);
            visitor.VisitWrite();
            var modified = origin["Name"].ToString();
            Assert.AreEqual(val.ToString(), modified);
        }
        [TestMethod]
        public void RaiseAddProperty_MustAdded()
        {
            var origin = CreateObject(new { });
            var path = new string[] { "Name" };
            JsonNode val = ("pite");
            var visitor = new JsonConfigurationVisitor(path, origin, val);
            visitor.VisitWrite();
            var modified = origin["Name"].ToString();
            Assert.AreEqual(val.ToString(), modified);
        }
        [TestMethod]
        public void RaiseAddObject_MustAdded()
        {
            var origin = CreateObject(new { });
            var path = new string[] { "Name", "Text" };
            JsonNode val = ("pite");
            var visitor = new JsonConfigurationVisitor(path, origin, val);
            visitor.VisitWrite();
            var modified = origin["Name"]["Text"].ToString();
            Assert.AreEqual(val.ToString(), modified);
        }
        [TestMethod]
        public void RaiseAddObjectWithArray_MustAdded()
        {
            var origin = CreateObject(new { });
            var path = new string[] { "Name", "Text", "2", "Hello" };
            JsonNode val = ("pite");
            var visitor = new JsonConfigurationVisitor(path, origin, val);
            visitor.VisitWrite();
            var modified = origin["Name"]["Text"][2]["Hello"].ToString();
            Assert.AreEqual(val.ToString(), modified);
        }
        [TestMethod]
        public void RaiseAddObjectWithIgnoreAdd_MustNotAdded()
        {
            var origin = CreateObject(new { });
            var path = new string[] { "Name", "Text" };
            JsonNode val = ("pite");
            var visitor = new JsonConfigurationVisitor(path, origin, val);
            visitor.IgnoreAdd = true;
            visitor.VisitWrite();
            Assert.AreEqual(0, ((JsonObject)origin).Count);
        }
        [TestMethod]
        public void RaiseModifyObject_WhenArray_NothingToDo()
        {
            var origin = CreateObject(new
            {
                Name = new string[0]
            });
            var path = new string[] { "Name", "Text" };
            JsonNode val = ("pite");
            var visitor = new JsonConfigurationVisitor(path, origin, val);
            visitor.VisitWrite();
            var modified = origin["Name"];
            Assert.IsInstanceOfType(modified, typeof(JsonArray));
        }
        [TestMethod]
        public void RaiseModifyArray_MustModified()
        {
            var origin = CreateObject(new
            {
                Ids = new int[] { 1, 2, 3, 4, 5 }
            });
            var path = new string[] { "Ids", "2" };
            JsonNode val = ("a");
            var visitor = new JsonConfigurationVisitor(path, origin, val);
            visitor.VisitWrite();
            var modified = origin["Ids"][2].ToString();
            Assert.AreEqual(val.ToString(), modified);
        }
        [TestMethod]
        public void RaiseAddArray_MustAdded()
        {
            var origin = CreateObject(new
            {
                Ids = new int[] { 1, 2 }
            });
            var path = new string[] { "Ids", "4" };
            JsonNode val = ("a");
            var visitor = new JsonConfigurationVisitor(path, origin, val);
            visitor.VisitWrite();
            var modified = origin["Ids"][4].ToString();
            Assert.AreEqual(val.ToString(), modified);
        }
    }
}
