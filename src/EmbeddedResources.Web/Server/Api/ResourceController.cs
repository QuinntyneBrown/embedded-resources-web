using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web.Http;

namespace EmbeddedResources.Web.Server.Api
{
    public class ResourceController : ApiController
    {
        private const string ResourcePath = "EmbeddedResources.Static{0}";

        public static Stream GetStream(string folderAndFileInProjectPath)
        {
            var asm = System.Web.Compilation.BuildManager.GetReferencedAssemblies()
                .Cast<Assembly>()
                .Where(a => a.GetName().Name == "EmbeddedResources.Static").Single();
            var resource = string.Format(ResourcePath, folderAndFileInProjectPath);
            return asm.GetManifestResourceStream(resource);
        }


        public HttpResponseMessage Get()
        {
            string filename = this.Request.RequestUri.PathAndQuery;
                        
            if (filename == "/")
            {
                filename = ".index.html";
            }
            
            filename = filename.Replace("/", ".");
            filename = filename.Replace("-", "_");
            var mimeType = System.Web.MimeMapping.GetMimeMapping(filename);
            var fileStream = GetStream(filename);
            if (fileStream != null)
            {
                var response = new HttpResponseMessage();
                response.Content = new StreamContent(fileStream);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
                return response;
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
        }
    }
}
