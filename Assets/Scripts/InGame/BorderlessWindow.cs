using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class BorderlessWindow : MonoBehaviour
{
    // DLL Import
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

    // Constants for window styles
    private const int GWL_STYLE = -16;
    private const uint WS_BORDER = 0x00800000;
    private const uint WS_POPUP = 0x80000000;
    private const uint WS_SYSMENU = 0x00080000;

    void Start()
    {
        // Set the window style to borderless
        SetBorderless();
    }

    void SetBorderless()
    {
        // Get the current window
        IntPtr hWnd = GetForegroundWindow();

        // Get the current window style
        uint style = GetWindowLong(hWnd, GWL_STYLE);

        // Update the window style to remove borders and buttons
        style &= ~WS_BORDER;  // Remove border
        style &= ~WS_SYSMENU; // Remove minimize and maximize buttons
        SetWindowLong(hWnd, GWL_STYLE, style);
    }
}
