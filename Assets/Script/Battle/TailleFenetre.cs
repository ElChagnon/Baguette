using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

public class TailleFenetre : MonoBehaviour
{
    [SerializeField] private int windowWidth = 1280;
    [SerializeField] private int windowHeight = 720;
    [SerializeField] private string windowTitle = "Know How I Die";

    [DllImport("user32.dll", EntryPoint = "SetWindowText")]
    private static extern bool SetWindowText(IntPtr hWnd, string lpString);

    [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    void Start()
    {
        #if !UNITY_EDITOR
            Screen.SetResolution(windowWidth, windowHeight, false);
            SetWindowTitle(windowTitle);
        #endif
    }

    void SetWindowTitle(string title)
    {
        #if UNITY_STANDALONE_WIN
            IntPtr hWnd = Process.GetCurrentProcess().MainWindowHandle;
            if (hWnd != IntPtr.Zero)
            {
                SetWindowText(hWnd, title);
            }
        #endif
    }
}