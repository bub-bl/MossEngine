using MossEngine.Editor;
using MossEngine.WebGpu;

// var interceptor = new ConsoleInterceptor( Console.Out );
//
// ConsoleInterceptor.OnWrite += ( msg ) =>
// {
// 	// Ici tu captures chaque log console
// 	// Tu peux stocker, filtrer ou envoyer ailleurs.
// 	Console.Error.WriteLine( $"[CAPTURE] {msg}" );
// };
//
// Console.SetOut( interceptor );

WebGpuApi.Initialize();

var window = new EditorWindow();
window.Run();
