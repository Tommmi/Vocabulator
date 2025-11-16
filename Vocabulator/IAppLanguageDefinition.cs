using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vocabulator.Domain.Interface;

namespace Vocabulator
{
	internal interface IAppLanguageDefinition
	{
		bool Handles(string motherLanguage, string foreignLanguage);
		IProcessorBase? TryGetProcessor(bool isWordInMotherLanguage);
	}
}
