using System.Collections.Generic;

namespace JsonToResx.Cli
{
	/// <summary>
	/// Options for <see cref="ResxToJsonConverter"/>.
	/// </summary>
	public class JsonToResxConverterOptions
	{
		public JsonToResxConverterOptions()
		{
		}

		/// <summary>
		/// Raw inputs (files and dirs).
		/// </summary>
		public List<string> Inputs { get; private set; } = new List<string>();

        /// <summary>
        /// Input folders full pathes.
        /// </summary>
        public List<string> InputFolders { get; private set; } = new List<string>();

        /// <summary>
        /// Input files.
        /// </summary>
        public List<string> InputFiles { get; private set; } = new List<string>();

        /// <summary>
        /// Output folder full path.
        /// </summary>
        public string OutputFolder { get; set; }

        /// <summary>
        /// Output file name.
        /// </summary>
        public string OutputFile { get; set; } = "Resources.resx";

        /// <summary>
        /// The key separator (Default: ".")
        /// </summary>
        public string KeySeparator { get; set; } = ".";

        /// <summary>
        /// When outputing i18next files the "root" culture goes in 
        /// it's own subdirectory - effectively forming it's own custom 
        /// culture which, by default, is "dev". A better option is to 
        /// call this something like "en" (if our original resources are 
        /// in English) or "fr" or something. That way the fallback will 
        /// also be a logical fallback for specific locales such as en-US
        /// or fr-FR.
        /// </summary>
	    public string FallbackCulture { get; set; } = "en";

		/// <summary>
		/// Overwrite existing files. 
		/// </summary>
		public OverwriteModes Overwrite { get; set; } = OverwriteModes.Ask;
	}

	public enum OverwriteModes
	{
		Skip = 0,
		Ask,
		Force
	}

}