using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Faker;
using JT76.Data.Models;

namespace JT76.Data.Factories
{
    //http://10consulting.com/2011/11/18/populating-test-data-using-c-sharp/

    //__Contact info__

    //var name = Faker.Name.FullName();  // "Alene Hayes"
    //Faker.Internet.Email(name);  // "alene_hayes@hartmann.co.uk"
    //Faker.Internet.UserName(name);  // "alene.hayes"

    //Faker.Internet.Email();  // "morris@friesen.us"
    //Faker.Internet.FreeEmail();  // "houston_purdy@yahoo.com"

    //Faker.Internet.DomainName();  // "larkinhirthe.com"

    //Faker.Phone.Number();  // "(033)216-0058 x0344"


    //__Addresses__

    //Faker.Address.StreetAddress();  // "52613 Turcotte Lock"
    //Faker.Address.SecondaryAddress();  // "Suite 656"
    //Faker.Address.City();  // "South Wavaside"

    //Faker.Address.UkCounty();  // "West Glamorgan"
    //Faker.Address.UkPostCode().ToUpper();  // "BQ7 3AM"

    //Faker.Address.UsState();  // "Tennessee"
    //Faker.Address.ZipCode();  // "66363-7828"


    //__Lorem Ipsum sentences and paragraphs__

    //Faker.Lorem.Sentence();  // "Voluptatem repudiandae necessitatibus assumenda dolor illo maiores in."
    //Faker.Lorem.Paragraph();  /* "Rerum dolor cumque cum animi consequatur praesentium. Enim quia quia modi est ut. Dolores qui debitis qui perspiciatis autem quas. Expedita distinctio earum aut. Delectus assumenda rerum quibusdam harum iusto." */


    //__company names, catchphrases and bs__

    //Faker.Company.Name();  // "Dickens Group"
    //Faker.Company.CatchPhrase();  // "User-centric neutral internet solution"
    //Faker.Company.BS();  // "transition proactive solutions"

    public static class JtMockFactory
    {
        public static IQueryable<Error> GetErrorMocks()
        {
            var errorList = new List<Error>();

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    ErrorFactory.GetThrownException();
                }
                catch (Exception e)
                {
                    errorList.Add(ErrorFactory.GetErrorFromException(e, ErrorLevels.Critical, Lorem.Sentence()));
                }
            }

            int count = 1;
            foreach (Error item in errorList)
            {
                item.Id = count++;
                item.DtCreated = DateTime.UtcNow;
            }

            return errorList.AsQueryable();
        }


        public static IQueryable<LogMessage> GetLogMessageMocks()
        {
            var logMessageOne = new LogMessage
            {
                StrLogMessage = GetFakerParagraphs(1)
            };
            var logMessageTwo = new LogMessage
            {
                StrLogMessage = GetFakerParagraphs(2)
            };
            var logMessageThree = new LogMessage
            {
                StrLogMessage = GetFakerParagraphs(3)
            };

            var logMessageList = new List<LogMessage> {logMessageOne, logMessageTwo, logMessageThree};

            int count = 1;
            foreach (LogMessage item in logMessageList)
            {
                item.Id = count++;
                item.DtCreated = DateTime.UtcNow;
            }

            return logMessageList.AsQueryable();
        }

        public static IQueryable<Topic> GetTopicMocks()
        {
            var topicOne = new Topic
            {
                StrTitle = Lorem.Sentence(),
                StrBody = GetFakerParagraphs(1),
                Replies = new List<Reply>
                {
                    new Reply
                    {
                        StrBody = GetFakerParagraphs(1)
                    },
                    new Reply
                    {
                        StrBody = GetFakerParagraphs(2)
                    },
                    new Reply
                    {
                        StrBody = GetFakerParagraphs(3)
                    }
                }
            };

            var topicTwo = new Topic
            {
                StrTitle = Lorem.Sentence(),
                StrBody = GetFakerParagraphs(2),
                Replies = new List<Reply>
                {
                    new Reply
                    {
                        StrBody = GetFakerParagraphs(1)
                    },
                    new Reply
                    {
                        StrBody = GetFakerParagraphs(2)
                    }
                }
            };

            var topicThree = new Topic
            {
                StrTitle = Lorem.Sentence(),
                StrBody = GetFakerParagraphs(3),
                Replies = new List<Reply>
                {
                    new Reply
                    {
                        StrBody = GetFakerParagraphs(1)
                    }
                }
            };

            var topicFour = new Topic
            {
                StrTitle = Lorem.Sentence(),
                StrBody = GetFakerParagraphs(4)
            };

            var topicList = new List<Topic> {topicOne, topicTwo, topicThree, topicFour};

            int count = 1;
            foreach (Topic item in topicList)
            {
                item.DtCreated = DateTime.UtcNow;

                if (item.Replies == null)
                {
                    item.Id = count++;
                    continue;
                }

                int replyCount = 1;
                foreach (Reply reply in item.Replies)
                {
                    reply.Id = replyCount++;
                    reply.DtCreated = DateTime.UtcNow;
                    reply.TopicId = count;
                }

                item.Id = count++;
            }

            return topicList.AsQueryable();
        }

        public static string GetFakerParagraphs(int nParagraphCount)
        {
            //return Faker.Lorem.Paragraphs(nParagraphCount).Aggregate("", (current, item) => current + (item + Environment.NewLine));

            var sb = new StringBuilder();
            foreach (string item in Lorem.Paragraphs(nParagraphCount))
            {
                sb.AppendLine(item);
            }

            return sb.ToString();
        }
    }
}