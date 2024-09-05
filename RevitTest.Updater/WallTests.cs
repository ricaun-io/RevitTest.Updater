using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using NUnit.Framework;
using RevitTest.Updater.Utils;
using System.Linq;

namespace RevitTest.Updater
{
    public class WallTests : OneTimeOpenDocumentTest
    {
        public BuiltInParameterUpdater builtInParameterUpdater;
        [OneTimeSetUp]
        public void UpdaterRegister(UIApplication uiapp)
        {
            this.builtInParameterUpdater = new BuiltInParameterUpdater(uiapp.ActiveAddInId);
            this.builtInParameterUpdater.Register();
        }

        [OneTimeTearDown]
        public void UpdaterUnRegister()
        {
            this.builtInParameterUpdater.Unregister();
        }

        [Test]
        public void WallTests_CreateWall()
        {
            Wall wall = null;

            using (Transaction transaction = new Transaction(document))
            {
                transaction.Start("CreateWall");

                var start = new XYZ(0, 0, 0);
                var end = new XYZ(10, 0, 0);
                wall = document.CreateWall(start, end);

                transaction.Commit();
            }

            Assert.IsNotNull(wall);
            Assert.AreEqual(10, wall.GetLenght());

            using (Transaction transaction = new Transaction(document))
            {
                transaction.Start("ExtendWall");

                wall.Extend(-1, 1);

                transaction.Commit();
            }

            Assert.AreEqual(12, wall.GetLenght());
            Assert.IsTrue(builtInParameterUpdater.ElementIdChangeType[wall.Id].Contains(BuiltInParameter.INVALID));

            using (Transaction transaction = new Transaction(document))
            {
                transaction.Start("CommentsWall");

                wall.SetComments("Comments");

                transaction.Commit();
            }

            Assert.IsTrue(builtInParameterUpdater.ElementIdChangeType[wall.Id].Contains(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS));

            using (Transaction transaction = new Transaction(document))
            {
                transaction.Start("DelteWall");

                document.Delete(wall.Id);

                transaction.Commit();
            }
        }
    }
}
