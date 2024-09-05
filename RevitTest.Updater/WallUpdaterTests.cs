using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using NUnit.Framework;
using RevitTest.Updater.Utils;
using System.Linq;

namespace RevitTest.Updater
{
    public class WallUpdaterTests : OneTimeOpenDocumentTest
    {
        public BuiltInParameterUpdater builtInParameterUpdater;
        [OneTimeSetUp]
        public void UpdaterRegister(UIApplication uiapp)
        {
            this.builtInParameterUpdater = new BuiltInParameterUpdater(uiapp.ActiveAddInId);
            this.builtInParameterUpdater.Register(BuiltInParameter.CURVE_ELEM_LENGTH, BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
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
                System.Console.WriteLine($"CreateWall: {wall.Id}");

                transaction.Commit();
            }

            Assert.IsNotNull(wall);
            Assert.AreEqual(10, wall.GetLength());

            using (Transaction transaction = new Transaction(document))
            {
                transaction.Start("ExtendWall");

                wall.Extend(-1, 1);

                transaction.Commit();
            }

            Assert.AreEqual(12, wall.GetLength());

            // This should contain CURVE_ELEM_LENGTH but Revit does not consider the parameter as change using 'UpdaterData.IsChangeTriggered'.
            Assert.IsTrue(builtInParameterUpdater.ElementIdChangeType[wall.Id].Contains(BuiltInParameter.INVALID));
            Assert.IsFalse(builtInParameterUpdater.ElementIdChangeType[wall.Id].Contains(BuiltInParameter.CURVE_ELEM_LENGTH));

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
