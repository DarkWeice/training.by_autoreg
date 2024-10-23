using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainingByAutoReg
{
    internal class GenerateNewEmailAddress
    {
        public string GenerateNewEmailAddresss()
        {
            const string pattern = "abcdefghijklmnopqrstuvwxyz0123456789";
            var patternLength = pattern.Length;
            const int suffixLength = 10;
            var random = new Random();
            var generatedSuffix = Enumerable.Range(0, suffixLength)
                .Aggregate(
                    new StringBuilder(),
                    (builder, _) => builder.Append(pattern[random.Next(patternLength)]))
                .ToString();

            var originalAddress = new Aspose.Email.MailAddress("some_address@gmail.com");
            var emailAddress = new Aspose.Email.MailAddress(
                $"{originalAddress.User}{generatedSuffix}@{originalAddress.Host}").ToString();
            return emailAddress;
        }
    }
}
