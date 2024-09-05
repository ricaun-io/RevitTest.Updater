# RevitTest.Updater

[![Revit 2021](https://img.shields.io/badge/Revit-2021+-blue.svg)](../..)
[![Visual Studio 2022](https://img.shields.io/badge/Visual%20Studio-2022-blue)](../..)
[![Nuke](https://img.shields.io/badge/Nuke-Build-blue)](https://nuke.build/)
[![License MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Build](../../actions/workflows/Build.yml/badge.svg)](../../actions)

## Updater

The sample project to test `IUpdate` using the [ricaun.RevitTest](https://ricaun.com/RevitTest) Framework.

### WallUpdaterTests

This test uses the `BuiltInParameterUpdater` to register all the `BuiltInParameter` to check what parameter is changed when the wall is modified.

Two thing is tested, the wall `Comments` changed and the wall `Curve` length changed.

* When the wall `Comments` parameter change, the update trigger with `BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS`, like expected.

* When the wall is extended or the geometry lenght is changed, the update not trigger with `BuiltInParameter.CURVE_ELEM_LENGTH` as the source of the updater, the `INVALID` is used to indicate that the updater did not find the parameter that was changed.

## License

This project is [licensed](LICENSE) under the [MIT License](https://en.wikipedia.org/wiki/MIT_License).

---

Do you like this project? Please [star this project on GitHub](../../stargazers)!