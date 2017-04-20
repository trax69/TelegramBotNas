//
//  botSettingsChecks.cs
//
//  Author:
//       Big Cash Monkeys <admin@bcm.ovh>
//
//  Copyright (c) 2017 Big Cash Monkeys
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
namespace botSettings.Checks
{
	public class botChecks 
	{
		/// <summary>
		/// Checks the token.
		/// </summary>
		/// <returns><c>true</c>, if token was checked, <c>false</c> otherwise.</returns>
		/// <param name="token">Token.</param>
		public bool checkToken(string token)
		{
			bool _isOk = false;	// Dar por sentado que estará mal
			if (!string.IsNullOrEmpty(token) || !string.IsNullOrWhiteSpace(token))
			{
				if (System.Text.RegularExpressions.Regex.IsMatch(token, @"^\d*:[\w\d-_]{35}$"))
				{
					_isOk = true;
				}
			}
			return _isOk;
		}

		/// <summary>
		/// Checks the PW.
		/// </summary>
		/// <returns><c>true</c>, if PW was checked, <c>false</c> otherwise.</returns>
		/// <param name="pW">P w.</param>
		public bool checkPW(string pW)
		{
			bool _isOk = false;
			if (!string.IsNullOrEmpty (pW) || !string.IsNullOrWhiteSpace (pW))
			{
				_isOk = true;
			}
			return _isOk;
		}

        public bool checkPath(string path)
        {
            bool _isOk = false;
            if (Directory.Exists(path))
            {
                _isOk = true;
            }
            return _isOk;
        }

        public static bool isLinux () {
			int p = (int)System.Environment.OSVersion.Platform;
			return (p == 4) || (p == 6) || (p == 128);
		}

	}

}

