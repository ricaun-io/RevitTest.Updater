using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;

namespace RevitTest.Updater.Utils
{
    public static class WallUtils
    {
        public static Wall CreateWall(this Document document, XYZ start, XYZ end)
        {
            var line = Line.CreateBound(start, end);
            var level = GetFirstLevel(document);

            var wall = Wall.Create(document, line, level.Id, false);
            return wall;
        }

        public static Wall Extend(this Wall wall, double startExtend, double endExtend = 0)
        {
            var location = wall.Location as LocationCurve;

            var curve = location.Curve;
            curve.MakeBound(curve.GetEndParameter(0) + startExtend, curve.GetEndParameter(1) + endExtend);

            location.Curve = curve;

            return wall;
        }

        public static double GetLenght(this Wall wall)
        {
            return wall.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
        }

        public static double GetArea(this Wall wall)
        {
            return wall.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble();
        }

        public static double GetVolume(this Wall wall)
        {
            return wall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble();
        }

        public static void SetComments(this Wall wall, string comments)
        {
            wall.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set(comments);
        }

        private static Level GetFirstLevel(Document document)
        {
            return new FilteredElementCollector(document)
                .OfCategory(BuiltInCategory.OST_Levels)
                .WhereElementIsNotElementType()
                .Cast<Level>()
                .OrderBy(w => w.Elevation)
                .First();
        }
    }
}