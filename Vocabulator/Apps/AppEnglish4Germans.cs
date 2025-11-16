using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vocabulator.Common;
using Vocabulator.Domain.Interface;
using Vocabulator.Domain.Services.QuestionTypes.English4Germans;

namespace Vocabulator.Apps
{
	internal class AppEnglish4Germans : IAppLanguageDefinition
	{
		private readonly Processor4EnglishGerman4Germans _processorEnglishGerman4Germans;
		private readonly Processor4GermanEnglish4Germans _processorGermanEnglish4Germans;

		public AppEnglish4Germans(IAiEngineFactory openAiFactory, string questionFilePath)
		{
			_processorEnglishGerman4Germans = new Processor4EnglishGerman4Germans(openAiFactory, questionFilePath: questionFilePath);
			_processorGermanEnglish4Germans = new Processor4GermanEnglish4Germans(openAiFactory, questionFilePath: questionFilePath);
		}
		public bool Handles(string motherLanguage, string foreignLanguage)
		{
			return motherLanguage == "German" && foreignLanguage == "English";
		}

		public IProcessorBase? TryGetProcessor(bool isWordInMotherLanguage)
		{
			return isWordInMotherLanguage ? _processorGermanEnglish4Germans : _processorEnglishGerman4Germans;
		}
	}
}
