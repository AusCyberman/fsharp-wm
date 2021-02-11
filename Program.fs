// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

module Program
open System
open System.Runtime.InteropServices
open System.Runtime.InteropServices.ComTypes

    
[<EntryPoint>]
let main argv =
    WindowFunctions.EnumTopWindows ()
    0 // return an integer exit code