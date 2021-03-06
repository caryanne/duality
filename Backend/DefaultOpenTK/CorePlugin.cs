﻿using System;
using System.Reflection;
using System.Linq;
using System.Threading;
using System.Runtime.CompilerServices;

using Duality;
using Duality.Resources;
using Duality.Input;

using OpenTK;

namespace Duality.Backend.DefaultOpenTK
{
    public class DefaultOpenTKBackendPlugin : CorePlugin
	{
		private static Thread mainThread;

		protected override void InitPlugin()
		{
			base.InitPlugin();

			mainThread = Thread.CurrentThread;

			// Initialize OpenTK
			{
				bool inEditor = DualityApp.ExecEnvironment == DualityApp.ExecutionEnvironment.Editor;
				ToolkitOptions options = new ToolkitOptions
				{
					// Prefer the native backend in the editor, because it supports GLControl. SDL doesn't.
					Backend = inEditor ? PlatformBackend.PreferNative : PlatformBackend.Default,
					// Disable High Resolution support in the editor, because it's not DPI-Aware
					EnableHighResolution = !inEditor
				};
				Toolkit.Init(options);
			}

			// Register global / non-windowbound input devices
			GlobalGamepadInputSource.UpdateAvailableDecives(DualityApp.Gamepads);
			GlobalJoystickInputSource.UpdateAvailableDecives(DualityApp.Joysticks);
		}
		protected override void OnDisposePlugin()
		{
			base.OnDisposePlugin();

			foreach (GamepadInput gamepad in DualityApp.Gamepads.ToArray())
			{
				if (gamepad.Source is GlobalGamepadInputSource)
					DualityApp.Gamepads.RemoveSource(gamepad.Source);
			}
			foreach (JoystickInput joystick in DualityApp.Joysticks.ToArray())
			{
				if (joystick.Source is GlobalJoystickInputSource)
					DualityApp.Joysticks.RemoveSource(joystick.Source);
			}
		}
		
		/// <summary>
		/// Guards the calling method agains being called from a thread that is not the main thread.
		/// Use this only at critical code segments that are likely to be called from somewhere else than the main thread
		/// but aren't allowed to.
		/// </summary>
		/// <param name="backend"></param>
		/// <param name="silent"></param>
		/// <returns>True if everyhing is allright. False if the guarded state has been violated.</returns>
		[System.Diagnostics.DebuggerStepThrough]
		internal static bool GuardSingleThreadState(bool silent = false, [CallerMemberName] string callerInfoMember = null)
		{
			if (Thread.CurrentThread != mainThread)
			{
				if (!silent)
				{
					Log.Core.WriteError(
						"Method {0} isn't allowed to be called from a Thread that is not the main Thread.",
						callerInfoMember);
				}
				return false;
			}
			return true;
		}
	}
}
