using MossEngine;
using MossEngine.Pipelines;
using Yoga;

Console.WriteLine("Yoga: Init");

unsafe
{
	var root = YG.NodeNew();
	YG.NodeStyleSetWidth(root, 130);
	YG.NodeStyleSetHeight(root, 50);
	YG.NodeStyleSetPadding(root, YGEdge.YGEdgeAll, 10);
	YG.NodeStyleSetFlexDirection(root, YGFlexDirection.YGFlexDirectionRow);
	YG.NodeStyleSetGap(root, YGGutter.YGGutterColumn, 10);
}

Console.WriteLine("Yoga: Done");

WebGpu.Initialize();

var window = new GameWindow();
window.Run();
// window.Dispose();

// var window2 = new GameWindow();
// window2.Run();

