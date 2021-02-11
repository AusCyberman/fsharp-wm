// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open System.Runtime.InteropServices
open System.Runtime.InteropServices.ComTypes
// Define a function to construct a message to print
let from whom =
    sprintf "from %s" whom

module WindowFunctions =
    open System
    open System.Runtime.InteropServices
    open System.Runtime.InteropServices.ComTypes
    [<StructLayout(LayoutKind.Sequential)>]
    type tagRect = 
        struct
            val left : int32   
            val top : int32
            val right : int32
            val bottom : int32
        end
    type TagWindowInfo = 
        struct
            [<DefaultValue>] val mutable cbSize :  int32
            [<DefaultValue>] val mutable rcWindow : tagRect
            [<DefaultValue>] val mutable rcClient : tagRect
            [<DefaultValue>] val mutable dwStyle : int32
            [<DefaultValue>] val mutable dwExStyle : int32
            [<DefaultValue>] val mutable dwWindowStatus : int32
            [<DefaultValue>] val mutable cxWindowBorders : uint
            [<DefaultValue>] val mutable cyWindowBorders : uint
            [<DefaultValue>] val mutable atomWindowType : uint16
            [<DefaultValue>] val mutable wCreatorVersion : uint16
        end
    open System.Text
    type EnumWindowsProc = delegate of IntPtr * IntPtr -> bool
    type HWND = IntPtr
    [<DllImport("user32.dll")>]
    extern bool EnumWindows(EnumWindowsProc callback,IntPtr lParam)
    [<DllImport("user32.dll")>]
    extern bool MoveWindow(HWND hwnd,int X,int Y, int nWidth,int nHeight,bool bRepaint)
    [<DllImport("user32.dll")>]
    extern int GetWindowTextA(HWND hwnd, StringBuilder lpString, int nMaxCount)
    [<DllImport("user32.dll")>]
    extern HWND GetParent(HWND hwnd)
    [<DllImport("user32.dll")>] 
    extern bool GetWindowInfo(HWND hwnd,TagWindowInfo& pwi)
    //[<DllImport>("user32.dll")]
    //extern HWND Get
    let windows : IntPtr list ref = ref []
    let EnumTopWindows () =
        let windowFunc_ x _ =
            let window = GetParent(x)
            let stringBuilder = StringBuilder(50)
            GetWindowTextA (window,stringBuilder,50)
            if List.contains window !windows || stringBuilder.Length = 0  then
                true
            else
                let mutable windowStruct =  TagWindowInfo ()
                windowStruct.cbSize <- Marshal.SizeOf(windowStruct)
                printfn "%d" windowStruct.cbSize
                printfn "%b" (GetWindowInfo(window,&windowStruct))
                //MoveWindow(window,100,100,100,100,false) |> ignore
                windows := window :: !windows
                printfn "Title: %s \n X: %d Y: %d Creator: %d" 
                    (stringBuilder.ToString()) 
                    windowStruct.cxWindowBorders
                     windowStruct.cyWindowBorders
                     windowStruct.wCreatorVersion
                true

        let windowFunc = new EnumWindowsProc(windowFunc_)
        EnumWindows(windowFunc,IntPtr.Zero)
        printfn "%d" (List.length !windows)

[<EntryPoint>]
let main argv =
    WindowFunctions.EnumTopWindows ()
    0 // return an integer exit code