using BandPageGenerator.Models;
using Xunit;

namespace Tests.Models
{
    public class FacebookModelsTest
    {
        [Fact]
        public static void ShouldCorrectlyFormatEventDescription()
        {
            var model = new FacebookEventModel
            {
                Description = "Lineone\nLineTwo\nhttp://www.google.com/?query=asdf"
            };

            Assert.Equal("Lineone<br/>LineTwo<br/><a href=\"http://www.google.com/?query=asdf\" target=\"_blank\">http://www.google.com/?query=asdf</a>", model.FormattedDescription);
        }
    }
}
