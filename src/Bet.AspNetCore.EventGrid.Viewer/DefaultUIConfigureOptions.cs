﻿using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace Bet.AspNetCore.EventGrid.Viewer
{
    public class DefaultUIConfigureOptions : IPostConfigureOptions<StaticFileOptions>
    {
#if NETSTANDARD2_0
        public DefaultUIConfigureOptions(IHostingEnvironment environment)
        {
            Environment = environment;
        }

        public IHostingEnvironment Environment { get; }

#else
        public DefaultUIConfigureOptions(IWebHostEnvironment environment)
        {
            Environment = environment;
        }

        public IWebHostEnvironment Environment { get; }

#endif
        public void PostConfigure(string name, StaticFileOptions options)
        {
            name = name ?? throw new ArgumentNullException(nameof(name));
            options = options ?? throw new ArgumentNullException(nameof(options));

            // Basic initialization in case the options weren't initialized by any other component
            options.ContentTypeProvider = options.ContentTypeProvider ?? new FileExtensionContentTypeProvider();
            if (options.FileProvider == null && Environment.WebRootFileProvider == null)
            {
                throw new InvalidOperationException("Missing FileProvider.");
            }

            options.FileProvider = options.FileProvider ?? Environment.WebRootFileProvider;

            var basePath = "wwwroot";

            // Add our provider
            var filesProvider = new ManifestEmbeddedFileProvider(GetType().Assembly, basePath);
            options.FileProvider = new CompositeFileProvider(options.FileProvider, filesProvider);
        }
    }
}
