﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Duality.IO;

namespace Duality.Backend.DotNetFramework
{
	[DontSerialize]
	public class SystemBackend : ISystemBackend
	{
		private NativeFileSystem fileSystem = new NativeFileSystem();

		string IDualityBackend.Id
		{
			get { return "DotNetFrameworkSystemBackend"; }
		}
		string IDualityBackend.Name
		{
			get { return ".Net Framework"; }
		}
		int IDualityBackend.Priority
		{
			get { return 0; }
		}

		IFileSystem ISystemBackend.FileSystem
		{
			get { return this.fileSystem; }
		}

		bool IDualityBackend.CheckAvailable()
		{
			return true;
		}
		void IDualityBackend.Init()
		{
			// Write systems specs as a debug log
			{
				string osName = Environment.OSVersion != null ? Environment.OSVersion.ToString() : "Unknown";
				string osFriendlyName = null;
				if (Environment.OSVersion.Platform == PlatformID.Win32NT)
				{
					if (Environment.OSVersion.Version >= new Version(10, 0, 0))
						osFriendlyName = "Windows 10";
					else if (Environment.OSVersion.Version >= new Version(6, 3, 0))
						osFriendlyName = "Windows 8.1";
					else if (Environment.OSVersion.Version >= new Version(6, 2, 0))
						osFriendlyName = "Windows 8";
					else if (Environment.OSVersion.Version >= new Version(6, 1, 0))
						osFriendlyName = "Windows 7";
					else if (Environment.OSVersion.Version >= new Version(6, 0, 0))
						osFriendlyName = "Windows Vista";
					else if (Environment.OSVersion.Version >= new Version(5, 2, 0))
						osFriendlyName = "Windows XP 64 Bit Edition";
					else if (Environment.OSVersion.Version >= new Version(5, 1, 0))
						osFriendlyName = "Windows XP";
					else if (Environment.OSVersion.Version >= new Version(5, 0, 0))
						osFriendlyName = "Windows 2000";
				}
				Log.Core.Write("Operating System: {1}{0}64 Bit Process: {2}{0}CLR Version: {3}{0}Processor Count: {4}", 
					Environment.NewLine,
					osName + (osFriendlyName != null ? (" (" + osFriendlyName + ")") : ""),
					Environment.Is64BitProcess,
					Environment.Version,
					Environment.ProcessorCount);
			}
		}
		void IDualityBackend.Shutdown() { }

		string ISystemBackend.GetNamedPath(NamedDirectory dir)
		{
			switch (dir)
			{
				default:                             return null;
				case NamedDirectory.Current:         return System.IO.Directory.GetCurrentDirectory();
				case NamedDirectory.ApplicationData: return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				case NamedDirectory.MyDocuments:     return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				case NamedDirectory.MyMusic:         return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
				case NamedDirectory.MyPictures:      return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
			}
		}

		IEnumerable<Assembly> ISystemBackend.GetLoadedAssemblies()
		{
			return AppDomain.CurrentDomain.GetAssemblies();
		}
	}
}
