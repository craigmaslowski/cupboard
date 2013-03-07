using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using CommandLine;
using CommandLine.Text;

namespace Cupboard
{
	class Options
	{
		[Option('s', "source", Required = true,
		  HelpText = "Path to root template folder to be processed.")]
		public string Source { get; set; }

		[Option('d', "destination", Required = true,
		  HelpText = "Final destination of the compiled template file.")]
		public string Destination { get; set; }

		[Option('e', "extension", DefaultValue = "html",
		  HelpText = "Extension of source template files. Default: *.html")]
		public string Extension { get; set; }

		[Option('v', "variable", DefaultValue = "templates",
		  HelpText = "Name of JavaScript variable that stores the template hash. Default: templates")]
		public string Variable { get; set; }
		
		[Option('c', "compile", DefaultValue = true,
		  HelpText = "Compile underscore templates to functions. Default: false")]
		public bool Compile { get; set; }

		[ParserState]
		public IParserState LastParserState { get; set; }

		[HelpOption]
		public string GetUsage()
		{
			return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
		}
	}

	class Program
	{
		static void Main(string[] args)
		{
			var options = new Options();
			if (Parser.Default.ParseArguments(args, options) && ValidateOptions(options))
			{
				var templates = string.Format("var {0} = {1};", 
					options.Variable, 
					ProcessDirectory(options.Source, options.Extension, options.Compile, false));

				try
				{
					using (var outfile = new StreamWriter(options.Destination))
					{
						outfile.Write(templates);
					}
				}
				catch (Exception e)
				{
					Console.WriteLine("The template file {0} could not be created: {0}", e.Message);
				}
				Console.WriteLine("Template file created at: {0}", options.Destination);
			}

			//Console.ReadLine();
		}

		static string ProcessDirectory(string directory, string extension, bool compileWithUnderscore, bool addTrailingComma)
		{
			//var templates = new Dictionary<string, object>();
			var directories = Directory.GetDirectories(directory);
			var files = Directory.GetFiles(directory);

			if (directories.Length == 0 && files.Length == 0)
			{
				return "null";
			}

			var template = "{";
			for (var i = 0; i < files.Length; i++)
			{
				const string propertyPattern = "\"{0}\":\"{1}\"";
				const string compiledPropertyPattern = "\"{0}\":_.template(\"{1}\")";

				if (Path.GetExtension(files[i]) != "." + extension) continue;
				
				var propName = Path.GetFileNameWithoutExtension(files[i]);
				if (propName == null) continue;
				
				try
				{
					using (var sr = new StreamReader(files[i]))
					{
						template += string.Format((compileWithUnderscore ? compiledPropertyPattern : propertyPattern), 
							propName, sr.ReadToEnd());
					}
				}
				catch (Exception e)
				{
					Console.WriteLine("The file {0} could not be read: {1}", files[i], e.Message);
				}

				// only add a comma if we have more properties to come in the object
				if (i != files.Length - 1 && directories.Length == 0)
					template += ",";
			}

			// process sub directories
			for (var i = 0; i < directories.Length; i++)
			{
				const string propertyPattern = "\"{0}\":{1}";
				const string dirPattern = @"\\([\w]+)$";
				var matches = Regex.Match(directories[i], dirPattern);
				if (matches.Groups.Count == 2)
				{
					var propertyName = matches.Groups[1];
					template += string.Format(
						propertyPattern, 
						propertyName,
						ProcessDirectory(directories[i], extension, compileWithUnderscore, (i != directories.Length - 1))
					);
				}
			}

			return template + "}" + (addTrailingComma ? "," : string.Empty);
		}

		static bool ValidateOptions(Options options)
		{
			if (!Directory.Exists(options.Source))
			{
				Console.WriteLine("Could not find the source directory: {0}", options.Source);
				return false;
			}

			var destinationDirectory = Path.GetDirectoryName(options.Destination);
			if (!Directory.Exists(destinationDirectory))
			{
				Console.WriteLine("Could not find the destination directory: {0}", destinationDirectory);
				return false;
			}

			return true;
		}
	}
}
