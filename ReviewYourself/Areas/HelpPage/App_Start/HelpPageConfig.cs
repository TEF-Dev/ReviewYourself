// Uncomment the following to provide samples for PageResult<T>. Must also add the Microsoft.AspNet.WebApi.OData
// package to your project.
////#define Handle_PageResultOfT

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web;
using System.Web.Http;
using ReviewYourself.Models;
using ReviewYourself.Models.Tools;

#if Handle_PageResultOfT
using System.Web.Http.OData;
#endif

namespace ReviewYourself.Areas.HelpPage
{
    /// <summary>
    /// Use this class to customize the Help Page.
    /// For example you can set a custom <see cref="System.Web.Http.Description.IDocumentationProvider"/> to supply the documentation
    /// or you can provide the samples for the requests/responses.
    /// </summary>
    public static class HelpPageConfig
    {
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
            MessageId = "ReviewYourself.Areas.HelpPage.TextSample.#ctor(System.String)",
            Justification = "End users may choose to merge this string with existing localized resources.")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly",
            MessageId = "bsonspec",
            Justification = "Part of a URI.")]
        public static void Register(HttpConfiguration config)
        {
            //// Uncomment the following to use the documentation from XML documentation file.
            //config.SetDocumentationProvider(new XmlDocumentationProvider(HttpContext.Current.Server.MapPath("~/App_Data/XmlDocument.xml")));

            //// Uncomment the following to use "sample string" as the sample for all actions that have string as the body parameter or return type.
            //// Also, the string arrays will be used for IEnumerable<string>. The sample objects will be serialized into different media type 
            //// formats by the available formatters.

            ResourceUser user = new ResourceUser()
            {
                Id = Guid.NewGuid(),
                Biography = "IM2STRN4DTWRLD",
                Email = "yourmain@gmail.com",
                FirstName = "Fredi",
                LastName = "Fowler",
                Login = "InredikaWb",
                Password = "1tsT0C0mpl1c4t3d"
            };
            Course course = new Course()
            {
                Description = "It's UML time!",
                Id = Guid.NewGuid(),
                Mentor = user,
                Title = "UML C2"
            };
            ResourceTask task = new ResourceTask()
            {
                CourseId = Guid.NewGuid(),
                Description = "Make america greate again!",
                Id = Guid.NewGuid(),
                PostTime = DateTime.Today,
                Title = "Very important task"
            };
            Solution solution = new Solution()
            {
                AuthorId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                PostTime = DateTime.Today,
                Status = false,
                TaskId = Guid.NewGuid(),
                TextData = "Here is a plan..."
            };
            Review review = new Review()
            {
                AuthorId = Guid.NewGuid(),
                PostTime = DateTime.Now,
                Id = Guid.NewGuid(),
                SolutionId = Guid.NewGuid()
            };
            RegistrationData reg = new RegistrationData()
            {
                Login = "login",
                Password = "p4ssword",
                FirstName = "Fredi",
                LastName = "Fowler"
            };
            config.SetSampleObjects(new Dictionary<Type, object>
            {
                {typeof(string), "sample string"},
                {typeof(Guid), Guid.NewGuid()},
                {typeof(Course), course},
                {typeof(Review), review},
                {typeof(ResourceTask), task},
                {typeof(ResourceUser), user},
                {typeof(Solution), solution},
                {typeof(Token), new Token {TokenData = Guid.NewGuid()}},
                {typeof(AuthorizeData), new AuthorizeData() { Login = "login", Password = "p4ssw0rd"}},
                {typeof(RegistrationData), reg},
            });


            // Extend the following to provide factories for types not handled automatically (those lacking parameterless
            // constructors) or for which you prefer to use non-default property values. Line below provides a fallback
            // since automatic handling will fail and GeneratePageResult handles only a single type.
#if Handle_PageResultOfT
            config.GetHelpPageSampleGenerator().SampleObjectFactories.Add(GeneratePageResult);
#endif

            // Extend the following to use a preset object directly as the sample for all actions that support a media
            // type, regardless of the body parameter or return type. The lines below avoid display of binary content.
            // The BsonMediaTypeFormatter (if available) is not used to serialize the TextSample object.
            config.SetSampleForMediaType(
                new TextSample("Binary JSON content. See http://bsonspec.org for details."),
                new MediaTypeHeaderValue("application/bson"));

            //// Uncomment the following to use "[0]=foo&[1]=bar" directly as the sample for all actions that support form URL encoded format
            //// and have IEnumerable<string> as the body parameter or return type.
            //config.SetSampleForType("[0]=foo&[1]=bar", new MediaTypeHeaderValue("application/x-www-form-urlencoded"), typeof(IEnumerable<string>));

            //// Uncomment the following to use "1234" directly as the request sample for media type "text/plain" on the controller named "Values"
            //// and action named "Put".
            //config.SetSampleRequest("1234", new MediaTypeHeaderValue("text/plain"), "Values", "Put");

            //// Uncomment the following to use the image on "../images/aspNetHome.png" directly as the response sample for media type "image/png"
            //// on the controller named "Values" and action named "Get" with parameter "id".
            //config.SetSampleResponse(new ImageSample("../images/aspNetHome.png"), new MediaTypeHeaderValue("image/png"), "Values", "Get", "id");

            //// Uncomment the following to correct the sample request when the action expects an HttpRequestMessage with ObjectContent<string>.
            //// The sample will be generated as if the controller named "Values" and action named "Get" were having string as the body parameter.
            //config.SetActualRequestType(typeof(Token), "Values", "Get");

            //// Uncomment the following to correct the sample response when the action returns an HttpResponseMessage with ObjectContent<string>.
            //// The sample will be generated as if the controller named "Values" and action named "Post" were returning a string.
            //config.SetActualResponseType(typeof(Token), "Values", "Post");
        }

#if Handle_PageResultOfT
        private static object GeneratePageResult(HelpPageSampleGenerator sampleGenerator, Type type)
        {
            if (type.IsGenericType)
            {
                Type openGenericType = type.GetGenericTypeDefinition();
                if (openGenericType == typeof(PageResult<>))
                {
                    // Get the T in PageResult<T>
                    Type[] typeParameters = type.GetGenericArguments();
                    Debug.Assert(typeParameters.Length == 1);

                    // Create an enumeration to pass as the first parameter to the PageResult<T> constuctor
                    Type itemsType = typeof(List<>).MakeGenericType(typeParameters);
                    object items = sampleGenerator.GetSampleObject(itemsType);

                    // Fill in the other information needed to invoke the PageResult<T> constuctor
                    Type[] parameterTypes = new Type[] { itemsType, typeof(Uri), typeof(long?), };
                    object[] parameters = new object[] { items, null, (long)ObjectGenerator.DefaultCollectionSize, };

                    // Call PageResult(IEnumerable<T> items, Uri nextPageLink, long? count) constructor
                    ConstructorInfo constructor = type.GetConstructor(parameterTypes);
                    return constructor.Invoke(parameters);
                }
            }

            return null;
        }
#endif
    }
}