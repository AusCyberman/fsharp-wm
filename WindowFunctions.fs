module WindowFunctions 
open System
open System.Runtime.InteropServices
open System.Runtime.InteropServices.ComTypes
type HWND = IntPtr
[<StructLayout(LayoutKind.Sequential)>]
type tagRect = 
    struct
        [<DefaultValue>] val left : int32   
        [<DefaultValue>] val top : int32
        [<DefaultValue>] val right : int32
        [<DefaultValue>] val bottom : int32
    end
[<StructLayout(LayoutKind.Sequential)>]
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
[<DllImport("user32.dll")>]
extern bool IsWindowVisible(HWND hwnd)
[<DllImport("user32.dll")>]
extern void SwitchToThisWindow(HWND hwnd)
[<DllImport("user32.dll")>]
extern bool IsIconic(HWND hwnd)
let windows : IntPtr list ref = ref []

let EnumTopWindows () =
    let windowFunc_ x _ =
            let window = GetParent(x)
            let stringBuilder = StringBuilder(50)
            GetWindowTextA (window,stringBuilder,50)
            if List.contains window !windows || stringBuilder.Length = 0 || (stringBuilder.ToString ())= "Program Manager" || not (IsWindowVisible(window)) || IsIconic(window)then
                true
            else
                //SwitchToThisWindow(window)
                let mutable windowStruct =  TagWindowInfo ()
                windowStruct.cbSize <- Marshal.SizeOf(windowStruct)
                printfn "%b" (GetWindowInfo(window,&windowStruct))
                let windowRect = windowStruct.rcWindow
                printfn "Up %d Down %d Left %d Right %d" windowRect.top windowRect.bottom windowRect.left windowRect.right
                windows := window :: !windows
                printfn "Title: %s " (stringBuilder.ToString()) 
                true

    let windowFunc = new EnumWindowsProc(windowFunc_)
    EnumWindows(windowFunc,IntPtr.Zero)
    printfn "%d" (List.length !windows)


    