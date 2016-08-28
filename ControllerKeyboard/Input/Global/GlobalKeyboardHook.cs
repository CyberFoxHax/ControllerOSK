using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ControllerKeyboard.Input.Global{
	//Based on https://gist.github.com/Stasonix
	public class GlobalKeyboardHook : IDisposable{
		public event EventHandler<GlobalKeyboardHookEventArgs> KeyboardPressed;

		public GlobalKeyboardHook(){
			_windowsHookHandle = IntPtr.Zero;
			_user32LibraryHandle = IntPtr.Zero;
			_hookProc = LowLevelKeyboardProc;
				// we must keep alive _hookProc, because GC is not aware about SetWindowsHookEx behaviour.

			_user32LibraryHandle = LoadLibrary("User32");
			if (_user32LibraryHandle == IntPtr.Zero){
				var errorCode = Marshal.GetLastWin32Error();
				throw new Win32Exception(errorCode,
					string.Format("Failed to load library 'User32.dll'. Error {0}: {1}.", errorCode,
						new Win32Exception(Marshal.GetLastWin32Error()).Message));
			}


			_windowsHookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, _hookProc, _user32LibraryHandle, 0);
			if (_windowsHookHandle == IntPtr.Zero){
				var errorCode = Marshal.GetLastWin32Error();
				throw new Win32Exception(errorCode,
					string.Format("Failed to adjust keyboard hooks for '{0}'. Error {1}: {2}.", Process.GetCurrentProcess().ProcessName,
						errorCode, new Win32Exception(Marshal.GetLastWin32Error()).Message));
			}
		}

		protected virtual void Dispose(bool disposing){
			if (disposing){
				// because we can unhook only in the same thread, not in garbage collector thread
				if (_windowsHookHandle != IntPtr.Zero){
					if (!UnhookWindowsHookEx(_windowsHookHandle)){
						var errorCode = Marshal.GetLastWin32Error();
						throw new Win32Exception(errorCode,
							String.Format("Failed to remove keyboard hooks for '{0}'. Error {1}: {2}.",
								Process.GetCurrentProcess().ProcessName, errorCode, new Win32Exception(Marshal.GetLastWin32Error()).Message));
					}
					_windowsHookHandle = IntPtr.Zero;

					// ReSharper disable once DelegateSubtraction
					_hookProc -= LowLevelKeyboardProc;
				}
			}

			if (_user32LibraryHandle != IntPtr.Zero){
				if (!FreeLibrary(_user32LibraryHandle)) // reduces reference to library by 1.
				{
					var errorCode = Marshal.GetLastWin32Error();
					throw new Win32Exception(errorCode,
						string.Format("Failed to unload library 'User32.dll'. Error {0}: {1}.", errorCode,
							new Win32Exception(Marshal.GetLastWin32Error()).Message));
				}
				_user32LibraryHandle = IntPtr.Zero;
			}
		}

		~GlobalKeyboardHook(){
			Dispose(false);
		}

		public void Dispose(){
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private IntPtr _windowsHookHandle;
		private IntPtr _user32LibraryHandle;
		private HookProc _hookProc;

		delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("kernel32.dll")]
		private static extern IntPtr LoadLibrary(string lpFileName);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private static extern bool FreeLibrary(IntPtr hModule);

		/// <summary>
		/// The SetWindowsHookEx function installs an application-defined hook procedure into a hook chain.
		/// You would install a hook procedure to monitor the system for certain types of events. These events are
		/// associated either with a specific thread or with all threads in the same desktop as the calling thread.
		/// </summary>
		/// <param name="idHook">hook type</param>
		/// <param name="lpfn">hook procedure</param>
		/// <param name="hMod">handle to application instance</param>
		/// <param name="dwThreadId">thread identifier</param>
		/// <returns>If the function succeeds, the return value is the handle to the hook procedure.</returns>
		[DllImport("USER32", SetLastError = true)]
		static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);

		/// <summary>
		/// The UnhookWindowsHookEx function removes a hook procedure installed in a hook chain by the SetWindowsHookEx function.
		/// </summary>
		/// <param name="hhk">handle to hook procedure</param>
		/// <returns>If the function succeeds, the return value is true.</returns>
		[DllImport("USER32", SetLastError = true)]
		public static extern bool UnhookWindowsHookEx(IntPtr hHook);

		/// <summary>
		/// The CallNextHookEx function passes the hook information to the next hook procedure in the current hook chain.
		/// A hook procedure can call this function either before or after processing the hook information.
		/// </summary>
		/// <param name="hHook">handle to current hook</param>
		/// <param name="code">hook code passed to hook procedure</param>
		/// <param name="wParam">value passed to hook procedure</param>
		/// <param name="lParam">value passed to hook procedure</param>
		/// <returns>If the function succeeds, the return value is true.</returns>
		[DllImport("USER32", SetLastError = true)]
		static extern IntPtr CallNextHookEx(IntPtr hHook, int code, IntPtr wParam, IntPtr lParam);

		public const int WH_KEYBOARD_LL = 13;

		public IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam){
			var fEatKeyStroke = false;

			var wparamTyped = wParam.ToInt32();
			if (Enum.IsDefined(typeof (KeyboardState), wparamTyped)){
				var o = Marshal.PtrToStructure(lParam, typeof (LowLevelKeyboardInputEvent));
				var p = (LowLevelKeyboardInputEvent) o;

				var eventArguments = new GlobalKeyboardHookEventArgs(p, (KeyboardState) wparamTyped);

				var handler = KeyboardPressed;
				if (handler != null) handler.Invoke(this, eventArguments);

				fEatKeyStroke = eventArguments.Handled;
			}

			return fEatKeyStroke ? (IntPtr) 1 : CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
		}
	}
}