using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChangeMe
{
	public sealed class IniFileReader
	{
		private readonly Dictionary<string, Dictionary<string, string>> m_Sections;

		/// <summary>
		/// Creates a new instance of the IniFileReader class, reading the
		/// specified INI file and storing its sections, keys, and values
		/// in memory.
		/// </summary>
		/// <param name="file">
		/// The full path to the INI file to read.
		/// </param>
		/// <param name="encoding">
		/// The text encoding to use when reading the INI file. Defaults to 
		/// UTF-8 encoding if set null or omitted.
		/// </param>
		public IniFileReader(string file, Encoding encoding = null)
		{
			if (encoding == null)
				encoding = Encoding.UTF8;

			if (file == null)
				throw new ArgumentNullException("file");

			if (!File.Exists(file))
				throw new FileNotFoundException("The specified INI file does not exist.", file);

			m_Sections = new Dictionary<string, Dictionary<string, string>>();

			string curSection = null;

			using (StreamReader sr = new StreamReader(file, encoding))
				while (!sr.EndOfStream)
				{
					string line = sr.ReadLine().Trim();

					if (line.Length == 0 || line[0] == ';')
						continue;

					if (line[0] == '[' && line[line.Length - 1] == ']')
					{
						curSection = line.Substring(1, line.Length - 2).ToUpperInvariant();
						if (!m_Sections.ContainsKey(curSection))
							m_Sections.Add(curSection, new Dictionary<string, string>());

						continue;
					}

					int idx = line.IndexOf('=');

					if (idx != -1 && curSection != null)
					{
						string key = line.Substring(0, idx).ToUpperInvariant().Trim();
						string value = line.Substring(idx + 1);

						int offsetComment = 0;

						// Skip all of the parsing if there is no value.
						if (value.Length == 0)
							goto addIt;

						// Do not check for comments inside quoted values
						if (value[0] == '"')
							offsetComment = value.IndexOf('"', 1);

						if (offsetComment != -1 && offsetComment + 1 < value.Length)
						{
							idx = value.IndexOf(';', offsetComment);
							if (idx != -1)
								value = value.Substring(0, idx);
						}

						value = value.Trim();

						// Remove quotes
						if (value.Length > 0 && value[0] == '"' && value[value.Length - 1] == '"')
							value = value.Substring(1, value.Length - 2);

						addIt:
						if (m_Sections[curSection].ContainsKey(key))
							m_Sections[curSection][key] = value;
						else
							m_Sections[curSection].Add(key, value);

						continue;
					}

					throw new InvalidDataException("Could not parse: \"" + line + "\"");
				}
		}

		/// <summary>
		/// Gets the value identified by the specified key and the specified
		/// section of the INI file.
		/// </summary>
		/// <param name="section">
		/// The name of the section that contains the key.
		/// </param>
		/// <param name="key">
		/// The name of the key that contains the value.
		/// </param>
		/// <param name="valueIfMissing">
		/// An optional fallback value to use if the section or key does not
		/// exist. Defaults to null.
		/// </param>
		/// <returns>
		/// The value of the specified key in the specified section or the
		/// default value.
		/// </returns>
		public string Get(string section, string key, string valueIfMissing = null)
		{
			if (section == null)
				throw new ArgumentNullException("section");

			if (key == null)
				throw new ArgumentNullException("key");

			section = section.ToUpperInvariant();
			key = key.ToUpperInvariant();

			if (!m_Sections.ContainsKey(section))
				return valueIfMissing;

			if (!m_Sections[section].ContainsKey(key))
				return valueIfMissing;

			return m_Sections[section][key];
		}
	}
}
