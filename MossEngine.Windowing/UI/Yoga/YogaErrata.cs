using Yoga;

namespace MossEngine.Windowing.UI.Yoga;

public enum YogaErrata
{
	None = YGErrata.YGErrataNone,
	StretchFlexBasic = YGErrata.YGErrataStretchFlexBasis,
	AbsolutePositionWithoutInsetsExcludesPadding = YGErrata.YGErrataAbsolutePositionWithoutInsetsExcludesPadding,
	AbsolutePercentAgainstInnerSize = YGErrata.YGErrataAbsolutePercentAgainstInnerSize,
	All = YGErrata.YGErrataAll,
	Classic = YGErrata.YGErrataClassic
}
